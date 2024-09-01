namespace Test

open Kernel
open Kernel.Extension;
open Microsoft.VisualStudio.TestTools.UnitTesting
open System

[<TestClass>]
type TestSystem() =

    [<TestMethod>]
    member _.TestSys() =
        let world = World()
        let intLs = [| 2 |]
        World.addResource world intLs |> ignore

        let counterSys (world: World) =
            let resource = World.tryGetResource<int array> world
            let resource = resource.Value
            resource[0] <- 3
            ()

        let scheduler = Scheduler.newScheduler ()
        scheduler.add counterSys
        scheduler.run world
        let xxx = World.tryGetResource<int array> world
        Assert.AreEqual (xxx.Value[0], 3)

    [<TestMethod>]
    member _.TestRes() =
        let world = World()

        let counterSys (res: Res<Hp>) =
            let mutable hp = res.value()
            hp.Hp <- 2.0
            res.change hp
            ()

        let scheduler = Scheduler.newScheduler ()
        scheduler.add counterSys
        scheduler.run world
        let xxx = World.tryGetResource<Hp> world
        Assert.AreEqual (xxx.Value.Hp, 2.0)


