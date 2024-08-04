using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using DuckDB.NET.Data;
using Godot;

namespace Mutemaanpa;

/// <summary>
/// Journal maintains game level states. Everyone can check whether they have any flags set.
/// 
/// group = save's UUID: these state is set and managed by game main.
/// 
/// group = character's UUID: these state is set and managed by some character.
/// </summary>
public class Journal
{
    public readonly string DbPath;
    public readonly Dictionary<Guid, Dictionary<string, string>> groupToKeyToValue = [];
    private static readonly string SCHEMA = """
        CREATE TABLE IF NOT EXISTS journal (
            owner UUID,
            key TEXT,
            value TEXT,
            PRIMARY KEY(owner, key)
        );
    """;

    public Journal(string dbPath)
    {
        DbPath = $"Data Source={dbPath}";
        using var db = new DuckDBConnection(DbPath);
        db.Open();
        db.Execute(SCHEMA);
        Load();
    }

    public static Journal Of(Node node) => node switch
    {
        GameMain m => m.Journal!,
        Node n => Of(n.GetParent()),
        _ => throw new Exception("This context has no journal attached")
    };

    public void Set(Guid uuid, string key, string value)
    {
        var keyToValue = groupToKeyToValue.GetOrCreate(uuid);
        keyToValue.InsertOrReplace(key, value);
    }

    public string? Get(Guid uuid, string key)
    {
        return groupToKeyToValue.TryGetValue(uuid, out var keyToValue)
            ? keyToValue?.TryGetValue(key, out var value) ?? false ? value : null
            : null;
    }

    public void Store()
    {
        using var db = new DuckDBConnection(DbPath);
        db.Open();
        string template = """
        INSERT OR REPLACE INTO journal VALUES ($uuid, $key, $value);
        """;
        foreach (var (uuid, keyToValue) in groupToKeyToValue.AsEnumerable())
        {
            foreach (var (key, value) in keyToValue.AsEnumerable())
            {
                object[] param = [new { uuid, key, value }];
                db.Execute(template, param);
            }
        }
    }

    public void Load()
    {
        var sql = """
        SELECT * FROM journal;
        """;
        using var db = new DuckDBConnection(DbPath);
        db.Open();
        var entries = db.Query<(Guid, string, string)>(sql).AsList();
        foreach (var (uuid, key, value) in entries)
        {
            Set(uuid, key, value);
        }
    }
}
