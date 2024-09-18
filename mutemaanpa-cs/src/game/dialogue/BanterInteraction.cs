using Godot;

namespace Mutemaanpa;

public abstract partial class BanterInteraction : Interaction
{
    [Export]
    Label3D? banterBox;

    bool inBanter = false;

    public override void _Ready()
    {
        banterBox = GetNode<Label3D>("../Head");
        banterBox.Text = "And you thought rivellon was flat.";
        banterBox!.Hide();
        base._Ready(); // register shaders
    }

    protected override void DoInteraction()
    {
        ShowBanter(GetBanters());
    }

    protected abstract IBanter GetBanters();

    private void ShowBanter(IBanter banter)
    {
        banterBox!.Show();
        inBanter = true;
        banterBox!.Text = banter.GetText();
        var timer = GetTree().CreateTimer(3.0);
        timer.Timeout += banterBox!.Hide;
        timer.Timeout += () => inBanter = false;
    }
}
