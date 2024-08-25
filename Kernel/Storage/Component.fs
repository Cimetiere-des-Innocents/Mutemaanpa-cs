namespace Kernel.Storage

open DuckDB.NET.Data
open Dapper
open System
open Kernel
open Extension
open System.Collections.Generic
open World

(*
Component:
    This module provides various helper utilities for us to simplify database operations.
*)
module Component =
    open Logger

    let logSql template arg =
        info
            $"""Executed SQL {template}{match arg with
                                        | None -> ""
                                        | Some args -> $" with {args}\n"}"""

    ///
    /// persist save's `comp` to `dbPath` using `serializer`
    ///
    /// ## Example
    ///
    /// ```fsharp
    /// let serializer db x =
    ///     db.Execute("INSERT INTO tab(id) VALUES $Id;", x)
    /// let persistAll = persist serializer dbPath health
    /// ```
    ///
    /// Persister: function who save Component to dbPath
    /// Serializer: function who save Component & Entity to a db
    ///
    /// Mapper: function who turn game object to sql store procedures
    ///
    let makePersister serializer dbPath (comp: Component<'ComponentItem>) =
        use db = new DuckDBConnection(dbPath)
        comp.ToArray() |> Array.map (fun x -> serializer db x)

    /// Make a row serializer based on sql and a sql mapper
    let makeRowSerializer (sql: string * (Entity * 'ComponentItem -> 'SqlParams)) =
        fun (db: DuckDBConnection) x ->
            let (template, mapper) = sql
            let param = mapper x
            logSql template (Some param)
            db.Execute(template, param)

    /// Make a persister who will persist world's component to dbPath.
    let makeRowPersister x = (makeRowSerializer x) |> makePersister

    let load<'a, 'b> dbPath (args: string * ('a -> 'b)) =
        use db = new DuckDBConnection(dbPath)
        let mutable dict = Dictionary()
        let (stat, mapper) = args
        logSql stat None

        db.Query<Guid, 'a, Guid * 'b>(stat, (fun guid a -> (guid, mapper a)))
        |> Seq.map (fun (u, v) -> dict.Add(u, v))

(*
Position:
    This module persists position to database.
*)
module Position =

    let persistOne =
        ("""
            INSERT OR REPLACE INTO position(id, x, y, z)
            VALUES ($Uuid, $X, $Y, $Z);
        """,
         fun (id: Entity, Position { X = x; Y = y; Z = z }) -> {| Uuid = id; X = x; Y = y; Z = z |})


    let load = ("SELECT * FROM position;", (fun x y z -> { X = x; Y = y; Z = z }))

(*
Character Stat:
    This module persists character stat to database.
*)
module CharacterStat =

    let persistOne =
        ("""
            INSERT OR REPLACE INTO character_stat(id, strength, stamina, dexterity, constitution, intelligence, wisdom)
            VALUES ($Uuid, $Strength, $Stamina, $Dexterity, $Constitution, $Intelligence, $Wisdom);
        """,
         fun
             (id: Entity,
              { strength = strength
                stamina = stamina
                dexterity = dexterity
                constitution = constitution
                intelligence = intelligence
                wisdom = wisdom }) ->
             {| Uuid = id
                Strength = strength
                Stamina = stamina
                Dexterity = dexterity
                Constitution = constitution
                Intelligence = intelligence
                Wisdom = wisdom |})

module Name =
    let persistOne =
        ("""
            INSERT OR REPLACE INTO name(id, name)
            VALUES ($Uuid, $Name);
            """,
         fun (id: Entity, Name name) -> {| Uuid = id; Name = name |})

module Hp =
    let persistOne =
        ("""
                INSERT OR REPLACE INTO hp(id, hp, max_hp)
                VALUES ($Uuid, $Hp, $MaxHp);
            """,
         fun (id: Entity, hp: Hp) ->
             {| Uuid = id
                Hp = hp.Hp
                MaxHp = hp.MaxHp |})

module Mp =
    let persistOne =
        ("""
                INSERT OR REPLACE INTO mp(id, hp, max_hp)
                VALUES ($Uuid, $Mp, $MaxMp);
            """,
         fun (id: Entity, mp: Mp) ->
             {| Uuid = id
                Mp = mp.Mp
                MaxMp = mp.MaxMp |})

module Perk =
    let persist (db: DuckDBConnection) (uuid: Entity, Perks perks) =
        let sql =
            """
            INSERT OR REPLACE INTO perk(id, perk)
            VALUES ($Uuid, $Perk);
        """

        perks
        |> Set.map (fun x ->
            let param = {| Uuid = uuid; Perk = x.ToString() |}
            Component.logSql sql (Some param)
            db.Execute(sql, param))

module Persistance =

    let persist: World * string -> unit =

        /// f: dbPath -> Component -> 'a
        /// side effect is to save component to dbPath
        ///
        /// World persister is a function who save some aspect of the world to dbPath
        let makeWorldPersister f (world: World, dbPath) =
            world |> tryGetComponent<'a> |> Option.map (f dbPath) |> ignore

            (world, dbPath)

        let persist x =
            x |> Component.makePersister |> makeWorldPersister

        /// transforms a single row sql mapper to a chained persister
        let persistRow (persistSqlMapper: string * (Entity * 'Item -> 'b)) =
            persistSqlMapper |> Component.makeRowSerializer |> persist

        persistRow Position.persistOne
        >> persistRow CharacterStat.persistOne
        >> persistRow Name.persistOne
        >> persistRow Hp.persistOne
        >> persistRow Mp.persistOne
        >> (Perk.persist |> persist)
        >> ignore
