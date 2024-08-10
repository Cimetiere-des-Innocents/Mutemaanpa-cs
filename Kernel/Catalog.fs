namespace Kernel

open System

[<Struct>]
[<CLIMutable>] // to make dapper work
type SaveData =
    { Id: Guid
      CreatedAt: DateTime
      LastPlayed: DateTime }

module Catalog =
    open DuckDB.NET.Data
    open Dapper

    let SCHEMA =
        """
        CREATE TABLE IF NOT EXISTS save_slot (
            id UUID PRIMARY KEY,
            created_at TIMESTAMP DEFAULT current_timestamp,
            last_played TIMESTAMP DEFAULT current_timestamp
        );
    """

    let init dbPath =
        use db = new DuckDBConnection(dbPath)
        db.Execute(SCHEMA)

    let makeSave dbPath =
        let newUuid = Guid.NewGuid()
        use db = new DuckDBConnection(dbPath)

        db.Execute(
            """
            INSERT INTO save_slot (id) VALUES ($id)
            """,

            {| id = newUuid |}
        )
        |> ignore

        newUuid

    let querySaves dbPath =
        use db = new DuckDBConnection(dbPath)

        db.Query<SaveData>(
            """
        SELECT * FROM save_slot;
        """
        )

    let hasSave = querySaves >> Seq.isEmpty >> not

    let remove dbPath uuid =
        use db = new DuckDBConnection(dbPath)

        db.Execute(
            """
            DELETE FROM save_slot WHERE id = $id
            """,
            {| id = uuid |}
        )

(* For C# *)
open Catalog

type Catalog(dbPath: string) =
    member _.init() = init dbPath
    member _.makeSave() = makeSave dbPath
    member _.querySaves() = querySaves dbPath
    member _.hasSave() = hasSave dbPath
    member _.remove id = remove dbPath id
