﻿using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Core.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace Rocket.Unturned.Events
{
    public sealed class UnturnedEvents : MonoBehaviour, IRocketImplementationEvents
    {
        private static UnturnedEvents Instance;
        private void Awake()
        {
            Instance = this;
            Provider.onServerDisconnected += (CSteamID r) =>
            {
                if (r != CSteamID.Nil)
                {
                    OnPlayerDisconnected.TryInvoke(UnturnedPlayer.FromCSteamID(r));
                }
            };
            Provider.onServerShutdown += () => { OnShutdown.TryInvoke(); };
            Provider.onServerConnected += (CSteamID r) =>
            {
                if (r != CSteamID.Nil)
                {
                    UnturnedPlayer p = UnturnedPlayer.FromCSteamID(r);
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerFeatures>();
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerMovement>();
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerEvents>();
                    OnBeforePlayerConnected.TryInvoke(p);
                }
            };
            DamageTool.playerDamaged += (SDG.Unturned.Player player, ref EDeathCause cause, ref ELimb limb, ref CSteamID killer, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage) =>
            {
                if (OnPlayerDamaged != null)
                {
                    if (player != null && killer != CSteamID.Nil && killer != null)
                    {
                        UnturnedPlayer getterDamage = UnturnedPlayer.FromPlayer(player);
                        UnturnedPlayer senderDamage = UnturnedPlayer.FromCSteamID(killer);
                        OnPlayerDamaged.TryInvoke(getterDamage, cause, limb, senderDamage, direction, damage, times, canDamage);
                    }
                }
            };
        }

        public delegate void PlayerDisconnected(UnturnedPlayer player);
        public event PlayerDisconnected OnPlayerDisconnected;

        public delegate void OnPlayerGetDamage(UnturnedPlayer player, ref EDeathCause cause, ref ELimb limb, ref UnturnedPlayer killer, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage);
        public static event OnPlayerGetDamage OnPlayerDamaged;

        public event ImplementationShutdown OnShutdown;

        internal static void TriggerOnPlayerConnected(UnturnedPlayer player)
        {
            Instance.OnPlayerConnected.TryInvoke(player);
        }

        public delegate void PlayerConnected(UnturnedPlayer player);
        public event PlayerConnected OnPlayerConnected;
        public event PlayerConnected OnBeforePlayerConnected;
    }
}