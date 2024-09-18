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

    internal Character? character;

    public override void _Ready()
    {
        Strength!.OnValueChanged += (int strength) =>
        {
            Character.STRENGTH[character!] = strength;
        };

        Stamina!.OnValueChanged += (int stamina) =>
        {
            Character.STAMINA[character!] = stamina;
        };

        Dexterity!.OnValueChanged += (int dexterity) =>
        {
            Character.DEXTERITY[character!] = dexterity;
        };

        Intelligence!.OnValueChanged += (int intelligence) =>
        {
            Character.INTELLIGENCE[character!] = intelligence;
        };

        Wisdom!.OnValueChanged += (int wisdom) =>
        {
            Character.WISDOM[character!] = wisdom;
        };

        Constitution!.OnValueChanged += (int constitution) =>
        {
            Character.CONSTITUTION[character!] = constitution;
        };
    }
}
