using System;

namespace Mutemaanpa;

public interface IBanter
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
