namespace Kernel

open Logger

type GameState =
    | InSession of Session
    | OutSession

(*
Game:
    This module wires up everything from Logic and Storage.
*)
type Game(catalog: Catalog, gameState: GameState) =
    let mutable gameState = gameState
    member _.catalog = catalog

    member self.newSession() =
        let uuid = catalog.makeSave ()
        self.loadSession uuid

    member _.loadSession uuid =
        let saveFile = Catalog.toSaveName uuid
        Migration.migrate saveFile
        let session = Storage.Persistance.load saveFile |> Session.bootstrap uuid
        gameState <- InSession session
        session

    member _.saveSession() =
        match gameState with
        | InSession session -> Storage.Persistance.persist (session.world, Catalog.toSaveName session.id)
        | OutSession -> failwith "You cannot save the game when you are not in game."

    member self.quitSession saveWhenQuit =
        if saveWhenQuit then self.saveSession () else ()
        gameState <- OutSession

module Game =
    let makeGame dbPath logger =
        setLogger logger
        info "starting game"
        Game(Catalog(dbPath), OutSession)
