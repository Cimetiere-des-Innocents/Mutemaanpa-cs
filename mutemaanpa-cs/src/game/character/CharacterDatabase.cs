namespace Mutemaanpa;
using System;
using System.Collections.Generic;
using Dapper;
using DuckDB.NET.Data;
using Godot;


/// <summary>
/// Database load and stores persistent data for Mutemaanpa.
/// </summary>
public class CharacterDatabase(string DbPath)
{
    public string DbPath { get; private set; } = DbPath;

    public static readonly string SCHEMA = """
        CREATE TYPE ORIGIN AS ENUM (
            'SOLDIER',
            'CLERIC',
            'ROGUE',
            'HUNTER',
            'BUREAUCRAT',
            'SPY',
            'NAMELESS ONE'
        );

        CREATE TABLE IF NOT EXISTS character (
            id UUID PRIMARY KEY,
            
            name TEXT,
            hp REAL,
            mp SMALLINT,
            origin ORIGIN,
            
            strength SMALLINT,
            stamina SMALLINT,
            dexterity SMALLINT,
            constitution SMALLINT,
            intelligence SMALLINT,
            wisdom SMALLINT,

            player UUID
        );

        CREATE TABLE IF NOT EXISTS position (
            id UUID PRIMARY KEY,
            x FLOAT,
            y FLOAT,
            z FLOAT,
            FOREIGN KEY(id) REFERENCES character(id)
        );
        """;

    public void Init()
    {
        using var db = new DuckDBConnection(DbPath);
        db.Execute(SCHEMA);
    }

    public void CommitCharacter(CharacterData character)
    {
        using var db = new DuckDBConnection(DbPath);
        db.Open();
        using var tx = db.BeginTransaction();
        string sql = """
            INSERT OR REPLACE INTO character(id, name, hp, mp, origin, strength, stamina,
            dexterity, constitution, intelligence, wisdom, player) VALUES 
            ($Id, $Name, $Hp, $Mp, $Origin, $Str, $Sta, $Dex, $Con, $Int, $Wis, $Player);
            """;
        object[] param = [ new {
            Id = character.Uuid,
            character.Stat.Name,
            character.Stat.Hp,
            character.Stat.Mp,
            Origin = character.Stat.Origin.ToString(),
            Str = character.Ability.Strength,
            Sta = character.Ability.Stamina,
            Dex = character.Ability.Dexterity,
            Con = character.Ability.Constitution,
            Int = character.Ability.Intelligence,
            Wis = character.Ability.Wisdom,
            character.Player
        }];
        db.Execute(sql, param);

        if (character.Position != null)
        {
            sql = """
            INSERT OR REPLACE INTO position(id, x, y, z) VALUES ($Id, $X, $Y, $Z);
            """;
            param = [ new {
                Id = character.Uuid,
                character.Position?.X,
                character.Position?.Y,
                character.Position?.Z
            }];
            db.Execute(sql, param);
        }
        tx.Commit();
        db.Close();
    }

    public IEnumerable<CharacterData> QueryCharacter()
    {
        using var db = new DuckDBConnection(DbPath);
        var sql = "SELECT * FROM character NATURAL LEFT OUTER JOIN position;";
        return db.Query<Guid,
                        CharacterStat,
                        CharacterAbility,
                        Guid?,
                        (float?, float?, float?), // https://github.com/DapperLib/Dapper/issues/1900
                        CharacterData>
        (
            sql,
            (id, stat, ability, player, position) => new CharacterData()
            {
                Ability = ability,
                Stat = stat,
                Uuid = id,
                Player = player,
                Position = position switch
                {
                    (float x, float y, float z) => new Vector3(x, y, z),
                    _ => null
                }
            },
            splitOn: "name, strength, player, x"
        );
    }
}
