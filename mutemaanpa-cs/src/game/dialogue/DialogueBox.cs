namespace Mutemaanpa;

using Godot;

/// <summary>
/// DialogueBox is the widget for long, stateful conversation between interactions. It is used to
/// pass user input to controller and then show internal states. It is by its nature not stateful.
/// </summary>
public partial class DialogueBox : Control
{
    IDialogue? controller;

    [Export]
    RichTextLabel? view;

    public static DialogueBox CreateDialogueBox(IDialogue dialogue)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/dialogue/dialogue_box.tscn")
            .Instantiate<DialogueBox>();
        node.controller = dialogue;
        return node;
    }

    private void ShowDialogue()
    {
        view!.AppendText(controller!.GetText());
        var optionList = 1;
        foreach (var transition in controller!.GetNext())
        {
            view!.AppendText($"[{optionList++}]: {transition!.Text}");
        }
    }
}
