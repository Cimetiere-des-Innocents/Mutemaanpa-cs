
using System;
using Godot;

namespace Mutemaanpa;
/// <summary>
/// DialogueBox is the widget for long, stateful conversation between interactions. It is used to
/// pass user input to controller and then show internal states. It is by its nature not stateful.
/// </summary>
public partial class DialogueBox : Control
{
    [Export]
    RichTextLabel? view;

    IDialogue? controller;

    public static DialogueBox CreateDialogueBox(IDialogue dialogue)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/dialogue/dialogue_box.tscn")
            .Instantiate<DialogueBox>();
        node.controller = dialogue;
        node.ShowDialogue();
        node.view!.MetaClicked += (Variant meta) =>
        {
            GD.Print($"{meta} clicked.");
            var option = meta.AsString();
            var optionIdx = option.Substr(0, option.Find(':')).ToInt() - 1; // -1 because UI interface begins with one while offset begins with 0
            (_, IDialogue? next, Action? action) = node.controller!.GetNext()[optionIdx];
            if (next != null)
            {
                node.controller = next;
                node.ShowDialogue();
            }
            action?.Invoke();
        };
        return node;
    }

    private void ShowDialogue()
    {
        view!.AppendText($"[p]{controller!.GetText()}[/p]");
        view!.Newline();
        var optionList = 1;
        foreach (var transition in controller!.GetNext())
        {
            view!.AppendText($"[url]{optionList++}: {transition!.Text ?? "Continue"}[/url]");
            view!.Newline();
        }
        view!.Newline();
    }
}
