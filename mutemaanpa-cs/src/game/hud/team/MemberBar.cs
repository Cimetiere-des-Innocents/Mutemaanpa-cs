namespace Mutemaanpa;

using System;
using Godot;

public record struct MemberLiveData
(
    Func<int> GetMaxHp,
    Func<int> GetCurHp,
    Func<int> GetMaxMp,
    Func<int> GetCurMp
);

/// <summary>
/// MemberBar allows you to see the whole team conditions.
/// </summary>
public partial class MemberBar : Control
{
    [Export]
    MemberState? memberState;

    public static MemberBar CreateMemberBar(Action clickPlayerCallback,
                                            MemberLiveData memberLiveData)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/hud/team/member_bar.tscn")
            .Instantiate<MemberBar>();
        node.memberState!.Init(memberLiveData, clickPlayerCallback);
        node.LayoutMode = 1; // anchor mode
        return node;
    }
}
