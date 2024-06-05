using Godot;

namespace Mutemaanpa
{
    public class CharacterAbility
    {
        /// Influence attack, weight, hit points
        public int Strength { get; set; }
        /// Influence mana, buff duration
        public int Stamina { get; set; }
        /// Influence speed, damage reduction
        public int Dexterity { get; set; }
        /// Influence hit points, weight
        public int Constitution { get; set; }
        /// Influence spells, environment interaction, crafting
        public int Intelligence { get; set; }
        /// Influence environment interaction, dialogue, merchant
        public int Wisdom { get; set; }
    }

    /// <summary>
    /// basic information about the character
    /// </summary>
    public class CharacterStat
    {
        public required string Name {get; set;}
        public int HitPoint { get; set; }
        public int ManaPoint { get; set; }
        public Vector3 Position { get; set; }

        /// <summary>
        /// This field can be null, because NPC characters don't have this field set.
        /// </summary>
        /// <value></value>
        public string? Player {get; set;}
    }

    /// <summary>
    /// Persistent data of the character.
    /// </summary>
    public class CharacterState
    {
        public required CharacterAbility Ability;
        public required CharacterStat Stat;
    }

    public class CharacterRuntime
    {
        public int MaxHitPoint {get; set;}
        public int MaxManaPoint {get; set;}

    }

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
            stat.Position += Vector3.Right * dx;
            stat.Position += Vector3.Up * dy;
            return stat.Position;
        }
    }

    class DeadCharacter : ICharacter
    {
        public override Vector3 Move(int dx, int dy)
        {
            // No op
            return stat.Position;
        }
    }

}
