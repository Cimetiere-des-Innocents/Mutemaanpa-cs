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

    let toDbPath file = $"Data Source={file}"
    let uuidToDbPath uuid = uuid |> Catalog.toSaveName |> toDbPath
    member _.catalog = catalog

    member self.newSession() =
        let uuid = catalog.makeSave ()
        self.loadSession uuid

    member _.loadSession uuid =
        let dbPath = uuidToDbPath uuid
        Migration.migrate dbPath
        let session = Storage.Persistance.load dbPath |> M8aWorld.bootstrap uuid
        gameState <- InSession session
        session

    member _.saveSession() =
        match gameState with
        | InSession session -> Storage.Persistance.persist (session.world, uuidToDbPath session.id)
        | OutSession -> failwith "You cannot save the game when you are not in game."

    member self.quitSession saveWhenQuit =
        if saveWhenQuit then self.saveSession () else ()
        gameState <- OutSession

    member _.remove uuid =
        catalog.remove uuid |> ignore
        System.IO.File.Delete(Catalog.toSaveName uuid)

module Game =
    let makeGame dbPath logger =
        setLogger logger
        info "starting game"
        Game(Catalog(dbPath), OutSession)
