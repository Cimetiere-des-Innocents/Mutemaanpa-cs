namespace Mutemaanpa;

using Godot;

public partial class NumberBar : PanelContainer
{
    public byte Value { get; set; }

    [Export]
    private Label? Ability;

    [Export]
    private Button? AddOne;

    [Export]
    private Button? MinusOne;

    public override void _Ready()
    {
        AddOne!.Pressed += () => {
            Value += 1;
            Ability!.Text = Value.ToString();
        };
        MinusOne!.Pressed += () => {
            Value -= 1;
            Ability!.Text = Value.ToString();
        };
    }

}
