﻿using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Unturned.Commands
{
    public class CommandHelp : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "help";

        public string Help => "Shows you a specific help";

        public string Syntax => "[command]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.help" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, "[Vanilla]");
                Commander.commands.OrderBy(c => c.command).All(c => { UnturnedChat.Say(caller, c.command.ToLower().PadRight(20, ' ') + " " + c.info.Replace(c.command, "").TrimStart().ToLower()); return true; });

                UnturnedChat.Say(caller, "---");

                UnturnedChat.Say(caller, "[Rocket]");
                R.Commands.Commands.Where(c => c.GetType().Assembly == Assembly.GetExecutingAssembly()).OrderBy(c => c.Name).All(c => { UnturnedChat.Say(caller, c.Name.ToLower().PadRight(20, ' ') + " " + c.Syntax.ToLower()); return true; });

                UnturnedChat.Say(caller, "---");

                foreach (IRocketPlugin plugin in R.Plugins.GetPlugins())
                {
                    UnturnedChat.Say(caller, "[" + plugin.GetType().Assembly.GetName().Name + "]");
                    R.Commands.Commands.Where(c => c.GetType().Assembly == plugin.GetType().Assembly).OrderBy(c => c.Name).All(c => { UnturnedChat.Say(caller, c.Name.ToLower().PadRight(20, ' ') + " " + c.Syntax.ToLower()); return true; });
                    UnturnedChat.Say(caller, "---");
                }
            }
            else
            {
                IRocketCommand cmd = R.Commands.Commands.Where(c => (string.Compare(c.Name, command[0], true) == 0)).FirstOrDefault();
                if (cmd != null)
                {
                    string commandName = cmd.GetType().Assembly.GetName().Name + " / " + cmd.Name;

                    UnturnedChat.Say(caller, "[" + commandName + "]");
                    UnturnedChat.Say(caller, cmd.Name + "\t\t" + cmd.Syntax);
                    UnturnedChat.Say(caller, cmd.Help);
                }
            }
        }
    }
}