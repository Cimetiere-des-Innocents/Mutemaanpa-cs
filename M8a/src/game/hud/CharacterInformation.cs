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
        info.CharacterName!.Text = state.Data.Stat.Name;
        info.HitPointDisplay!.SetLabelValue("HP", $"{state.Data.Stat.Hp} / {state.Runtime.MaxHitPoint}");
        info.ManaPointDisplay!.SetLabelValue("MP", $"{state.Data.Stat.Mp} / {state.Runtime.MaxManaPoint}");
        info.OriginDisplay!.SetLabelValue("Origin", state.Data.Stat.Origin.ToString());
        info.StrengthDisplay!.SetLabelValue("Strength", state.Data.Ability.Strength.ToString());
        info.StaminaDisplay!.SetLabelValue("Stamina", state.Data.Ability.Stamina.ToString());
        info.DexterityDisplay!.SetLabelValue("Dexterity", state.Data.Ability.Stamina.ToString());
        info.ConstitutionDisplay!.SetLabelValue("Constitution", state.Data.Ability.Stamina.ToString());
        info.IntelligenceDisplay!.SetLabelValue("Intelligence", state.Data.Ability.Stamina.ToString());
        info.WisdomDisplay!.SetLabelValue("Wisdom", state.Data.Ability.Stamina.ToString());
        return info;
    }
 
    public override void _Ready()
    {
        QuitButton!.Pressed += QueueFree;
    }
}
