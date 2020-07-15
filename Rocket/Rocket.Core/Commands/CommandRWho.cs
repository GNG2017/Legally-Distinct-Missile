using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.RCON;
using System;
using System.Collections.Generic;

namespace Rocket.Core.Commands
{
    internal class CommandRWho : IRocketCommand
    {
        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Console;

        public string Help => "Returns a list of clients connected to RCON.";

        public string Name => "rwho";

        public List<string> Permissions => new List<string>() { "rocket.rwho" };

        public string Syntax => "";

        public void Execute(IRocketPlayer caller, string[] command)
        {
            for (int i = 0; i < RCONServer.Clients.Count; i++)
            {
                RCONConnection client = RCONServer.Clients[i];
                int timeTotal = (int)((DateTime.Now - client.ConnectedTime).TotalSeconds);
                string connectedTimeFormat = "";

                // Format days, hours minutes and seconds since the client connected to RCON.
                if (timeTotal >= (60 * 60 * 24))
                {
                    connectedTimeFormat = (timeTotal / (60 * 60 * 24)).ToString() + "d ";
                }
                if (timeTotal >= (60 * 60))
                {
                    connectedTimeFormat += ((timeTotal / (60 * 60)) % 24).ToString() + "h ";
                }
                if (timeTotal >= 60)
                {
                    connectedTimeFormat += ((timeTotal / 60) % 60).ToString() + "m ";
                }
                connectedTimeFormat += (timeTotal % 60).ToString() + "s";
                Logger.Log(R.Translate("command_rwho_line", i + 1, client.InstanceID, client.Authenticated, client.Address, client.ConnectedTime.ToString(), connectedTimeFormat));
            }
        }
    }
}
