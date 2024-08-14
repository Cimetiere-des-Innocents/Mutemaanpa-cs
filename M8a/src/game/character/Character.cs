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
    public required float Hp;
    public required int Mp;
    public required Origin Origin;
}

/// <summary>
/// Persistent data of the character.
/// </summary>
public class CharacterData
{
    public required CharacterAbility Ability;
    public required CharacterStat Stat;
    public required Guid Uuid;

    /// A character may not have any positions(daemon characters) in some
    /// special circumstances.
    public required Vector3? Position;

    /// <summary>
    /// This field can be null, because NPC characters don't have this field set.
    /// </summary>  /// <value></value>
    public required Guid? Player;
}

/// <summary>
/// Derived and calculated properties of Character. It is maintained when
/// game is running. We don't put them on disk in saving process.
/// </summary>
public class CharacterRuntime
{
    public required int MaxHitPoint;
    public required int MaxManaPoint;
    public required float Speed;
}

public class CharacterState
{
    public required CharacterData Data;
    public required CharacterRuntime Runtime;
}

public abstract class ICharacter(CharacterState state)
{
    public CharacterState state = state;
    public abstract Vector3 GetVelocity(Vector3 input);
    /// <summary>
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>(newState, damage)</returns>
    internal abstract (ICharacter, float) Hit(float damage);
    /// <summary>
    /// </summary>
    /// <param name="newPosition"></param>
    /// <returns>(newState, newPosition)</returns>
    internal abstract (ICharacter, Vector3) Move(Vector3 newPosition);
}

class ALiveCharacter(CharacterState state) : ICharacter(state)
{
    public override Vector3 GetVelocity(Vector3 input) => input.Normalized() * state.Runtime.Speed;

    internal override (ICharacter, float) Hit(float damage)
    {
        Func<CharacterState, ICharacter> transit = (CharacterState state) => new ALiveCharacter(state);
        if (damage >= state.Data.Stat.Hp)
        {
            damage = state.Data.Stat.Hp;
            transit = (CharacterState state) =>
            {
                EventBus.Publish(new DeadEvent(state));
                return new DeadCharacter(state);
            };
        }
        state.Data.Stat.Hp -= damage;
        return (transit(state), damage);
    }

    internal override (ICharacter, Vector3) Move(Vector3 newPosition)
    {
        state.Data.Position = newPosition;
        return (new ALiveCharacter(state), newPosition);
    }
}

class DeadCharacter(CharacterState state) : ICharacter(state)
{
    public override Vector3 GetVelocity(Vector3 input) => Vector3.Zero;

    internal override (ICharacter, float) Hit(float damage)
    {
        return (this, 0);
    }

    internal override (ICharacter, Vector3) Move(Vector3 newPosition)
    {
        return (this, state.Data.Position!.Value);
    }
}

/// <summary>
/// Controller class for characters
/// </summary>
/// <param name="state"></param> <summary>
/// The state of a character.
/// </summary>
public partial class Character(ICharacter state)
{
    static readonly Action NoAction = () => { };

    public Character(CharacterState characterState) : this(
        characterState.Data.Stat.Hp switch
        {
            0 => new DeadCharacter(characterState),
            > 0 => new ALiveCharacter(characterState),
            _ => throw new Exception("we don't have negative health players")
        })
    { }

    public CharacterData Dump() => state.state.Data;

    public CharacterState State() => state.state;

    /// <summary>
    /// Called when the player is hit by another entity.
    /// </summary>
    /// <param name="damage">The net damage</param>
    /// <returns>Reaction to the damage</returns>
    public Action Hit(float damage)
    {
        (state, damage) = state.Hit(damage);
        EventBus.Publish(new HitEvent(state.state.Data.Stat.Name, damage));
        return NoAction;
    }

    public Vector3 GetVelocity(Vector3 input) => state.GetVelocity(input);

    public Vector3 Move(Vector3 newPosition)
    {
        (state, newPosition) = state.Move(newPosition);
        return newPosition;
    }

    public bool Dead
    {
        get => state is DeadCharacter;
    }
}

public class CharacterModel
{
}
