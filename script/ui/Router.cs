namespace Mutemaanpa
{

    using Godot;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using Welles = System.Lazy<Godot.PackedScene>;

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
    public partial class Router : Control
    {

        private readonly Dictionary<string, Welles> nameToPath = [];
        private readonly Stack<Node> sceneStack = [];

        public enum RouterError
        {
            EMPTY_SWITCH,
            EMPTY_POP,
            EMPTY_STACK_POP,
            ROUTE_NOT_EXIST,
        }

        // a router should only have one child for single responsibility
        [ContractInvariantMethod]
        protected void RouterInvariant()
        {
            Contract.Invariant(GetChildCount() == 1);
        }

        private Router() { }

        public static Router CreateRouter(string defaultPage, params (string name, string uri)[] routes)
        {
            var router = new Router();
            router.Register(routes);
            router.Push(defaultPage);
            return router;
        }

        /// <summary>
        /// Every node should call `Of` if they want to change scene. After this
        /// they get a router so they can do everything they want.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>The most recent Router ancestor</returns>
        public static Router Of(Node node) => node switch
        {
            Router r => r,
            _ => Of(node.GetParent())
        };

        public void Register(params (string, string)[] routes)
        {
            foreach (var (name, path) in routes)
            {
                nameToPath.Add(name, new Welles(() => ResourceLoader.Load<PackedScene>(path)));
            }
        }

        /// <summary>
        /// Replace the current page by page `name`. Note that there must be one child.
        /// return old page, load new page.
        /// </summary>
        /// <param name="name"></param>
        public Node Switch(string name)
        {
            if (GetChildCount() != 1)
            {
                ReportError(RouterError.EMPTY_SWITCH);
            }
            var oldChild = GetChild(0);
            RemoveChild(oldChild);
            AddChildByName(name);
            return oldChild;
        }

        /// <summary>
        /// Overwrite the current scene if it exists, otherwise just put a scene.
        /// </summary>
        public void Overwrite(string name)
        {
            if (GetChildCount() == 1)
            {
                RemoveChildAndFree();
            }
            AddChildByName(name);
        }

        /// <summary>
        /// Show a new scene, and save the old scene to the stack if applicable.
        ///
        /// Note that don't do anything after call this function; it will be undefined.
        /// </summary>
        /// <param name="name"></param>
        public void Push(string name)
        {
            if (GetChildCount() > 0)
            {
                var child = GetChild(0);
                sceneStack.Push(child);
                RemoveChild(child);
            }
            AddChildByName(name);
        }

        private void RemoveChildAndFree()
        {
            var child = GetChild(0);
            RemoveChild(child);
            child.QueueFree();
        }

        private void AddChildByName(string name)
        {
            if (nameToPath.TryGetValue(name, out Welles? welles))
            {
                AddChild(welles!.Value.Instantiate());
            }
            else
            {
                ReportError(RouterError.ROUTE_NOT_EXIST, name);
            }
        }

        /// <summary>
        /// Restore the old saved scene.
        ///
        /// Note that don't do anything after call this function; it will be undefined.
        /// </summary>
        public void Pop()
        {
            RemoveChildAndFree();
            if (sceneStack.TryPop(out Node? result))
            {
                AddChild(result!);
            }
            else
            {
                ReportError(RouterError.EMPTY_STACK_POP);
            }
        }

        /// <summary>
        /// Display a error message to user, then panic.
        /// </summary>
        /// <param name="msg"></param>
        [DoesNotReturn]
        private static void ReportError(RouterError err, params string[] msg)
        {
           throw new Exception(ShowError(err) + msg.Join(","));
        }

        private static string ShowError(RouterError err) => err switch
        {
            RouterError.EMPTY_SWITCH => "Empty router cannot switch its child",
            RouterError.EMPTY_POP => "Empty router cannot pop its page",
            RouterError.EMPTY_STACK_POP => "No more pages in the stack is available",
            RouterError.ROUTE_NOT_EXIST => "The route does not exist",
            _ => throw new ArgumentException("Enum RouterError overflow.")
        };

    }

}
