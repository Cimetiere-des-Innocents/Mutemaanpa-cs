namespace Kernel.Storage

open DuckDB.NET.Data
open Dapper
open System
open Kernel
open Extension
open System.Collections.Generic

module Component =

    let persist dbPath (sql: Entity * 'a -> string * 'b) (comp: Component<'a>) =
        use db = new DuckDBConnection(dbPath)

        comp.ToArray()
        |> Array.map (fun x ->
            let (stat, param) = sql x
            db.Execute(stat, param))

    let load<'a, 'b> dbPath (args: string * ('a -> 'b)) =
        use db = new DuckDBConnection(dbPath)
        let mutable dict = Dictionary()
        let (stat, mapper) = args

        db.Query<Guid, 'a, Guid * 'b>(stat, (fun guid a -> (guid, mapper a)))
        |> Seq.map (fun (u, v) -> dict.Add(u, v))

(*
Position:
    This module persists position to database.
*)
module Position =

    let persistOne (id: Entity, { X = x; Y = y; Z = z }) =
        ("""
            INSERT OR REPLACE INTO position(id, x, y, z)
            VALUES ($Uuid, $X, $Y, $Z);
        """,
         {| Uuid = id; X = x; Y = y; Z = z |})


    let load = ("SELECT * FROM position;", (fun x y z -> { X = x; Y = y; Z = z }))


module name =
    let persist dbPath (comp: Component<Name>) =
        use db = new DuckDBConnection(dbPath)

        ()
