using System;
using Dapper;
using DuckDB.NET.Data;
using Godot;

namespace Mutemaanpa;

public class Journal
{
    public readonly string DbPath;
    private static readonly string SCHEMA = """
        CREATE TABLE IF NOT EXIST journal (
            group UUID PRIMARY KEY,
            key TEXT PRIMARY KEY,
            value TEXT
        );
    """;

    public Journal(string DbPath)
    {
        this.DbPath = DbPath;
        using var db = new DuckDBConnection(DbPath);
        db.Execute(SCHEMA);
    }

    public static Journal Of(Node node) => node switch
    {
        GameMain m => m.Journal!,
        Node n => Of(n.GetParent()),
        _ => throw new Exception("This context has no journal attached")
    };
}
