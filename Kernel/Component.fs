namespace Kernel

open System

[<Struct>]
[<CLIMutable>] // to make dapper work
type Vector3 = { X: float; Y: float; Z: float }

module Position =
    open DuckDB.NET.Data
    open Dapper
    open System.Collections.Generic

    let SCHEMA =
        """
        CREATE TABLE IF NOT EXISTS position (
            id UUID PRIMARY KEY,
            x FLOAT,
            y FLOAT,
            z FLOAT,
        );
    """

    let initDb dbPath =
        use db = new DuckDBConnection(dbPath)
        db.Execute(SCHEMA)

    let persist dbPath ([<ParamArray>] arg: (Guid * Vector3)[]) =
        use db = new DuckDBConnection(dbPath)

        let persistOne (id, { X = x; Y = y; Z = z }) =
            db.Execute(
                """
                INSERT OR REPLACE INTO position(id, x, y, z)
                VALUES ($Uuid, $X, $Y, $Z);
            """,
                {| Uuid = id; X = x; Y = y; Z = z |}
            )

        arg |> Array.map persistOne

    let load dbPath =
        use db = new DuckDBConnection(dbPath)

        let mutable dict = Dictionary()

        db.Query<Guid, float, float, float, Guid * Vector3>(
            "SELECT * FROM position;",
            fun uuid x y z -> (uuid, { X = x; Y = y; Z = z })
        )
        |> Seq.map (fun (u, v) -> dict.Add(u, v))
        |> ignore

        dict

    open Extension

    type Store(DbPath: string) =
        let mutable uuidToPosition = load DbPath
        member _.lookup = uuidToPosition.TryGet
        member _.add = uuidToPosition.Add
        member _.remove = uuidToPosition.Remove
        member _.persist = uuidToPosition.ToArray >> persist DbPath

