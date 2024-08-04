namespace Mutemaanpa;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Holds all in-game characters data in memory, and persists them.
/// </summary>
public class CharacterMemory(CharacterDatabase database)
{
    private readonly Dictionary<Guid, Character> UuidToCharacter = [];

    /// <summary>
    /// Generate a new character with its data, assigns uuid.
    /// </summary>
    /// <param name="characterState"></param>
    public Guid RegisterCharacter(CharacterStat stat,
                                  CharacterAbility ability,
                                  Vector3 spawnPoint,
                                  Guid? player)
    {
        var uuid = Guid.NewGuid();
        var data = new CharacterData()
        {
            Ability = ability,
            Stat = stat,
            Uuid = uuid,
            Position = spawnPoint,
            Player = player
        };
        // Some properties must be set after calculation.
        var runtimeState = CalculateRuntimeProperty(data);
        stat.Hp = runtimeState.MaxHitPoint;
        stat.Mp = runtimeState.MaxManaPoint;
        UuidToCharacter.Add(uuid, new(new CharacterState()
        {
            Data = data,
            Runtime = runtimeState
        }));
        return uuid;
    }

    public void Store()
    {
        foreach (var character in UuidToCharacter.Values)
        {
            var data = character.Dump();
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

    private static Character LoadCharacter(CharacterData data) => new(LoadCharacterState(data));

    private static CharacterState LoadCharacterState(CharacterData data) =>
        new()
        {
            Data = data,
            Runtime = CalculateRuntimeProperty(data)
        };


    private static CharacterRuntime CalculateRuntimeProperty(CharacterData data)
    {
        return new CharacterRuntime()
        {
            MaxHitPoint = data.Ability.Constitution,
            MaxManaPoint = 0,
            Speed = 5.0f
        };
    }

    public CharacterState? GetCharacterState(Guid guid)
        => UuidToCharacter.TryGetValue(guid, out var v)
            ? v.State()
            : null;

    public CharacterState GetPlayerState() => GetPlayer().State();

    public Character GetPlayer() => (from c in UuidToCharacter.Values
                                     where c.State().Data.Player is not null
                                     select c).First();
}

