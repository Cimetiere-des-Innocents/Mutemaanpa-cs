CREATE TYPE ORIGIN AS ENUM (
    'Soldier',
    'Cleric',
    'Rogue',
    'Hunter',
    'Bureaucrat',
    'Spy',
    'Nameless One'
);

CREATE TYPE VECTOR3 AS STRUCT(
    x FLOAT,
    y FLOAT,
    z FLOAT,
);

CREATE TABLE IF NOT EXISTS character (
    id UUID NOT NULL,
    hp REAL,
    mp SMALLINT,
    position VECTOR3,
    
    strength SMALLINT,
    stamina SMALLINT,
    dexterity SMALLINT,
    constitution SMALLINT,
    intelligence SMALLINT,
    wisdom SMALLINT,

    origin ORIGIN,
    player UUID 
);


