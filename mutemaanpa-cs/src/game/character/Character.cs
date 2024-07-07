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
    int MaxManaPoint,
    float Speed
);

public record struct CharacterState
(
    CharacterData Data,
    CharacterRuntime Runtime
);

public abstract class ICharacter(CharacterState state)
{
    public CharacterState state = state;
    public abstract (ICharacter, Vector3) Move(Vector3 input, float delta);
    public virtual void OnEnterState() { }
    public virtual void OnLeaveState() { }
    internal abstract (ICharacter, float) Hit(float damage);
}

class ALiveCharacter(CharacterState state) : ICharacter(state)
{
    public override (ICharacter, Vector3) Move(Vector3 input, float delta)
    {
        var deltaPosition = input.Normalized() * state.Runtime.Speed * delta;
        var newPosition = state.Data.Position!.Value + deltaPosition;
        return (new ALiveCharacter(state with
        {
            Data = state.Data with { Position = newPosition }
        }), newPosition);
    }

    internal override (ICharacter, float) Hit(float damage)
    {
        Func<CharacterState, ICharacter> transit = (CharacterState state) => new ALiveCharacter(state);
        if (damage > state.Data.Stat.Hp)
        {
            damage = state.Data.Stat.Hp;
            transit = (CharacterState state) => new DeadCharacter(state);
        }
        var newHp = state.Data.Stat.Hp - damage;
        var newState = state with
        {
            Data = state.Data with
            {
                Stat = state.Data.Stat with { Hp = newHp }
            }
        };
        return (transit(newState), newHp);
    }
}

class DeadCharacter(CharacterState state) : ICharacter(state)
{
    public override (ICharacter, Vector3) Move(Vector3 input, float delta)
    {
        return (this, Vector3.Zero);
    }

    internal override (ICharacter, float) Hit(float damage)
    {
        return (this, 0);
    }
}

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

    public Action Hit(float damage)
    {
        (state, damage) = state.Hit(damage);
        EventBus.Publish(new HitEvent(state.state.Data.Stat.Name, damage));
        return NoAction;
    }

    public Vector3 Move(Vector3 input, float delta)
    {
        (state, var position) = state.Move(input, delta);
        return position;
    }
}
