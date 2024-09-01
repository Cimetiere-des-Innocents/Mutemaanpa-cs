namespace Kernel

open System

type Session =
    { world: World
      id: Guid
      scheduler: Scheduler }

    member public self.addSystem sys =
        self.scheduler.add sys
        self

    member public self.addComponent<'a> comp =
        World.addComponent comp |> ignore
        self

    member public self.registerComponent<'a>() =
        World.registerComponent self.world |> ignore
        self

    member public self.addResource<'a> res =
        World.addResource self.world res |> ignore
        self

    member public self.registerResource<'a when 'a: (new: unit -> 'a)>() =
        World.registerResource<'a> self.world |> ignore
        self

    member public self.update () =
        self.scheduler.run self.world


module Session =
    let newSession uuid world =
        let mainScheduler = Scheduler.newScheduler ()

        { world = world
          id = uuid
          scheduler = mainScheduler }
