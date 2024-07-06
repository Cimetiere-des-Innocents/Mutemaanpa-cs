namespace Mutemaanpa;

using System;
using Godot;

public partial class MemberState : AspectRatioContainer
{
    [Export]
    TextureButton? playerButton;

    [Export]
    ResourceBar? hpBar;

    [Export]
    ResourceBar? mpBar;

    MemberLiveData? liveData;

    public void Init(MemberLiveData memberLiveData, Action pressedCallback)
    {
        liveData = memberLiveData;
        (hpBar!.GetMaximumValue, hpBar!.GetCurrentValue, mpBar!.GetMaximumValue, mpBar!.GetCurrentValue)
            = liveData!.Value;
       playerButton!.Pressed += pressedCallback; 
    }

    public override void _Ready()
    {
        hpBar!.SetBarStyle(ResourceLoader.Load<StyleBoxTexture>("res://scene/game/hud/team/style/hp_bar_fill.tres"));
        mpBar!.SetBarStyle(ResourceLoader.Load<StyleBoxTexture>("res://scene/game/hud/team/style/mp_bar_fill.tres"));
    }
}
