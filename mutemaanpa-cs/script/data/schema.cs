namespace Mutemaanpa;

public readonly struct DatabaseConst
{
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
}
