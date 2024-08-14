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
module internal Migration =
    open DuckDB.NET.Data
    open Dapper
    open System.Data
    open System.Text.RegularExpressions
    open System.IO

    let TABLE_NAME = "_migration"

    (*
        Internal migration table, keeps record of what migrations has
        applied and what has not.

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
            checksum TEXT,
            migrated_time TIMESTAMP DEFAULT current_timestamp
        );
    """

    (*
        Migration file naming convention:
            "V" + version (monotone increasing) + "_" + description + ".sql"
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

    type MigrationFile =
        { version: int
          name: string
          content: string }

    let allEmbeddedMigrations =
        let assembly = System.Reflection.Assembly.GetExecutingAssembly()

        let readMigration filename =
            let (version, name) = parseFileName filename
            use fileStream = assembly.GetManifestResourceStream(filename)
            use readStream = new StreamReader(fileStream)
            let content = readStream.ReadToEnd()

            { version = version
              name = name
              content = content }

        let isMigration =
            function
            | MigrationFileName _ -> true
            | _ -> false

        assembly.GetManifestResourceNames()
        |> Seq.filter isMigration
        |> Seq.map readMigration

    let migrate dbPath =
        use db = new DuckDBConnection(dbPath)
        use _ = db.BeginTransaction IsolationLevel.ReadCommitted
        db.Execute(SCHEMA) |> ignore

        let lastMigration =
            let version =
                db.Query<string>($"SELECT version FROM {TABLE_NAME} ORDER BY migrated_time DESC LIMIT 1;")

            if Seq.isEmpty version then
                0
            else
                version |> Seq.head |> int

        let currentMigration = 1

        ()

    let a = 1
