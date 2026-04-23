#nullable enable
using System;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace ForceTeam;

public class ForceTeamPlugin : BasePlugin
{
    public override string ModuleName => "ForceTeam";
    public override string ModuleVersion => "1.0.0";

    public override void Load(bool hotReload)
    {
        AddCommandListener("jointeam", OnJoinTeamCommand);
    }

    private HookResult OnJoinTeamCommand(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.IsBot)
            return HookResult.Continue;

        if (!TryGetRequestedTeam(info, out var requestedTeam))
            return HookResult.Continue;

        if (requestedTeam == CsTeam.Spectator)
            return HookResult.Continue;

        // Only balance a player's first move from unassigned/spectator into a team.
        if (player.Team is CsTeam.Terrorist or CsTeam.CounterTerrorist)
            return HookResult.Continue;

        var assignedTeam = GetTeamWithFewerPlayers(player);

        // Preserve normal jointeam behavior for initial team assignment.
        Server.PrintToConsole($"[ForceTeam] {player.PlayerName} assigned to {assignedTeam}");
        player.ChangeTeam(assignedTeam);
        return HookResult.Handled;
    }

    private static bool TryGetRequestedTeam(CommandInfo info, out CsTeam requestedTeam)
    {
        requestedTeam = CsTeam.None;

        if (info.ArgCount < 2)
            return false;

        return info.ArgByIndex(1) switch
        {
            "1" => Assign(CsTeam.Spectator, out requestedTeam),
            "2" => Assign(CsTeam.Terrorist, out requestedTeam),
            "3" => Assign(CsTeam.CounterTerrorist, out requestedTeam),
            _ => false
        };
    }

    private static bool Assign(CsTeam team, out CsTeam requestedTeam)
    {
        requestedTeam = team;
        return true;
    }

    private CsTeam GetTeamWithFewerPlayers(CCSPlayerController joiner)
    {
        int tCount = 0, ctCount = 0;

        foreach (var p in Utilities.GetPlayers())
        {
            if (!p.IsValid || p.IsBot || p == joiner) continue;
            if (p.Team == CsTeam.Terrorist) tCount++;
            else if (p.Team == CsTeam.CounterTerrorist) ctCount++;
        }

        Server.PrintToConsole($"[ForceTeam] Team counts - T:{tCount} CT:{ctCount}");

        // If teams are equal, pick randomly
        if (tCount == ctCount)
            return Random.Shared.Next(2) == 0 ? CsTeam.Terrorist : CsTeam.CounterTerrorist;

        return tCount < ctCount ? CsTeam.Terrorist : CsTeam.CounterTerrorist;
    }
}
