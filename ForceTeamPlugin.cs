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
    public override string ModuleVersion => "0.1.0";

    public override void Load(bool hotReload)
    {
        AddCommandListener("jointeam", OnJoinTeamCommand);
    }

    private HookResult OnJoinTeamCommand(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.IsBot)
            return HookResult.Continue;

        var assignedTeam = GetTeamWithFewerPlayers(player);

        // Always force the balanced team, regardless of what they picked
        Server.PrintToConsole($"[ForceTeam] {player.PlayerName} assigned to {assignedTeam}");
        player.SwitchTeam(assignedTeam);
        return HookResult.Handled;
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
