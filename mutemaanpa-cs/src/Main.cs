namespace Mutemaanpa;

using System;
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
    public Setting? setting;

    /// <summary>
    /// Manage game saves 
    /// </summary>
    public Catalog? catalog;

    public override void _Ready()
    {
        Logger.endpoint = GD.Print;
        setting = new Setting();
        catalog = new Catalog();
        AddChild(catalog);
        AddRouter();
    }

    private void AddRouter()
    {
        Node newGameHandler()
        {
            var uuid = catalog!.NewSave();
            catalog!.UseGame(uuid);
            return ResourceLoader
                .Load<PackedScene>("res://scene/TestScene.tscn")
                .Instantiate<TestScene>();
        }

        Node settingHandler()
        {
            return SettingPage.CreateSettingPage(setting!);
        }

        Node loadGameHandler()
        {
            return LoadGame.CreateLoadGame(catalog!);
        }

        var router = Router.CreateRouter(
                defaultPage: "/menu",
                routes: [
                (name: "/menu", endpoint: MainMenu.CreateMainMenu),
                (name: "/setting", endpoint: settingHandler),
                (name: "/newGame", endpoint: newGameHandler),
                (name: "/load", endpoint: loadGameHandler)
            ]
        );

        AddChild(router);
    }

    public static Main Get(Node node) => node switch
    {
        null => throw new Exception("empty parent"),
        Main main => main,
        _ => Get(node.GetParent())
    };
}
