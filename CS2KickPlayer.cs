using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.ValveConstants.Protobuf;
using Microsoft.Extensions.Logging;

namespace CS2KickPlayer
{
    public class KickPlugin : BasePlugin
    {
        public override string ModuleName => "CS2KickPlayer";
        public override string ModuleVersion => "1.0.0";
        public override string ModuleAuthor => "kanoah";
        public override string ModuleDescription => "Adds a command to kick players.";

        public override void Load(bool HotReload)
        {
            Logger.LogInformation("{ModuleName} v{ModuleVersion} by {ModuleAuthor} has been loaded.", ModuleName, ModuleVersion, ModuleAuthor);
        }

        [RequiresPermissions("@css/kick")]
        [ConsoleCommand("css_kick", "kicks a player. NOW INCLUDING: partial name searching!")]
        [CommandHelper(minArgs: 1, whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        public void OnCommandKick(CCSPlayerController? player, CommandInfo info)
        {
            if (player == null)
            {
                return;
            }

            string search = info.ArgString.Trim().ToLower();

            var matches = Utilities.GetPlayers()
                .Where(p => p != null && p.PlayerName.Contains(search, StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            if (matches.Count == 0)
            {
                info.ReplyToCommand($"No players found matching '{search}'.");
                return;
            }

            if (matches.Count > 1)
            {
                info.ReplyToCommand("Multiple matches found:");
                foreach (var m in matches)
                    info.ReplyToCommand($"- {m.PlayerName} [{m.SteamID}]");
                return;
            }

            var target = matches.First();
            target.Disconnect(NetworkDisconnectionReason.NETWORK_DISCONNECT_STEAM_VACBANSTATE);
            info.ReplyToCommand($" Kicked {ChatColors.Gold}{target.PlayerName} {ChatColors.Default}[{ChatColors.Green}{target.SteamID}{ChatColors.Default}]");
        }
    }
}
