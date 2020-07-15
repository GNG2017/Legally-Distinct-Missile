using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.RCON;
using System.Collections.Generic;

namespace Rocket.Core.Commands
{
    internal class CommandRFlush : IRocketCommand
    {
        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Console;

        public string Help => "Kicks all RCON clients off of the server.";

        public string Name => "rflush";

        public List<string> Permissions => new List<string>() { "rocket.rflush" };

        public string Syntax => "";

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0 || command.Length > 1 || command[0] != "y")
            {
                Logger.Log(R.Translate("command_rflush_help"));
            }
            else
            {
                Logger.Log(R.Translate("command_rflush_total", RCONServer.Clients.Count.ToString()));
                List<RCONConnection> connections = new List<RCONConnection>();
                connections.AddRange(RCONServer.Clients);
                for (int i = 0; i < connections.Count; i++)
                {
                    RCONConnection client = connections[i];
                    if (client.Client.Client.Connected)
                    {
                        Logger.Log(R.Translate("command_rflush_line", i + 1, client.InstanceID, client.Address));
                        client.Close();
                    }
                }
            }
        }
    }
}
