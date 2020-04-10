# SCPSLCustomSpawnpoints

## It's a bit of a confusing plugin to use, and is very easy to break, so read carefully!

## Do not edit the config files directly without knowing what you are doing, as it could break the plugin.

## Commands

### `addsp`

adds a spawnpoint to the database (`spawnpoint_db.txt`)

Usage:
`addsp <list | debug | (id from list)> [(name of spawnpoint)]`
`debug` is for debugging, so don't use it as it will spawn MicroHIDs at every valid spawnpoint.
`list` will list all the room id's
`listsp` will list all the spawnpoint data

How it works:
`addsp 173 173spawnpoint` will add a spawnpoint named `173spawnpoint` relative to my location of the 173 spawn room.

### `addis`

adds an item to spawn at that spawnpoint (`itemspawns.txt`)
https://github.com/Grover-c13/Smod2/wiki/Enum-Lists#itemtype

Usage:
`addis <spawnpoint name> [item id]`

How it works:
`addis 173spawnpoint` will remove ALL items listed under `173spawnpoint`
`addis 173spawnpoint 0` will remove/add the janitor keycard from spawning at the spawnpoint `173spawnpoint`