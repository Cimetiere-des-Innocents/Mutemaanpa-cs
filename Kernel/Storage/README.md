# Storage subsystem

As a video game, mutamaanpa must support save functionality. This subsystem persists user data
on disk reliably.

## Metadata

Mutemaanpa has three kind of metadata, they are:

- User settings
    Example: sound volume, language
- Save file
    Example: how many saves do we have, ...
- Game configs
    Example: mod & plugins installed, game version

Other parts of the code must not know how these data are persisted. They just get these data from
somewhere.

In reality, we save 1 and 3 to json and 2 to a relational table.

## Session

Player click "new game" to create a new game world. We call they created a session. Session is saved
in a relational DBMS. To provide forward compatibility we have made a migration framework.
