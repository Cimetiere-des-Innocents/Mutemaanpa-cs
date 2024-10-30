using System;
using Godot;
using YarnSpinnerGodot;

namespace Mutemaanpa;

public partial class DialogueView : MarginContainer, DialogueViewBase
{
    [Export]
    RichTextLabel? richTextLabel;

    [Export]
    Button? button;

    public event Action? OnDialogueLineFinished;

    public override void _Ready()
    {
        button!.Pressed += () =>
        {
            OnDialogueLineFinished?.Invoke();
            richTextLabel!.Clear();
        };
    }

    public Action? requestInterrupt { get; set; }

    public void DialogueStarted()
    {
        button!.Disabled = true;
    }

    public void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        richTextLabel!.AppendText(dialogueLine.Text.Text);
        richTextLabel!.Newline();
        onDialogueLineFinished();
    }

    /// <inheritdoc/> 
    public void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
        void value(Variant meta)
        {
            GD.Print($"{meta} clicked.");
            var option = meta.AsString();
            var optionIdx = option.Substr(0, option.Find(':')).ToInt() - 1; // -1 because UI interface begins with one while offset begins with 0
            onOptionSelected(optionIdx);
            richTextLabel!.MetaClicked -= value;
        }
        richTextLabel!.MetaClicked += value;
        richTextLabel!.Newline();

        var idx = 1;
        foreach (var option in dialogueOptions)
        {
            richTextLabel!.AppendText($"[url]{idx}: {option.Line.Text.Text}[/url]");
            richTextLabel!.Newline();
            idx++;
        }
        richTextLabel!.Newline();
    }

    public void DialogueComplete()
    {
        button!.Disabled = false;
    }
}
