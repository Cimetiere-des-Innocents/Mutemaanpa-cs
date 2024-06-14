namespace Mutemaanpa;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Holds all in-game characters data, and persists them.
/// </summary>
public class CharacterManager(CharacterDatabase database)
{
    private readonly Dictionary<Guid, CharacterState> UuidToCharacter = [];

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
        // Some properties must be set after calculation.
        var runtimeState = CalculateRuntimeProperty(data);
        var newStat = stat with
        {
            Hp = runtimeState.MaxHitPoint,
            Mp = runtimeState.MaxManaPoint
        };
        data.Stat = newStat;
        UuidToCharacter.Add(uuid, new(data, runtimeState));
        return uuid;
    }

    public void Store()
    {
        foreach (var state in UuidToCharacter.Values)
        {
            var data = state.CharacterData;
            database.CommitCharacter(data);
        }
    }

    public void Load()
    {
        foreach (var character in database.QueryCharacter())
        {
            UuidToCharacter.Add(character.Uuid, LoadCharacter(character));
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
        => UuidToCharacter.TryGetValue(guid, out var v)
            ? v
            : null;


    public CharacterState GetPlayer(/* TODO: distinguish players */)
        => (from c in UuidToCharacter.Values
            where c.CharacterData.Player is not null
            select c).First();
}

