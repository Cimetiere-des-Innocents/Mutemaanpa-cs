namespace Test

open System
open Kernel.Storage
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TestClass() =

    [<TestMethod>]
    member this.TestMethodPassing() =
        let PerkPersister = Perk.persist |> Component.makePersister

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
        comp.Add(uuid, Kernel.Perks perks)
        PerkPersister source comp |> ignore
        Assert.IsTrue(true)
