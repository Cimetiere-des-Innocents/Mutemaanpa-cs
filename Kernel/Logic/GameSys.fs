namespace Kernel

(*
Game system:
    Execute functions in world.
*)

type Res<'a when 'a: (new: unit -> 'a)>(world: World) =
    member _.value () = World.getOrCreateResource<'a> world

    member _.change(a: 'a) = world[typeof<'a>] <- a

module GameSys =

    let makeSystemRes (f: Res<'a> -> unit) =
        fun (world: World) ->
            let arg = Res<'a>(world)
            f arg
