using Godot;

public partial class ErrorPage(string errorMessage) : VBoxContainer
{
    [Export]
    private Label errorMessageDisplay;

    public override void _Ready()
    {
        base._Ready();
        errorMessageDisplay.Text = errorMessage;
    }

}
