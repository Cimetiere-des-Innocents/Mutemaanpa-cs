namespace Mutemaanpa
{
    using System;
    using DuckDB.NET.Data;
    /// <summary>
    /// Database load and stores persistent data for Mutemaanpa.
    /// </summary>
    public class Database: IDisposable
    {
        readonly DuckDBConnection db;

        public Database(string dbPath) 
        {
            db = new DuckDBConnection(dbPath);
            db.Open();
        }

        public void CommitCharacter(CharacterState character)
        {
            
        }

        public void Dispose()
        {
            db.Close();
            GC.SuppressFinalize(this);
        }
    }
}
