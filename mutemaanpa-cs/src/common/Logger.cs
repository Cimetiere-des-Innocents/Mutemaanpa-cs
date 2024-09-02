using System;

namespace Mutemaanpa;

public static class Logger
{
    public static Action<string> endpoint { get; set; } = Console.WriteLine;

    public static void Info(string s)
    {
        endpoint($"[INFO] {s}");
    }

    public static void Warn(string s)
    {
        endpoint($"[WARN] {s}");
    }
}
