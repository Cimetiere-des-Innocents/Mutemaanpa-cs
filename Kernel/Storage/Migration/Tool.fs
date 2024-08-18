namespace Kernel

(* 
 Migration:
    This code prepares a correct initial state for the game session.

    During the setup period, the Session must ensure the save database
    is in a regular state because other db access codes require so.
    
    It calls the migrate function to upgrade save from old versions,
    or do nothing if already latest. This operation should be an 
    idempotent function, which is implemented via version numbers in
    an auxiliary table.
*)
module Migration =
    open DuckDB.NET.Data
    open Dapper
    open System.Data
    open System.Text.RegularExpressions
    open System.IO

    let TABLE_NAME = "_migration"

    (*
        Internal migration table, keeps record of what migrations has applied.

        By convention, we must arrange our migrations in serial versions
        V1, ..., V128. The save file must has an equal or smaller serial
        number than the program (i.e. we can open lower version data).
        We can migrate low version saves, but we cannot load data from higher versions.

        The table keeps track of the migration file checksum also, to verify
        that past migrations are consistent with what embedded in the code.
    *)
    let SCHEMA =
        $"""
        CREATE TABLE IF NOT EXISTS {TABLE_NAME} (
            version INTEGER,
            name TEXT,
            checksum INTEGER,
            migrated_time TIMESTAMP DEFAULT current_timestamp
        );
    """

    (*
        Migration file naming convention:
            "V" + version (monotone increasing) + "_" + description + ".sql"

        We require the version is discrete continuous. It is a very convenient property.
    *)
    let (|MigrationFileName|_|) (str: string) =
        let regex = Regex(@"V(\d+)_(\w+).sql", RegexOptions.Compiled)

        if regex.IsMatch str then
            let matched = regex.Match str
            Some(matched.Groups[1].Value |> int, matched.Groups[2].Value)
        else
            None

    let parseFileName s =
        match s with
        | MigrationFileName mfs -> mfs
        | _ -> failwith $"Found wrong formatting migration file {s} (unreachable)"

    [<Struct>]
    [<CLIMutable>]
    type MigrationTuple =
        { version: int
          name: string
          checksum: int
          content: string option }

    let isSameMigration m1 m2 =
        if m1.version <> m2.version then false
        elif m1.name <> m2.name then false
        elif m1.checksum <> m2.checksum then false
        elif m1.content.IsSome && m2.content.IsSome then m1 = m2
        else true

    (* Load all embedded migrations during module initialize period, sorted by version number. *)
    let allEmbeddedMigrations =
        let assembly = System.Reflection.Assembly.GetExecutingAssembly()

        let readMigration filename =
            let (version, name) = parseFileName filename
            use fileStream = assembly.GetManifestResourceStream(filename)
            use readStream = new StreamReader(fileStream)
            let content = readStream.ReadToEnd()

            { version = version
              name = name
              checksum = content.GetHashCode()
              content = Some content }

        let isMigration =
            function
            | MigrationFileName _ -> true
            | _ -> false

        let verifyIsSeq (mList: MigrationTuple list) =
            let sequential =
                mList
                |> Seq.zip (seq { 1 .. mList.Length })
                |> Seq.forall (fun (n, { version = v }) -> n = v)

            if sequential then
                mList
            else
                failwith "the migrations are not sequentially arranged, please check source code."

        assembly.GetManifestResourceNames()
        |> Seq.filter isMigration
        |> Seq.map readMigration
        |> Seq.sortBy _.version
        |> Seq.toList
        |> verifyIsSeq

    let migrate dbPath logger =
        use db = new DuckDBConnection(dbPath)
        use tx = db.BeginTransaction IsolationLevel.ReadCommitted
        db.Execute(SCHEMA, transaction = tx) |> ignore

        let allAppliedMigrations =
            db.Query(
                $"SELECT * FROM {TABLE_NAME} ORDER BY version;",
                (fun version name checksum ->
                    { version = version
                      name = name
                      checksum = checksum
                      content = None }),
                transaction = tx
            )
            |> Seq.sortBy _.version
            |> Seq.toList

        let doMigration
            { version = version
              name = name
              checksum = checksum
              content = content }
            =
            if content.IsSome then
                db.Execute(content.Value, transaction = tx) |> ignore
                logger $"executed migration {content.Value}"

                db.Execute(
                    $"""
                    INSERT INTO {TABLE_NAME}(version, name, checksum)
                    VALUES ($version, $name, $checksum)
                    """,
                    {| version = version
                       name = name
                       checksum = checksum |},
                    transaction = tx
                )
                |> ignore
            else
                failwith "you cannot do an empty migration"


        if allAppliedMigrations.Length > allEmbeddedMigrations.Length then
            failwith "the save file is newer than the game version"
        elif
            allAppliedMigrations
            |> List.zip allEmbeddedMigrations[0 .. allAppliedMigrations.Length - 1]
            |> List.exists (fun (m1, m2) -> not (isSameMigration m1 m2))
        then
            failwith "The save file has tainted past migrations"
        else
            allEmbeddedMigrations[allAppliedMigrations.Length ..]
            |> List.map doMigration
            |> ignore

            tx.Commit()
