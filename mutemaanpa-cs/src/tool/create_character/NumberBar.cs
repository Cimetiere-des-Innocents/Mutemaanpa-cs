namespace Mutemaanpa;

using System;
using Godot;

public partial class NumberBar : PanelContainer
{
    public int Value { get; set; }

    public event Action<int>? OnValueChanged;

    [Export]
    private Label? Ability;

    [Export]
    private Button? AddOne;

    [Export]
    private Button? MinusOne;

    public override void _Ready()
    {
        OnValueChanged += (int i) =>
        {
            Ability!.Text = i.ToString();
        };
        AddOne!.Pressed += () =>
        {
            Value += 1;
            OnValueChanged!.Invoke(Value);
        };
        MinusOne!.Pressed += () =>
        {
            Value -= 1;
            OnValueChanged!.Invoke(Value);
        };
    }
}
