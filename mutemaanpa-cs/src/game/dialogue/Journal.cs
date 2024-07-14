using System;
using Godot;

namespace Mutemaanpa;

public class Journal
{
    public static Journal Of(Node node) => node switch
    {
        GameMain m => m.Journal!,
        Node n => Of(n.GetParent()),
        _ => throw new Exception("This context has no journal attached")
    };

}
