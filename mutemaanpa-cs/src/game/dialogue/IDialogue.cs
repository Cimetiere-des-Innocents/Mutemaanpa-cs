using System;
using System.Collections.Generic;
using System.Linq;

namespace Mutemaanpa;

public interface IInteractiveText;

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
public interface IDialogue : IInteractiveText
{
    public string GetText();

    public List<Transition> GetNext();

    public bool NextIsDummy()
    {
        var transitions = GetNext();
        return transitions.Count == 1 && transitions[0].IsDummyTransition;
    }
};

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

public class Dialogue(string text, params Transition[] transitions) : IDialogue
{
    public List<Transition> GetNext() => [.. transitions];

    public string GetText() => text;

    public static Dialogue MakeSeqDialogue(Transition lastTo, params string[] texts) => texts switch
    {
        [] => throw new Exception("At least one text to make a series of conversation."),
        [var lastText] => new(lastText, lastTo),
        [var nextText, .. var remainingTexts] => new(nextText, new Transition(null, MakeSeqDialogue(lastTo, remainingTexts), null))
    };
}

public interface IBanter : IInteractiveText
{
    public string GetText();
}

/// <summary>
/// The banter just repeats and repeats.
/// </summary>
/// <param name="banter"></param>
public class SingleBanter(string banter) : IBanter
{
    public string GetText() => banter;
}

/// <summary>
/// The banter contains many variations.
/// </summary>
/// <param name="choosingStrategy"></param>
/// <param name="banters"></param>
public class ManyBanter(Func<int> choosingStrategy, params string[] banters) : IBanter
{
    public string GetText()
    {
        var next = choosingStrategy() % banters.Length;
        return banters[next];
    }
}

/// <summary>
/// Interaction says banters in a round-taking way.
/// </summary>
/// <param name="banters"></param>
public class SerialBanter(params string[] banters) : ManyBanter(MakeSerialCounter(), banters)
{
    static Func<int> MakeSerialCounter()
    {
        int i = 0;
        return () => i++;
    }
}
