CREATE TABLE IF NOT EXISTS character (
    id UUID PRIMARY KEY,
    name TEXT,
    hp REAL,
    mp SMALLINT,
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
);

CREATE TABLE IF NOT EXISTS perk (
    id UUID,
    perk TEXT
);
