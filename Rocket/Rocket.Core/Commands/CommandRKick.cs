using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.RCON;
using System.Collections.Generic;

namespace Rocket.Core.Commands
{
    internal class CommandRKick : IRocketCommand
    {
        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Console;

        public string Help => "Kicks a client off of RCON.";

        public string Name => "rkick";

        public List<string> Permissions => new List<string>() { "rocket.rkick" };

        public string Syntax => "<ConnectionID>";

        public void Execute(IRocketPlayer caller, string[] command)
        {
            int? instance = command.GetInt32Parameter(0);
            if (command.Length == 0 || command.Length > 1 || instance == null)
            {
                Logger.Log(R.Translate("command_rkick_help"));
                return;
            }
            foreach (RCONConnection client in RCONServer.Clients)
            {
                if (client.InstanceID == instance)
                {
                    Logger.Log(R.Translate("command_rkick_kicked", instance.ToString(), client.Address));
                    client.Close();
                    return;
                }
            }
            Logger.Log(R.Translate("command_rkick_notfound", instance.ToString()));
        }
    }
}
