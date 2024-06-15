namespace Mutemaanpa;
using System;
using Godot;

public record struct CharacterAbility
(
    /// Influence attack, weight, hit points
    int Strength,
    /// Influence mana, buff duration
    int Stamina,
    /// Influence speed, damage reduction
    int Dexterity,
    /// Influence hit points, weight
    int Constitution,
    /// Influence spells, environment interaction, crafting
    int Intelligence,
    /// Influence environment interaction, dialogue, merchant
    int Wisdom
);

public enum Origin
{
    SOLDIER,
    CLERIC,
    ROGUE,
    HUNTER,
    BUREAUCRAT,
    SPY,
    NAMELESS_ONE
}

/// <summary>
/// basic information about the character
/// </summary>
public record struct CharacterStat
(
    string Name,
    float Hp,
    int Mp,
    Origin Origin
);

/// <summary>
/// Persistent data of the character.
/// </summary>
public record struct CharacterData
(
    CharacterAbility Ability,
    CharacterStat Stat,
    Guid Uuid,

    /// A character may not have any positions(daemon characters) in some
    /// special circumstances.
    Vector3? Position,

    /// <summary>
    /// This field can be null, because NPC characters don't have this field set.
    /// </summary>
    /// <value></value>
    Guid? Player
);

/// <summary>
/// Derived and calculated properties of Character. It is maintained when
/// game is running. We don't put them on disk in saving process.
/// </summary>
public record struct CharacterRuntime
(
    int MaxHitPoint,
    int MaxManaPoint
);

public record struct CharacterState
(
    CharacterData CharacterData,
    CharacterRuntime CharacterRuntime
);

abstract class ICharacter
{
    public required string name;
    public required CharacterStat stat;
    public required CharacterAbility ability;
    public abstract Vector3 Move(int dx, int dy);
    public virtual void OnEnterState() { }
    public virtual void OnLeaveState() { }
}

class ALiveCharacter : ICharacter
{
    public override Vector3 Move(int dx, int dy)
    {
        return Vector3.Up;
    }
}

class DeadCharacter : ICharacter
{
    public override Vector3 Move(int dx, int dy)
    {
        // No op
        return Vector3.Up;
    }
}


