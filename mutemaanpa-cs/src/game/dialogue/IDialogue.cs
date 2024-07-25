using System;
using System.Collections.Generic;

namespace Mutemaanpa;

public record struct Transition
(
    string? Text,
    IDialogue? Next,
    Action? Effect
)
{
    /// <summary>
    /// DummyTransition does not show option. Instead, it prompts a NEXT button or wait several seconds,
    /// depend on implementation.
    /// </summary>
    public readonly bool IsDummyTransition => Text is null && Next is not null;
};

/// <summary>
/// Dialogues can compose a DAG, which has both text and transition to next state.
/// 
/// # Example
/// 
/// Charlotte: Good morning, my dear friend.
/// 
/// [1] Hello, charlotte. -> transit to next piece of dialogue
/// 
/// [2] Goodbye. -> exit dialogue
/// 
/// </summary>
public interface IDialogue
{
    public string GetText();

    public List<Transition> GetNext();

    public bool NextIsDummy()
    {
        var transitions = GetNext();
        return transitions.Count == 1 && transitions[0].IsDummyTransition;
    }
};
