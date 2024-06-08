namespace Mutemaanpa;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Holds all in-game characters data, and persists them.
/// </summary>
public class CharacterManager(Database database)
{
    private readonly Dictionary<Guid, CharacterState> uuidToCharacter = [];

    /// <summary>
    /// Generate a new character with its data, assigns uuid.
    /// </summary>
    /// <param name="characterState"></param>
    public Guid RegisterCharacter(CharacterStat stat,
                                  CharacterAbility ability,
                                  Vector3? spawnPoint,
                                  Guid? player)
    {
        var uuid = Guid.NewGuid();
        var data = new CharacterData(ability, stat, uuid, spawnPoint, player);
        uuidToCharacter.Add(uuid, LoadCharacter(data));
        return uuid;
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
            uuidToCharacter.Add(character.Uuid, LoadCharacter(character));
        }
    }

    private static CharacterState LoadCharacter(CharacterData data)
    {
        return new(
            data,
            CalculateRuntimeProperty(data)
        );
    }

    private static CharacterRuntime CalculateRuntimeProperty(CharacterData data)
    {
        return new CharacterRuntime(
            MaxHitPoint: data.Ability.Constitution,
            MaxManaPoint: data.Ability.Stamina
        );
    }

    public CharacterState? GetCharacterState(Guid guid)
        => uuidToCharacter.TryGetValue(guid, out var v)
            ? v
            : null;


    public CharacterState GetPlayer(/* TODO: distinguish players */)
        => (from c in uuidToCharacter.Values
            where c.CharacterData.Player is not null
            select c).First();
}

