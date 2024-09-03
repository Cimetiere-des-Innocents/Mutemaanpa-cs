
using Godot;
using System;

namespace Mutemaanpa;
public partial class ResourceBar : MarginContainer
{
    [Export]
    ProgressBar? ProgressBar;

    public Func<double>? GetCurrentValue;

    public Func<double>? GetMaximumValue;

    public override void _Process(double delta)
    {
        ProgressBar!.MaxValue = GetMaximumValue!();
        ProgressBar!.Value = GetCurrentValue!();
        if (ProgressBar!.MaxValue == 0)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void SetBarStyle(StyleBoxTexture fillStyle)
    {
        ProgressBar!.AddThemeStyleboxOverride("fill", fillStyle);
    }
}
