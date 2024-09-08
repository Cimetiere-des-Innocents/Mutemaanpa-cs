namespace Mutemaanpa;

using System;
using Godot;

/// <summary>
/// DialogueBox is the widget for long, stateful conversation between interactions. It is used to
/// pass user input to controller and then show internal states. It is by its nature not stateful.
/// </summary>
public partial class DialogueBox : Control
{
    [Export]
    RichTextLabel? view;

    public static DialogueBox CreateDialogueBox()
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/dialogue/dialogue_box.tscn")
            .Instantiate<DialogueBox>();
        node.ShowDialogue();
        node.view!.MetaClicked += (Variant meta) =>
        {
            GD.Print($"{meta} clicked.");
            var option = meta.AsString();
            var optionIdx = option.Substr(0, option.Find(':')).ToInt() - 1; // -1 because UI interface begins with one while offset begins with 0
        };
        return node;
    }

    private void ShowDialogue()
    {

    }
}
