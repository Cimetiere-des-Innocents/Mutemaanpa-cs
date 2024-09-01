namespace Test

open System
open Kernel.Storage
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TestClass() =

    [<TestMethod>]
    member _.TestMethodPassing() =
        let PerkPersister = Perk.setPersister |> Component.makePersister

        let source = "Data Source=test.db"

        Kernel.Migration.migrate source

        let perks =
            Set(
                seq {
                    Kernel.Scholar
                    Kernel.Soldier
                }
            )

        let comp = Kernel.Component()
        let uuid = Guid.NewGuid()
        comp.Add(uuid, { Kernel.Perks = perks })
        PerkPersister source comp |> ignore
        let world = Persistance.load source

        let ans = Kernel.World.tryQuery<Kernel.Perks> world uuid
        Assert.AreEqual(ans, Some({ Kernel.Perks = perks }))
