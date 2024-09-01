namespace Kernel

open System.Collections.Generic

(* ECS loop: tick -> handle input -> update time -> update view(in godot) *)

type BuiltinCommand = NewCharacter of CharacterStat * Name

[<CLIMutable>]
type Command = { queue: Queue<BuiltinCommand> }

module Command =
    let resolveBuiltinCommand world builtin =
        match builtin with
        | NewCharacter(stat, name) -> Character.create world stat name

    let rec resolveCommand (world: World) =
        let commands = World.getOrCreateResource<Command> world
        let (ok, v) = commands.queue.TryDequeue()

        if ok then
            (resolveBuiltinCommand world v) |> ignore
            resolveCommand world

    let createUser world stat name =
        let commands = World.tryGetResource<Command> world

        match commands with
        | None -> failwith "No command pipe registered"
        | Some command -> command.queue.Enqueue(NewCharacter(stat, name))

    let addCommandPlugin (session: Session) =
        let command = { queue = Queue() }
        session.addResource(command).addSystem (resolveCommand)
