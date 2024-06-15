namespace Mutemaanpa;

using Godot;

public partial class SetAbility : MarginContainer
{
    [Export]
    private NumberBar? Strength;

    [Export]
    private NumberBar? Stamina;

    [Export]
    private NumberBar? Dexterity;

    [Export]
    private NumberBar? Constitution;

    [Export]
    private NumberBar? Intelligence;

    [Export]
    private NumberBar? Wisdom;

    [Export]
    public Button? FinishButton;

    public CharacterAbility GetAbility() => new(
        Strength!.Value,
        Stamina!.Value,
        Dexterity!.Value,
        Constitution!.Value,
        Intelligence!.Value,
        Wisdom!.Value
    );

}
