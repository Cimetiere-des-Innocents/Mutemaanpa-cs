namespace Mutemaanpa;

using Godot;

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

    public static CharacterInformation From(CharacterState state)
    {
        var info = ResourceLoader.Load<PackedScene>("res://scene/game/hud/character_information.tscn")
            .Instantiate<CharacterInformation>();
        info.CharacterName!.Text = state.CharacterData.Stat.Name;
        info.HitPointDisplay!.SetLabelValue("HP", $"{state.CharacterData.Stat.Hp} / {state.CharacterRuntime.MaxHitPoint}");
        info.ManaPointDisplay!.SetLabelValue("MP", $"{state.CharacterData.Stat.Mp} / {state.CharacterRuntime.MaxManaPoint}");
        info.OriginDisplay!.SetLabelValue("Origin", state.CharacterData.Stat.Origin.ToString());
        info.StrengthDisplay!.SetLabelValue("Strength", state.CharacterData.Ability.Strength.ToString());
        info.StaminaDisplay!.SetLabelValue("Stamina", state.CharacterData.Ability.Stamina.ToString());
        info.DexterityDisplay!.SetLabelValue("Dexterity", state.CharacterData.Ability.Stamina.ToString());
        info.ConstitutionDisplay!.SetLabelValue("Constitution", state.CharacterData.Ability.Stamina.ToString());
        info.IntelligenceDisplay!.SetLabelValue("Intelligence", state.CharacterData.Ability.Stamina.ToString());
        info.WisdomDisplay!.SetLabelValue("Wisdom", state.CharacterData.Ability.Stamina.ToString());
        return info;
    }

    public override void _Ready()
    {
        QuitButton!.Pressed += QueueFree;
    }
}
