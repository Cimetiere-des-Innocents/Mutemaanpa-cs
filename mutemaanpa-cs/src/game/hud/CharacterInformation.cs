
using Godot;

namespace Mutemaanpa;
/// <summary>
/// CharacterInformation shows the character stat in a in-game popup window.
/// </summary>
public partial class CharacterInformation : CenterContainer
{
    [Export]
    Label? CharacterName;

    [Export]
    LabelValue? HitPointDisplay;

    [Export]
    LabelValue? ManaPointDisplay;

    [Export]
    LabelValue? OriginDisplay;

    [Export]
    LabelValue? StrengthDisplay;

    [Export]
    LabelValue? StaminaDisplay;

    [Export]
    LabelValue? DexterityDisplay;

    [Export]
    LabelValue? ConstitutionDisplay;

    [Export]
    LabelValue? IntelligenceDisplay;

    [Export]
    LabelValue? WisdomDisplay;

    [Export]
    Button? QuitButton;

    public static CharacterInformation From(Character character)
    {
        var info = ResourceLoader.Load<PackedScene>("res://scene/game/hud/character_information.tscn")
            .Instantiate<CharacterInformation>();
        info.CharacterName!.Text = character.inner.Stat.Name;
        info.HitPointDisplay!.SetLabelValue("HP", $"{character.hp.hp} / {character.hp.maxHp}");
        info.OriginDisplay!.SetLabelValue("Origin", character.inner.Stat.Origin.ToString());
        info.StrengthDisplay!.SetLabelValue("Strength", character.inner.Ability.Strength.ToString());
        info.StaminaDisplay!.SetLabelValue("Stamina", character.inner.Ability.Stamina.ToString());
        info.DexterityDisplay!.SetLabelValue("Dexterity", character.inner.Ability.Stamina.ToString());
        info.ConstitutionDisplay!.SetLabelValue("Constitution", character.inner.Ability.Stamina.ToString());
        info.IntelligenceDisplay!.SetLabelValue("Intelligence", character.inner.Ability.Stamina.ToString());
        info.WisdomDisplay!.SetLabelValue("Wisdom", character.inner.Ability.Stamina.ToString());
        return info;
    }

    public override void _Ready()
    {
        QuitButton!.Pressed += QueueFree;
    }
}
