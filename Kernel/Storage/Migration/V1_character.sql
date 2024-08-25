CREATE TABLE IF NOT EXISTS position (
    id UUID PRIMARY KEY,
    x FLOAT,
    y FLOAT,
    z FLOAT,
);

CREATE TABLE IF NOT EXISTS name (
    id UUID PRIMARY KEY,
    name TEXT
);

CREATE TABLE IF NOT EXISTS character_stat (
    id UUID PRIMARY KEY,
    strength SMALLINT,
    stamina SMALLINT,
    dexterity SMALLINT,
    constitution SMALLINT,
    intelligence SMALLINT,
    wisdom SMALLINT,
);

CREATE TABLE IF NOT EXISTS hp (
    id UUID PRIMARY KEY,
    hp FLOAT,
    max_hp FLOAT
);

CREATE TABLE IF NOT EXISTS mp (
    id UUID PRIMARY KEY,
    mp SMALLINT,
    max_mp SMALLINT
);

CREATE TABLE IF NOT EXISTS perk (
    id UUID,
    perk TEXT,
    PRIMARY KEY(id, perk)
);
