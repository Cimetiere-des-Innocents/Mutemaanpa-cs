namespace Kernel

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
