namespace Mutemaanpa;

using Godot;

/// <summary>
/// CharacterInformation shows the character stat in a in-game popup window.
/// </summary>
public partial class CharacterInformation : PanelContainer
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

    public override void _Ready()
    {
        QuitButton!.Pressed += QueueFree;
    }
}
