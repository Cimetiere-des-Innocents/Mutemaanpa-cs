namespace Kernel

(* 
ECS:
    We use ECS architecture to process game logic.

    In ECS, game logic are separated to many many parts ("systems"). Game data is stored in
    "World" where the logic acts upon.

    These data is called "Resource". For one type of resource we only allow one instance. If
    you want to reuse data structures, you can use records to create new type(note that is
    not the same as type alias, it's a new type), like this:

    ```fsharp
    type Health = { Health: int}
    ```

    Here we must use record because sum types are not very compatible to C#.
*)

open System
open System.Collections.Generic
open Extension

type Entity = Guid

(* Basically everything can be a resource *)
type Resource = Object

(* Component is linked with entities, each entity has some components. *)
type Component<'a when 'a :> Resource> = Dictionary<Entity, 'a>

module World =
    type World = Dictionary<Type, Resource>

    let registerResource (world: World) resource =
        world.Add(resource.GetType(), resource)
        world

    let registerComponent<'a> (world: World) =
        world.Add(typeof<Component<'a>>, Component<'a>())
        world

    let addComponent<'a> (comp: Component<'a>) (world: World) =
        world.Add(comp.GetType(), comp)
        world


    (* In compile time! *)
    let tryGetResource<'a> (world: World) =
        world.TryGet typeof<'a> |> Option.map (fun x -> x :?> 'a)

    let tryGetComponent<'a> (world: World) = world |> tryGetResource<Component<'a>>

    let tryQuery<'a> world uuid =
        world |> tryGetComponent<'a> |> Option.bind (fun x -> x.TryGet uuid)

    let addComponentResource world uuid item =
        let comp =
            match tryGetComponent world with
            | Some comp -> comp
            | None ->
                registerComponent world |> ignore
                tryGetComponent(world).Value

        comp.Add(uuid, item)


[<Struct>]
[<CLIMutable>] // to make dapper work
type Vector3 =
    { mutable X: float
      mutable Y: float
      mutable Z: float }

[<Struct>]
[<CLIMutable>]
type Position = { Position: Vector3 }

[<Struct>]
[<CLIMutable>]
type Velocity = { Velocity: Vector3 }

[<Struct>]
[<CLIMutable>]
type CharacterStat =
    { strength: uint8
      stamina: uint8
      dexterity: uint8
      constitution: uint8
      intelligence: uint8
      wisdom: uint8 }

[<Struct>]
[<CLIMutable>]
type Name = { Name: string }

[<Struct>]
[<CLIMutable>]
type Hp = { MaxHp: float; Hp: float }

[<Struct>]
[<CLIMutable>]
type Mp = { MaxMp: int; Mp: int }

type Perk =
    | Soldier
    | Scholar

type Perks = Perks of Set<Perk>

module M8aWorld =
    open World

    let makeM8aWorld () =
        World.World()
        |> registerComponent<Position>
        |> registerComponent<Velocity>
        |> registerComponent<CharacterStat>
        |> registerComponent<Name>
        |> registerComponent<Hp>
        |> registerComponent<Mp>
        |> registerComponent<Perks>
