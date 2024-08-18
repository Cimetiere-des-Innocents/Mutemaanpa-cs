namespace Kernel

(* 
ECS:
    We use ECS architecture to process game logic.

    In ECS, game logic are separated to many many parts ("systems"). Game data is stored in
    "World" where the logic acts upon.
*)

open System
open System.Collections.Generic
open Extension

(* Basically everything can be a resource *)
type Resource = Object
type Component<'a> = Dictionary<Guid, 'a>

module World =
    type World(comps: Dictionary<Type, Resource>) =
        member _.register resource = comps.Add(resource.GetType(), resource)

        (* In compile time! *)
        member _.tryProvide<'a>() =
            comps.TryGet typeof<'a> |> Option.map (fun x -> x :?> 'a)

        member self.provide<'a>() =
            match self.tryProvide<'a> () with
            | Some res -> res
            | None -> failwith $"didn't find resource typed {typeof<'a>}"

    [<AbstractClass>]
    type Lich<'a>() =
        abstract member run: 'a -> unit

        member self.run_in(world: World) =
            let data = world.provide<'a> ()
            self.run data


[<Struct>]
[<CLIMutable>] // to make dapper work
type Vector3 = { X: float; Y: float; Z: float }

module Position =
    let a = 1
