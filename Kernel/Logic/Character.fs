namespace Kernel

[<Struct>]
[<CLIMutable>] // to make dapper work
type Vector3 =
    { mutable X: float
      mutable Y: float
      mutable Z: float }

(*
Position:
    This position is the global position in world.
*)
[<Struct>]
[<CLIMutable>]
type Position = { Position: Vector3 }


(*
Velocity: not the current velocity, it is the potential.
    
    For character, base velocity:
        1.0
        + number of dexterity
*)
[<Struct>]
[<CLIMutable>]
type Velocity = { Velocity: Vector3 }


(*
Character Stat:
    These defines the basic capability of a character. Nearly all their in-game performance are
    determined by these numbers.
*)
[<Struct>]
[<CLIMutable>]
type CharacterStat =
    { strength: uint8
      stamina: uint8
      dexterity: uint8
      constitution: uint8
      intelligence: uint8
      wisdom: uint8 }

(* For everyone that has a name. *)
[<Struct>]
[<CLIMutable>]
type Name = { Name: string }

(* If health decreased to zero, normally the character die.
    TODO: consider if we left a corpse, or just remove the character?

    Formula: 10 + constitution
 *)
[<Struct>]
[<CLIMutable>]
type Hp = { MaxHp: float; Hp: float }

(* Perk. Used in dialogue system. *)
type Perk =
    | Soldier
    | Scholar

type Perks = { Perks: Set<Perk> }

module Velocity =
    let speed (characterStat: CharacterStat) = 1.0 + (float) characterStat.dexterity

module Hp =
    let maxHpOf (stat: CharacterStat) = 10.0 + (float) stat.constitution

    let hpOf (characterStat: CharacterStat) =
        let maxHp = maxHpOf characterStat
        { MaxHp = maxHp; Hp = maxHp }

module Character =
    type CharacterBundle =
        { Position: Position
          Velocity: Velocity
          CharacterStat: CharacterStat
          Name: Name
          Hp: Hp
          Perks: Perks }

    let spawn (session: Session) (bundle: CharacterBundle) =
        session.spawn
            [ bundle.Position :> Resource
              bundle.Velocity
              bundle.CharacterStat
              bundle.Name
              bundle.Hp
              bundle.Perks ]

    let create (session: Session) (stat: CharacterStat) spawnPoint name =
        let velocity =
            let speed = Velocity.speed stat
            { Velocity = { X = speed; Y = speed; Z = speed } }

        let hp = Hp.hpOf stat
        let perks = { Perks = Set.empty }

        spawn
            session
            { Position = spawnPoint
              Velocity = velocity
              CharacterStat = stat
              Name = name
              Hp = hp
              Perks = perks }
