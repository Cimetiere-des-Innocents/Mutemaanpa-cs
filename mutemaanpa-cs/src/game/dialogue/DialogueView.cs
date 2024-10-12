using System;
using Godot;
using YarnSpinnerGodot;

namespace Mutemaanpa;

public partial class DialogueView : MarginContainer, DialogueViewBase
{
    [Export]
    RichTextLabel? richTextLabel;

    public override void _Ready()
    {
    }

    public Action? requestInterrupt { get; set; }

    public void DialogueStarted()
    {
    }

    public void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        richTextLabel!.AppendText(dialogueLine.Text.Text);
        onDialogueLineFinished();
    }

    /// <inheritdoc/> 
    public void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
        foreach (var option in dialogueOptions) {
            richTextLabel!.AppendText(option.Line.Text.Text);
        }
    }

    public void DialogueComplete()
    {
        // Default implementation does nothing.
    }
}
