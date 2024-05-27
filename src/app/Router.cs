using Godot;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Welles = System.Lazy<Godot.Node>;

/// <summary>
/// Router controls the navigation of scenes. The router node is responsible of
///
/// 1) switch this scene to a new scene
/// 2) stack a new scene above this scene, save old state in a stack
/// 3) pop out a scene ini the stack
///
/// In order to do this, any component in the node tree must know what its
/// router is. To get this knowledge I expose the `of` static method to find
/// them.
///
/// </summary>
public partial class Router : Control {

    private Dictionary<string, Welles> nameToPath;
    private Stack<Node> sceneStack;

    // a router should only have one child for single responsibility
    [ContractInvariantMethod]
    protected void RouterInvariant() {
        Contract.Invariant(GetChildCount() == 1);
    }


    /// <summary>
    /// Every node should call `Of` if they want to change scene. After this
    /// they get a router so they can do everything they want.
    /// </summary>
    /// <param name="node"></param>
    /// <returns>The most recent Router ancestor</returns>
    public static Router Of(Node node) => node switch {
        Router r => r,
        _ => Of(node.GetParent())
    };

    public void Register(params (string, string)[] routes) {
        foreach (var (name, path) in routes)
        {
            nameToPath.Add(name, new Welles(() => ResourceLoader.Load<PackedScene>(path).Instantiate()));
        }
    }

    // TODO: make these methods safer
    public void Switch(string name) {
        GetChild(0).Free();
        AddChild(nameToPath[name].Value);
    }

    public void Push(string name) {
        sceneStack.Push(GetChild(0));
        GetChild(0).ReplaceBy(nameToPath[name].Value);
    }

    public void Pop() {
        GetChild(0).Free();
        AddChild(sceneStack.Pop());
    }

}
