using Rocket.API;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Unturned.Commands
{
    public class CommandRocket : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "rocket";

        public string Help => "Reloading Rocket or individual plugins";

        public string Syntax => "<plugins | reload> | <reload | unload | load> <plugin>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.info", "rocket.rocket" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, "Rocket v" + Assembly.GetExecutingAssembly().GetName().Version + " for Unturned v" + Provider.APP_VERSION);
                UnturnedChat.Say(caller, "https://rocketmod.net - 2017");
                return;
            }

            if (command.Length == 1)
            {
                switch (command[0].ToLower())
                {
                    case "plugins":
                        if (caller != null && !caller.HasPermission("rocket.plugins"))
                        {
                            return;
                        }

                        List<IRocketPlugin> plugins = R.Plugins.GetPlugins();
                        UnturnedChat.Say(caller, U.Translate("command_rocket_plugins_loaded", string.Join(", ", plugins.Where(p => p.State == PluginState.Loaded).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        UnturnedChat.Say(caller, U.Translate("command_rocket_plugins_unloaded", string.Join(", ", plugins.Where(p => p.State == PluginState.Unloaded).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        UnturnedChat.Say(caller, U.Translate("command_rocket_plugins_failure", string.Join(", ", plugins.Where(p => p.State == PluginState.Failure).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        UnturnedChat.Say(caller, U.Translate("command_rocket_plugins_cancelled", string.Join(", ", plugins.Where(p => p.State == PluginState.Cancelled).Select(p => p.GetType().Assembly.GetName().Name).ToArray())));
                        break;
                    case "reload":
                        if (caller != null && !caller.HasPermission("rocket.reload"))
                        {
                            return;
                        }
                        // Many plugins do not support reloading properly, so this command which reloaded all plugins
                        // at once has been disabled by popular request. Reloading individual plugins is still enabled.
                        // https://github.com/SmartlyDressedGames/Unturned-3.x-Community/issues/1794
                        UnturnedChat.Say(caller, U.Translate("command_rocket_reload_disabled"));
                        break;
                }
            }

            if (command.Length == 2)
            {
                RocketPlugin p = (RocketPlugin)R.Plugins.GetPlugins().Where(pl => pl.Name.ToLower().Contains(command[1].ToLower())).FirstOrDefault();
                if (p != null)
                {
                    switch (command[0].ToLower())
                    {
                        case "reload":
                            if (caller != null && !caller.HasPermission("rocket.reloadplugin"))
                            {
                                return;
                            }

                            if (p.State == PluginState.Loaded)
                            {
                                UnturnedChat.Say(caller, U.Translate("command_rocket_reload_plugin", p.GetType().Assembly.GetName().Name));
                                p.ReloadPlugin();
                            }
                            else
                            {
                                UnturnedChat.Say(caller, U.Translate("command_rocket_not_loaded", p.GetType().Assembly.GetName().Name));
                            }
                            break;
                        case "unload":
                            if (caller != null && !caller.HasPermission("rocket.unloadplugin"))
                            {
                                return;
                            }

                            if (p.State == PluginState.Loaded)
                            {
                                UnturnedChat.Say(caller, U.Translate("command_rocket_unload_plugin", p.GetType().Assembly.GetName().Name));
                                p.UnloadPlugin();
                            }
                            else
                            {
                                UnturnedChat.Say(caller, U.Translate("command_rocket_not_loaded", p.GetType().Assembly.GetName().Name));
                            }
                            break;
                        case "load":
                            if (caller != null && !caller.HasPermission("rocket.loadplugin"))
                            {
                                return;
                            }

                            if (p.State != PluginState.Loaded)
                            {
                                UnturnedChat.Say(caller, U.Translate("command_rocket_load_plugin", p.GetType().Assembly.GetName().Name));
                                p.LoadPlugin();
                            }
                            else
                            {
                                UnturnedChat.Say(caller, U.Translate("command_rocket_already_loaded", p.GetType().Assembly.GetName().Name));
                            }
                            break;
                    }
                }
                else
                {
                    UnturnedChat.Say(caller, U.Translate("command_rocket_plugin_not_found", command[1]));
                }
            }


        }
    }
}
