namespace Kernel

type Game = { Catalog: Catalog }

(*
Game:
    This module wires up everything from Logic and Storage.
*)
module Game =
    open Logger

    let makeGame dbPath logger =
        setLogger logger
        info "starting game"
        { Catalog = Catalog(dbPath) }
