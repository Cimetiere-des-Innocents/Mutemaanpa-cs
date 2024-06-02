namespace Mutemaanpa
{
    internal record struct CharacterStat {
        /// Influence attack, weight, hit points
        int Strength;
        /// Influence mana, buff duration
        int Devotion;
        /// Influence speed, damage reduction
        int Dexterity;
        /// Influence hit points, weight
        int Constitution;
        /// Influence spells, environment interaction, crafting
        int Intelligence;
        /// Influence environment interaction, dialogue, merchant
        int Wisdom;
    }
}
