namespace Mutemaanpa
{
    using Godot;

    /// <summary>
    /// Since Godot free loaded resources automatically, we don't have to free PackedScene
    /// by ourselves.
    ///
    /// states:
    ///
    /// Unloaded --> Loaded <-> Instantiated
    ///
    /// </summary>

    internal partial interface IWellesState;

    internal record struct Unloaded(string ScenePath) : IWellesState;

    internal record struct Loaded(string ScenePath, PackedScene PackedScene) : IWellesState;

    internal record struct Instantiated(
        string ScenePath,
        PackedScene PackedScene,
        Node InstantiatedScene
    ) : IWellesState;


    internal partial interface IWellesState
    {
        public static IWellesState NewScene(string Path) => new Unloaded(Path);

        /// ATTENTION: this is not in-placed
        public IWellesState LoadScene() => this switch
        {
            Unloaded(var ScenePath) => new Loaded(ScenePath, ResourceLoader.Load<PackedScene>(ScenePath)),
            Loaded or Instantiated => this,
            _ => throw new System.NotImplementedException(),
        };

        // ATTENTION: this is not in-placed
        public Instantiated InstantiateScene() => this switch
        {
            Unloaded or Loaded => this.LoadScene() switch
            {
                Loaded(var scenePath, PackedScene packedScene) => new Instantiated(
                    scenePath,
                    packedScene,
                    packedScene.Instantiate()
                ),
                _ => throw new System.NotImplementedException(),
            },
            Instantiated i => i,
            _ => throw new System.NotImplementedException(),
        };

        public Node GetNode() => InstantiateScene().InstantiatedScene;

        private static Loaded FreeInstantiated(Instantiated instantiated)
        {
            var (path, packed, node) = instantiated;
            node.QueueFree();
            return new Loaded(path, packed);
        }

        public IWellesState FreeInstance() => this switch
        {
            Instantiated i => FreeInstantiated(i),
            Unloaded or Loaded => this,
            _ => throw new System.NotImplementedException(),
        };
    }

    /// Mutable facade for welles
    public class Welles(string path)
    {
        private IWellesState State = new Unloaded(path);

        public void LoadScene()
        {
            State = State.LoadScene();
        }

        public Node GetNode()
        {
            State = State.InstantiateScene();
            return State.GetNode();
        }

        public void FreeInstance()
        {
            State = State.FreeInstance();
        }
    }
}
