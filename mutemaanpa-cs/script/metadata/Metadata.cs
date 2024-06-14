namespace Mutemaanpa;
using System;
using System.Text.Json;
using Godot;


public record struct MetadataState
(
    bool Test,
    Guid PlayerId
);

/// <summary>
/// MetadataManager persists some key-value configurations in JSON format because
/// they are not very convenient stored in relational databases.
/// 
/// It should have no external dependencies.
/// </summary>
public class MetadataManager
{
    private static readonly string MetadataPath = "user://setting";
    public bool FirstTimeLaunch { get; private set; } = false;

    public MetadataState Metadata;

    public MetadataManager() => LoadMetadata();

    public void WriteToDisk()
    {
        using var file = FileAccess.Open(MetadataPath, FileAccess.ModeFlags.Write);
        string s = JsonSerializer.Serialize(Metadata);
        file.StoreString(s);
    }

    private MetadataState ProvideDefaultMetadata()
    {
        FirstTimeLaunch = true;
        return new MetadataState(
            Test: false,
            PlayerId: Guid.NewGuid()
        );
    }

    private void LoadMetadata()
    {
        using FileAccess? file = FileAccess.Open(MetadataPath, FileAccess.ModeFlags.Read);
        Metadata = file?.GetAsText() switch
        {
            string s when s != "" => JsonSerializer.Deserialize<MetadataState>(s),
            _ => ProvideDefaultMetadata(),
        };
    }
}
