namespace Mutemaanpa;
using System;
using System.Collections.Generic;

/// <summary>
/// Holds all in-game characters data, and persists them.
/// </summary>
public class CharacterManager(Database database)
{
    private Dictionary<Guid, CharacterState> uuidToCharacter = [];

    /// <summary>
    /// Generate a new character with its data, assigns uuid.
    /// </summary>
    /// <param name="characterState"></param>
    public void RegisterCharacter(CharacterStat stat,
                                  CharacterAbility ability,
                                  Guid player)
    {
        var uuid = Guid.NewGuid();
        var data = new CharacterData(ability, stat, uuid, Godot.Vector3.Up, null);
        var runtime = CalculateRuntimeProperty(data);
        var state = new CharacterState(data, runtime);
        uuidToCharacter.Add(uuid, state);
    }

    public void Store()
    {
        foreach (var state in uuidToCharacter.Values)
        {
            var data = state.CharacterData;
            database.CommitCharacter(data);
        }
    }

    public void Load()
    {
        foreach (var character in database.QueryCharacter())
        {

        }
    }

    private static CharacterRuntime CalculateRuntimeProperty(CharacterData data)
    {
        return new CharacterRuntime(MaxHitPoint: 10, MaxManaPoint: 10);
    }
}

