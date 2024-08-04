using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using DuckDB.NET.Data;

namespace Mutemaanpa;

public record struct SaveData(
    Guid Id,
    DateTime CreatedAt,
    DateTime LastPlayed
);

public class SaveDatabase(string DbPath)
{
    private static readonly string SCHEMA = """
        CREATE TABLE IF NOT EXISTS save_slot (
            id UUID PRIMARY KEY,
            created_at TIMESTAMP DEFAULT current_timestamp,
            last_played TIMESTAMP DEFAULT current_timestamp
        );
    """;

    /// <summary>
    /// Run SQL Data Define language.
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
            new
            {
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

    public bool HasSave() => QuerySaves().Any();

    internal void Remove(Guid id)
    {
        using var db = new DuckDBConnection(DbPath);
        db.Execute(
            """
            DELETE FROM save_slot WHERE id = $id
            """,
            new
            {
                id
            }
        );
    }
}
