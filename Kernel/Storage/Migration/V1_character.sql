CREATE TABLE IF NOT EXISTS position (
    x FLOAT,
    y FLOAT,
    z FLOAT,
    id UUID PRIMARY KEY,
);

CREATE TABLE IF NOT EXISTS name (
    name TEXT,
    id UUID PRIMARY KEY,
);

CREATE TABLE IF NOT EXISTS character_stat (
    strength SMALLINT,
    stamina SMALLINT,
    dexterity SMALLINT,
    constitution SMALLINT,
    intelligence SMALLINT,
    wisdom SMALLINT,
    id UUID PRIMARY KEY,
);

CREATE TABLE IF NOT EXISTS hp (
    hp FLOAT,
    max_hp FLOAT,
    id UUID PRIMARY KEY,
);

CREATE TABLE IF NOT EXISTS mp (
    mp SMALLINT,
    max_mp SMALLINT,
    id UUID PRIMARY KEY,
);

CREATE TABLE IF NOT EXISTS perk (
    perk TEXT,
    PRIMARY KEY(id, perk),
    id UUID,
);
