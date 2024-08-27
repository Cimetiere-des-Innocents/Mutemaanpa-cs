namespace Kernel

open System

type Session =
    { world: World.World
      id: Guid }

    member public self.spawn([<ParamArray>] arr: Resource array) =
        let uuid = Guid.NewGuid()
        arr |> Array.map (World.addComponentResource self.world uuid) |> ignore
        uuid

module Session =
    let bootstrap uuid world = { world = world; id = uuid }
