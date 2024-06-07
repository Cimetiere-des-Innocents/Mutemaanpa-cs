namespace Mutemaanpa;
using System;
using System.Collections.Generic;
using Dapper;
using DuckDB.NET.Data;
using Godot;

/// <summary>
/// Database load and stores persistent data for Mutemaanpa.
/// </summary>
public class Database
{
    private string DbPath { get; set; }

    public Database(string dbPath)
    {
        DbPath = dbPath;
        using var db = new DuckDBConnection(dbPath);
        db.Execute(DatabaseConst.SCHEMA);
    }

    public void CommitCharacter(CharacterData character)
    {
        using var db = new DuckDBConnection(DbPath);
        string sql = """
            INSERT INTO character(id, name, hp, mp, origin, strength, stamina,
            dexterity, constitution, intelligence, wisdom, player) VALUES 
            ($Id, $Name, $Hp, $Mp, $Origin, $Str, $Sta, $Dex, $Con, $Int, $Wis, $Player);
            """;
        object[] param = [ new {
            Id = character.Uuid,
            character.Stat.Name,
            character.Stat.Hp,
            character.Stat.Mp,
            character.Stat.Origin,
            Str = character.Ability.Strength,
            Sta = character.Ability.Stamina,
            Dex = character.Ability.Dexterity,
            Con = character.Ability.Constitution,
            Int = character.Ability.Intelligence,
            Wis = character.Ability.Wisdom,
            character.Player
        }];
        db.Execute(sql, param);
    }

    public IEnumerable<CharacterData> QueryCharacter()
    {
        using var db = new DuckDBConnection(DbPath);
        var sql = "SELECT * FROM character NATURAL JOIN position;";
        return db.Query<Guid,
                        CharacterStat,
                        CharacterAbility,
                        Guid,
                        Vector3,
                        CharacterData>
        (
            sql,
            (id, stat, ability, player, position) => new CharacterData(
                Ability: ability,
                Stat: stat,
                Uuid: id,
                Player: player,
                Position: position
            ),
            splitOn: "name,strength,player,x"
        );
    }
}
