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

    public Kernel.CharacterStat GetAbility()
    {
        return new(
         strength: Strength!.Value,
         stamina: Stamina!.Value,
         dexterity: Dexterity!.Value,
         constitution: Constitution!.Value,
         intelligence: Intelligence!.Value,
         wisdom: Wisdom!.Value
        );
    }

}
