namespace Kernel

type Game = { Catalog: Catalog }

(*
Game:
    This module wires up everything from Logic and Storage
*)
module Game =
    let makeGame dbPath = { Catalog = Catalog(dbPath) }
