namespace Mutemaanpa;

using Godot;
using System;

public partial class ResourceBar : MarginContainer
{
    [Export]
    ProgressBar? ProgressBar;

    public Func<int>? GetCurrentValue;

    public Func<int>? GetMaximumValue;

    public override void _Process(double delta)
    {
        ProgressBar!.MaxValue = GetMaximumValue!();
        ProgressBar!.Value = GetCurrentValue!();
    }

    public void SetBarStyle(StyleBoxTexture fillStyle)
    {
        ProgressBar!.AddThemeStyleboxOverride("fill", fillStyle);
    }
}
