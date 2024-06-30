namespace Mutemaanpa;

using System;
using Godot;

/// <summary>
/// Hud of the game world.
/// </summary>
public partial class WorldHud : Control
{
    MemberBar? memberBar;

    public static WorldHud CreateWorldHud(Action clickPlayerCallback)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/hud/world_hud.tscn")
            .Instantiate<WorldHud>();
        node.memberBar = MemberBar.CreateMemberBar(clickPlayerCallback);
        node.AddChild(node.memberBar);
        return node;
    }
}
