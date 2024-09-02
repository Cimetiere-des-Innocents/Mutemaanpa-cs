namespace Mutemaanpa;
using System;
using Godot;

public class CharacterAbility
{
    /// Influence attack, weight, hit points
    public required int Strength;
    /// Influence mana, buff duration
    public required int Stamina;
    /// Influence speed, damage reduction
    public required int Dexterity;
    /// Influence hit points, weight
    public required int Constitution;
    /// Influence spells, environment interaction, crafting
    public required int Intelligence;
    /// Influence environment interaction, dialogue, merchant
    public required int Wisdom;
}

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
public class CharacterStat
{
    public required string Name;
    public required Origin Origin;
}

/// <summary>
/// Persistent data of the character.
/// </summary>
public class CharacterInner
{
    public required CharacterAbility Ability;
    public required CharacterStat Stat;
    public required Guid Uuid;
}

public class Hp
{
    public required double hp;
    public required double maxHp;
}

/// <summary>
/// Controller class for characters
/// </summary>
/// <param name="state"></param> <summary>
/// The state of a character.
/// </summary>
public class Character
{
    public required CharacterInner inner;
    public required Hp hp;
    public static Character NewCharacter(CharacterAbility ability, CharacterStat stat)
    {
        var Hp = new Hp()
        {
            hp = 1.0f * ability.Constitution,
            maxHp = 1.0f * ability.Constitution
        };
        var uuid = Guid.NewGuid();
        var inner = new CharacterInner()
        {
            Ability = ability,
            Stat = stat,
            Uuid = uuid
        };
        return new Character()
        {
            inner = inner,
            hp = Hp
        };
    }

    internal Vector3 GetVelocity(Vector3 input)
    {
        return inner.Ability.Dexterity * input;
    }

    internal void Hit(float v)
    {
        hp.hp -= v;
    }
}
