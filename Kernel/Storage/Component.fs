namespace Kernel.Storage

open DuckDB.NET.Data
open Dapper
open System
open Kernel
open System.Collections.Generic

(*
Position:
    This module persists position to database.
*)
module Position =
    open Extension

    let persist dbPath (comp: Component<Vector3>) =
        use db = new DuckDBConnection(dbPath)

        let persistOne (id, { X = x; Y = y; Z = z }) =
            db.Execute(
                """
                INSERT OR REPLACE INTO position(id, x, y, z)
                VALUES ($Uuid, $X, $Y, $Z);
            """,
                {| Uuid = id; X = x; Y = y; Z = z |}
            )

        comp.ToArray() |> Array.map persistOne

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
