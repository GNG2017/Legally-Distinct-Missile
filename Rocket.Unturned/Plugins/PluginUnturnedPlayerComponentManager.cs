using Rocket.API;
using Rocket.Core.Utils;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Rocket.Unturned.Plugins
{
    public sealed class PluginUnturnedPlayerComponentManager : MonoBehaviour
    {
        private Assembly _assembly;
        private readonly List<Type> _unturnedPlayerComponents = new List<Type>();

        #region Start & Stop
        private void OnEnable()
        {
            try
            {
                IRocketPlugin plugin = GetComponent<IRocketPlugin>();
                _assembly = plugin.GetType().Assembly;


                //This is called before 'OnBeforePlayerConnected'
                _unturnedPlayerComponents.AddRange(RocketHelper.GetTypesFromParentClass(_assembly, typeof(UnturnedPlayerComponent)));
                Provider.onEnemyConnected += OnConnect;


                //Apply to existing players
                foreach (SteamPlayer client in Provider.clients)
                {
                    GiveAllComponents(client.player.gameObject);
                }
            }
            catch (Exception ex)
            {
                Core.Logging.Logger.LogException(ex);
            }
        }

        private void OnDisable()
        {
            try
            {
                Provider.onEnemyConnected -= OnConnect;


                //Remove from existing players
                foreach (Component comp in Provider.clients.SelectMany(client => _unturnedPlayerComponents.Select(playerComponent => client.player.gameObject.GetComponent(playerComponent)).Where(comp => comp != null)))
                {
                    Destroy(comp);
                }


                _unturnedPlayerComponents.Clear();
                _assembly = null;
            }
            catch (Exception ex)
            {
                Core.Logging.Logger.LogException(ex);
            }
        }
        #endregion

        #region Events
        private void OnConnect(SteamPlayer steamPlayer)
        {
            GiveAllComponents(steamPlayer.player.gameObject);
        }
        #endregion

        #region Functions
        private void GiveAllComponents(GameObject playerGameObject)
        {
            foreach (Type playerComponent in _unturnedPlayerComponents.Where(comp => playerGameObject.GetComponent(comp) == null))
            {
                playerGameObject.AddComponent(playerComponent);
            }
        }
        #endregion
    }
}