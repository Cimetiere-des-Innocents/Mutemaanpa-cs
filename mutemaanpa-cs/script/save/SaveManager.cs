using System;
using System.Collections.Generic;
using Dapper;
using DuckDB.NET.Data;

namespace Mutemaanpa;

public record struct SaveData (
    Guid Uuid,
    DateTime CreatedAt,
    DateTime LastPlayed
);

public class SaveDatabase(string DbPath)
{
    private static readonly string SCHEMA = """
        CREATE TABLE IF NOT EXISTS save_slot (
            id UUID PRIMARY KEY,
            created_at TIMESTAMP DEFAULT current_timestamp
            last_played TIMESTAMP DEFAULT current_timestamp
        );
    """;

    /// <summary>
    /// Run SQL Data Define language. Create table. Only execute when user first
    /// run this game.
    /// </summary>    
    public void InitDatabase()
    {
        using var db = new DuckDBConnection(DbPath);
        db.Execute(SCHEMA);
    }

    public Guid NewSave()
    {
        Guid saveUuid = Guid.NewGuid();
        using var db = new DuckDBConnection(DbPath);
        db.Execute(
            """
            INSERT INTO save_slot (id) VALUES ($id)
            """,
            new {
                id = saveUuid
            }
        );
        return saveUuid;
    }

    public IEnumerable<SaveData> QuerySaves()
    {
        using var db = new DuckDBConnection(DbPath);
        return db.Query<SaveData>(
            """
            SELECT * FROM save_slot;
            """
        );
    }
}
