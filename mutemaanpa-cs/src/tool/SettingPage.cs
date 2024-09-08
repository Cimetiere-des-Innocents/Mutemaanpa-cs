namespace Mutemaanpa;

using System;
using System.Text.Json;
using Godot;


public record struct SettingData
(
    bool Test
);

/// <summary>
/// MetadataManager persists some key-value configurations in JSON format
/// 
/// </summary>
public class Setting
{
    private static readonly string MetadataPath = "user://setting";

    public SettingData Metadata;

    public Setting() => LoadMetadata();

    public void WriteToDisk()
    {
        using var file = FileAccess.Open(MetadataPath, FileAccess.ModeFlags.Write);
        string s = JsonSerializer.Serialize(Metadata);
        file.StoreString(s);
    }

    private static SettingData ProvideDefaultMetadata()
    {
        return new SettingData(
            Test: false
        );
    }

    private void LoadMetadata()
    {
        using FileAccess? file = FileAccess.Open(MetadataPath, FileAccess.ModeFlags.Read);
        Metadata = file?.GetAsText() switch
        {
            string s when s != "" => JsonSerializer.Deserialize<SettingData>(s),
            _ => ProvideDefaultMetadata(),
        };
    }
}

public partial class SettingPage : MarginContainer
{

    private Setting? setting;

    [Export]
    private Button? _CancelButton;

    [Export]
    private Button? _OkayButton;

    [Export]
    private CheckBox? _TestCheckBox;

    public static SettingPage CreateSettingPage(Setting setting)
    {
        var settingPage = ResourceLoader.Load<PackedScene>("res://scene/tool/setting_page.tscn")
            .Instantiate<SettingPage>();
        settingPage.setting = setting;
        return settingPage;
    }

    public override void _Ready()
    {
        base._Ready();
        _CancelButton!.ButtonUp += () =>
        {
            Router.Of(this).Pop();
        };
        _OkayButton!.ButtonUp += () =>
        {
            setting!.WriteToDisk();
            Router.Of(this).Pop();
        };
        _TestCheckBox!.ButtonUp += () =>
        {
            setting!.Metadata.Test = _TestCheckBox.ButtonPressed;
        };
        _TestCheckBox?.SetPressedNoSignal(setting!.Metadata.Test);
    }
}
