CREATE TYPE ORIGIN AS ENUM (
    'Soldier',
    'Cleric',
    'Rogue',
    'Hunter',
    'Bureaucrat',
    'Spy',
    'Nameless One'
);

CREATE TABLE IF NOT EXISTS position (
    id UUID PRIMARY KEY,
    x FLOAT,
    y FLOAT,
    z FLOAT,
    FOREIGN KEY(id) REFERENCES character(id)
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
