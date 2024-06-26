namespace Mutemaanpa;

using Godot;

public partial class SettingPage : MarginContainer
{

    private MetadataManager? metadataManager;

    [Export]
    private Button? _CancelButton;

    [Export]
    private Button? _OkayButton;

    [Export]
    private CheckBox? _TestCheckBox;

    public static SettingPage CreateSettingPage(MetadataManager metadataManager)
    {
        var settingPage = ResourceLoader.Load<PackedScene>("res://scene/tool/setting_page.tscn")
            .Instantiate<SettingPage>();
        settingPage.metadataManager = metadataManager;
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
            metadataManager!.WriteToDisk();
            Router.Of(this).Pop();
        };
        _TestCheckBox!.ButtonUp += () =>
        {
            metadataManager!.Metadata.Test = _TestCheckBox.ButtonPressed;
        };
        _TestCheckBox?.SetPressedNoSignal(metadataManager!.Metadata.Test);
    }
}

