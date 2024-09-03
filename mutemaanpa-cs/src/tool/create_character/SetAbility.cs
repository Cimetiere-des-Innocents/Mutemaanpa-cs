
using Godot;

namespace Mutemaanpa;
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

    public CharacterAbility GetAbility() => new()
    {
        Strength = Strength!.Value,
        Stamina = Stamina!.Value,
        Dexterity = Dexterity!.Value,
        Constitution = Constitution!.Value,
        Intelligence = Intelligence!.Value,
        Wisdom = Wisdom!.Value
    };

}
