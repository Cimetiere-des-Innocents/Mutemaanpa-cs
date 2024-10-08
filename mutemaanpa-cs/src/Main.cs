namespace Mutemaanpa;

using Godot;

/// <summary>
/// Main class controls the whole game wide configuration / states.
///
///
/// # Main States
///
/// ----------------------------------------------------
/// |   [Load Game] ------------------------           |
/// |    1^                               6^           |
/// |    2V                               7v        12 |
/// --> [Title] 3<->4 [New Game] ---->5 [In Game]<------
///  13  8^                                 ^
///      9v                                 | 10
///     [Setting] <--------------------------
///               11
///
///
///  1. click "Load Game"
///  2. click "Back To Title"
///  3. click "New Game"
///  4. click "Back To Title"
///  5. click "Start Game"
///  6. click "Load Game"
///  7. click "Start Game"
///  8. click "Ok"
///  9. click "Settings"
///  10. click "Ok"
///  11. click "Settings"
///  12. click "Back To Title"
///  13. click "Continue"
///
/// </summary>
public partial class Main : PanelContainer
{
    /// <summary>
    /// Manage game settings
    /// </summary>
    MetadataManager? metadata;

    /// <summary>
    /// Manage game saves 
    /// </summary>
    Catalog? catalog;

    public override void _Ready()
    {
        metadata = new MetadataManager();
        catalog = new Catalog();
        AddChild(catalog);
        AddRouter();
    }

    private void AddRouter()
    {
        Node newGameHandler()
        {
            var save = catalog!.NewSave();
            return GameMain.CreateGameMain(save);
        }

        Node settingHandler() { return SettingPage.CreateSettingPage(metadata!); }

        Node loadGameHandler() => LoadGame.CreateLoadGame(catalog!);

        var router = Router.CreateRouter(
                defaultPage: "/menu",
                routes: [
                (name: "/menu", endpoint: MainMenu.CreateMainMenu),
                (name: "/setting", endpoint: settingHandler),
                (name: "/newGame", endpoint: newGameHandler),
                (name: "/load", endpoint: loadGameHandler)
            ]
        );

        Node GetGameOverScene()
        {
            GetTree().Paused = true;
            void CleanGameSession()
            {
                GetTree().Paused = false;
                foreach (var child in router.GetChildren())
                {
                    router.RemoveChild(child);
                    child.QueueFree();
                }
                router.Push("/menu");
            }
            return GameOver.CreateGameOver(
                ToTitleAction: CleanGameSession,
                LoadGameAction: () =>
                {
                    CleanGameSession();
                    router.Push("/load");
                }
            );
        }
        router.Register(("/gameOver", GetGameOverScene));
        AddChild(router);
    }
}
