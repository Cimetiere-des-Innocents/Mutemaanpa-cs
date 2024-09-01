namespace Kernel

module M8aWorld =

    let bootstrap uuid world =
        Session.newSession uuid world
        |> Command.addCommandPlugin
        |> Character.addCharacterPlugin
