# CS2 Force Team Join

Small CounterStrikeSharp plugin for Counter-Strike 2 that forces players onto the team with fewer players when they join a side.

## Behavior

- Players joining Terrorist or Counter-Terrorist are redirected to the smaller team.
- Players can still join Spectator normally.
- Existing team members are not forcibly swapped mid-round.

## Installation

1. Build or download `ForceTeamJoin.dll`.
2. Place the DLL in your server's `addons/counterstrikesharp/plugins/ForceTeamJoin` folder.
3. Restart the server or reload CounterStrikeSharp plugins.

## Release

Tagged releases publish the built plugin through GitHub Actions.
