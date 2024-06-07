namespace Mutemaanpa;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using DuckDB.NET.Data;
using Godot;

public class OriginMapper : SqlMapper.TypeHandler<Origin>
{
    public override Origin Parse(object value) => value.ToString()?.ToUpper() switch
    {
        string s => Enum.Parse<Origin>(s),
        _ => Origin.NAMELESS_ONE
    };

    public override void SetValue(IDbDataParameter parameter, Origin value)
    {
        parameter.Value = value.ToString();
    }
}


/// <summary>
/// Database load and stores persistent data for Mutemaanpa.
/// </summary>
public class Database(string dbPath)
{
    public void CommitCharacter(CharacterData character)
    {
        using var db = new DuckDBConnection(dbPath);
        string sql = """
            INSERT INTO character(id, name, hp, mp, origin, strength, stamina,
            dexterity, constitution, intelligence, wisdom, player) VALUES 
            (@Id, @Name, @Hp, @Mp, @Origin, @Str, @Sta, @Dex, @Con, @Int, @Wis, @Player);
            """;
        object[] param = { };
        db.Execute(sql, param);
    }

    public IEnumerable<CharacterData> QueryCharacter()
    {
        using var db = new DuckDBConnection(dbPath);
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
