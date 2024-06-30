namespace Mutemaanpa;

using Godot;

/// <summary>
/// LabelValue is use to display <key>:<value> pair of things.
/// </summary>
public partial class LabelValue : HBoxContainer
{
    [Export]
    Label? KeyLabel;

    [Export]
    Label? ValueLabel;

    public static LabelValue CreateLabelValue(string key, string value)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/hud/label_value.tscn")
            .Instantiate<LabelValue>();
        node.KeyLabel!.Text = key;
        node.ValueLabel!.Text = value;
        return node;
    }
}
