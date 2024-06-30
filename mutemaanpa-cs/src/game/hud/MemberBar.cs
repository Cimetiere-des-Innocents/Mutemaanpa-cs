namespace Mutemaanpa;

using System;
using Godot;

/// <summary>
/// MemberBar allows you to see the whole team conditions.
/// </summary>
public partial class MemberBar : Control
{
    [Export]
    TextureButton? playerButton;

    public static MemberBar CreateMemberBar(Action clickPlayerCallback)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/hud/member_bar.tscn")
            .Instantiate<MemberBar>();
        node.playerButton!.Pressed += clickPlayerCallback;
        node.LayoutMode = 1; // anchor mode
        return node;
    }
}
