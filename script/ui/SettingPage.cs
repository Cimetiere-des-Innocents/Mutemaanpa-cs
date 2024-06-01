namespace Mutemaanpa
{

    using Godot;
    using System.Text.Json;

    public partial class SettingPage : MarginContainer
    {
        #region Persistance
        private readonly string SettingPath = "user://setting";
        public record struct SettingState(bool Test);
        public void WriteToDisk(SettingState settingState)
        {
            using var file = FileAccess.Open(SettingPath, FileAccess.ModeFlags.Write);
            string s = JsonSerializer.Serialize(settingState);
            file.StoreString(s);
        }

        static SettingState ProvideDefaultSetting()
        {
            return new SettingState(false);
        }

        public SettingState ReadFromDisk()
        {
            using FileAccess? file = FileAccess.Open(SettingPath, FileAccess.ModeFlags.Read);
            return file?.GetAsText() switch
            {
                string s when s != "" => JsonSerializer.Deserialize<SettingState>(s),
                _ => ProvideDefaultSetting(),
            };
        }
        #endregion


        #region UI
        [Export]
        private Button? _CancelButton;

        [Export]
        private Button? _OkayButton;

        [Export]
        private CheckBox? _TestCheckBox;

        private SettingState _SettingState;

        public override void _Ready()
        {
            base._Ready();
            _CancelButton!.ButtonUp += () =>
            {
                Router.Of(this).Pop();
            };
            _OkayButton!.ButtonUp += () =>
            {
                WriteToDisk(_SettingState);
                Router.Of(this).Pop();
            };
            _TestCheckBox!.ButtonUp += () =>
            {
                _SettingState.Test = _TestCheckBox.ButtonPressed;
            };
            _SettingState = ReadFromDisk();
            _TestCheckBox?.SetPressedNoSignal(_SettingState.Test);
        }
        #endregion
    }
}
