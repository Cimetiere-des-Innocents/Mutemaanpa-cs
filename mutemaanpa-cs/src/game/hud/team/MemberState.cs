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

    public void Init(MemberLiveData memberLiveData, Action pressedCallback)
    {
        (hpBar!.GetMaximumValue, hpBar!.GetCurrentValue, mpBar!.GetMaximumValue, mpBar!.GetCurrentValue)
            = memberLiveData;
       playerButton!.Pressed += pressedCallback; 
    }

    public override void _Ready()
    {
        hpBar!.SetBarStyle(ResourceLoader.Load<StyleBoxTexture>("res://scene/game/hud/team/style/hp_bar_fill.tres"));
        mpBar!.SetBarStyle(ResourceLoader.Load<StyleBoxTexture>("res://scene/game/hud/team/style/mp_bar_fill.tres"));
    }
}
