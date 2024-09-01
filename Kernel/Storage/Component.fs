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

    let logSqlp template arg = logSql template (Some arg)

    let logSqlnp template = logSql template None

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
            logSqlp template param
            db.Execute(template, param)

    /// Make a persister who will persist world's component to dbPath.
    let makeRowPersister x = (makeRowSerializer x) |> makePersister

    let load<'a, 'b> (aggregator: (Guid * 'a) seq -> (Guid * 'b) seq) dbPath (stat: string) =
        use db = new DuckDBConnection(dbPath)
        let mutable dict = Dictionary()
        logSqlnp stat

        let results =
            db.Query<'a, Guid, Guid * 'a>(stat, (fun a uuid -> (uuid, a))) |> aggregator

        // Because Seq is lazy, we need manual enumeration.
        for (k, v) in results do
            dict.Add(k, v)

        dict

    let loadRow<'a> x = load<'a, 'a> id x

(*
Position:
    This module persists position to database.
*)
module Position =

    let persistRow =
        ("""
            INSERT OR REPLACE INTO position(id, x, y, z)
            VALUES ($Uuid, $X, $Y, $Z);
        """,
         fun (id: Entity, { Position = { X = x; Y = y; Z = z } }) -> {| Uuid = id; X = x; Y = y; Z = z |})


    let load = "SELECT * FROM position;"

(*
Character Stat:
    This module persists character stat to database.
*)
module CharacterStat =

    let rowPersister =
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

    let load = "SELECT * FROM character_stat;"

module Name =
    let rowPersister =
        ("""
            INSERT OR REPLACE INTO name(id, name)
            VALUES ($Uuid, $Name);
            """,
         fun (id: Entity, { Name = name }) -> {| Uuid = id; Name = name |})


    let load = "SELECT * FROM name;"

module Hp =
    let rowPersister =
        ("""
                INSERT OR REPLACE INTO hp(id, hp, max_hp)
                VALUES ($Uuid, $Hp, $MaxHp);
            """,
         fun (id: Entity, hp: Hp) ->
             {| Uuid = id
                Hp = hp.Hp
                MaxHp = hp.MaxHp |})

    let load = "SELECT * FROM hp;"

module Mp =
    let rowPersister =
        ("""
                INSERT OR REPLACE INTO mp(id, hp, max_hp)
                VALUES ($Uuid, $Mp, $MaxMp);
            """,
         fun (id: Entity, mp: Mp) ->
             {| Uuid = id
                Mp = mp.Mp
                MaxMp = mp.MaxMp |})

    let load = "SELECT * FROM mp;"

module Perk =
    let setPersister (db: DuckDBConnection) (uuid: Entity, { Perks = perks }) =
        let sql =
            """
            INSERT OR REPLACE INTO perk(id, perk)
            VALUES ($Uuid, $Perk);
        """

        perks
        |> Set.map (fun x ->
            let param = {| Uuid = uuid; Perk = x.ToString() |}
            Component.logSqlp sql param
            db.Execute(sql, param))

    let parse s =
        match s with
        | "Scholar" -> Scholar
        | "Soldier" -> Soldier
        | _ -> failwith $"unknown perk {s}"

    let load dbPath =
        Component.load
            (Seq.groupBy (fun (uuid, _) -> uuid)
             >> Seq.map (fun (uuid, entries) ->
                 let perks =
                     entries |> Seq.map (fun (_, v) -> parse v) |> (fun x -> { Perks = Set x })

                 (uuid, perks)))
            dbPath
            """SELECT * FROM perk;"""


module Persistance =

    let persist: World * string -> unit =

        /// f: dbPath -> Component -> 'a
        /// side effect is to save component to dbPath
        ///
        /// World persister is a function who save some aspect of the world to dbPath
        let makeWorldPersister f (world: World, dbPath) =
            world |> tryGetComponent<'a> |> Option.map (f dbPath) |> ignore

            (world, dbPath)

        let usePersister x =
            x |> Component.makePersister |> makeWorldPersister

        /// transforms a single row sql mapper to a chained persister
        let useRowPersister (persistSqlMapper: string * (Entity * 'Item -> 'b)) =
            persistSqlMapper |> Component.makeRowSerializer |> usePersister

        useRowPersister Position.persistRow
        >> useRowPersister CharacterStat.rowPersister
        >> useRowPersister Name.rowPersister
        >> useRowPersister Hp.rowPersister
        >> useRowPersister Mp.rowPersister
        >> (usePersister Perk.setPersister)
        >> ignore

    let load dbPath =
        let world = World()

        world
        |> addComponent (Component.loadRow<Position> dbPath Position.load)
        |> addComponent (Component.loadRow<CharacterStat> dbPath CharacterStat.load)
        |> addComponent (Component.loadRow<Name> dbPath Name.load)
        |> addComponent (Component.loadRow<Hp> dbPath Hp.load)
        |> addComponent (Component.loadRow<Mp> dbPath Mp.load)
        |> addComponent (Perk.load dbPath)
