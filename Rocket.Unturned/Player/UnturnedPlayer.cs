using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Steam;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Skills;
using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;
using UnityEngine;

namespace Rocket.Unturned.Player
{
    public class PlayerIsConsoleException : Exception { }

    public sealed class UnturnedPlayer : IRocketPlayer
    {

        public string Id => CSteamID.ToString();

        public string DisplayName => CharacterName;

        public bool IsAdmin => Player.channel.owner.isAdmin;

        public Profile SteamProfile => new Profile(ulong.Parse(CSteamID.ToString()));

        public SDG.Unturned.Player Player { get; }

        public CSteamID CSteamID => Player.channel.owner.playerID.steamID;

        public Exception PlayerIsConsoleException;

        private UnturnedPlayer(SteamPlayer player)
        {
            Player = player.player;
        }

        public Color Color
        {
            get
            {
                if (Features.Color.HasValue)
                {
                    return Features.Color.Value;
                }

                if (IsAdmin && !Provider.hideAdmins)
                {
                    return Palette.ADMIN;
                }

                RocketPermissionsGroup group = R.Permissions.GetGroups(this, false).Where(g => g.Color != null && g.Color != "white").FirstOrDefault();
                string color = "";
                if (group != null)
                {
                    color = group.Color;
                }

                return UnturnedChat.GetColorFromName(color, Palette.COLOR_W);
            }
            set => Features.Color = value;
        }


        private UnturnedPlayer(CSteamID cSteamID)
        {
            if (string.IsNullOrEmpty(cSteamID.ToString()) || cSteamID.ToString() == "0")
            {
                throw new PlayerIsConsoleException();
            }
            else
            {
                Player = PlayerTool.getPlayer(cSteamID);
            }
        }

        public float Ping => Player.channel.owner.ping;

        public bool Equals(UnturnedPlayer otherPlayer)
        {
            return !(otherPlayer is null) && CSteamID == otherPlayer.CSteamID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UnturnedPlayer);
        }

        public override int GetHashCode()
        {
            return CSteamID.GetHashCode();
        }

        public T GetComponent<T>()
        {
            return (T)(object)Player.GetComponent(typeof(T));
        }

        private UnturnedPlayer(SDG.Unturned.Player p)
        {
            Player = p;
        }

        public static UnturnedPlayer FromName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            SDG.Unturned.Player p;
            if (ulong.TryParse(name, out ulong id) && id > 76561197960265728)
            {
                p = PlayerTool.getPlayer(new CSteamID(id));
            }
            else
            {
                p = PlayerTool.getPlayer(name);
            }

            return p == null ? null : new UnturnedPlayer(p);
        }

        public static UnturnedPlayer FromCSteamID(CSteamID cSteamID)
        {
            return string.IsNullOrEmpty(cSteamID.ToString()) || cSteamID.ToString() == "0" ? null : new UnturnedPlayer(cSteamID);
        }

        public static UnturnedPlayer FromPlayer(SDG.Unturned.Player player)
        {
            return new UnturnedPlayer(player.channel.owner);
        }

        public static UnturnedPlayer FromSteamPlayer(SteamPlayer player)
        {
            return new UnturnedPlayer(player);
        }

        public UnturnedPlayerFeatures Features => Player.gameObject.transform.GetComponent<UnturnedPlayerFeatures>();

        public UnturnedPlayerEvents Events => Player.gameObject.transform.GetComponent<UnturnedPlayerEvents>();

        public override string ToString()
        {
            return CSteamID.ToString();
        }

        public void TriggerEffect(ushort effectID)
        {
            EffectManager.instance.channel.send("tellEffectPoint", CSteamID, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] { effectID, Player.transform.position });
        }

        public string IP => SteamGameServerNetworking.GetP2PSessionState(CSteamID, out P2PSessionState_t State) ? Parser.getIPFromUInt32(State.m_nRemoteIP) : null;

        public void MaxSkills()
        {
            PlayerSkills skills = Player.skills;

            foreach (Skill skill in skills.skills.SelectMany(s => s))
            {
                skill.level = skill.max;
            }

            skills.askSkills(Player.channel.owner.playerID.steamID);
        }

        public string SteamGroupName()
        {
            FriendsGroupID_t id;
            id.m_FriendsGroupID = (short)SteamGroupID.m_SteamID;
            return SteamFriends.GetFriendsGroupName(id);
        }

        public int SteamGroupMembersCount()
        {
            FriendsGroupID_t id;
            id.m_FriendsGroupID = (short)SteamGroupID.m_SteamID;
            return SteamFriends.GetFriendsGroupMembersCount(id);
        }

        public SteamPlayer SteamPlayer()
        {
            foreach (SteamPlayer SteamPlayer in Provider.clients)
            {
                if (CSteamID == SteamPlayer.playerID.steamID)
                {
                    return SteamPlayer;
                }
            }

            return null;
        }

        public PlayerInventory Inventory => Player.inventory;

        public bool GiveItem(ushort itemId, byte amount)
        {
            return ItemTool.tryForceGiveItem(Player, itemId, amount);
        }

        public bool GiveItem(Item item)
        {
            return Player.inventory.tryAddItem(item, false);
        }

        public bool GiveVehicle(ushort vehicleId)
        {
            return VehicleTool.giveVehicle(Player, vehicleId);
        }

        public CSteamID SteamGroupID => Player.channel.owner.playerID.group;

        public void Kick(string reason)
        {
            Provider.kick(CSteamID, reason);
        }

        public void Ban(string reason, uint duration)
        {
            Ban(CSteamID.Nil, reason, duration);
        }

        public void Ban(CSteamID instigator, string reason, uint duration)
        {
            CSteamID steamIdToBan = CSteamID;

            uint ipToBan = 0;
            if (SteamGameServerNetworking.GetP2PSessionState(steamIdToBan, out P2PSessionState_t state))
            {
                ipToBan = state.m_nRemoteIP;
            }

            Provider.requestBanPlayer(instigator, steamIdToBan, ipToBan, reason, duration);
        }

        public void Admin(bool admin)
        {
            Admin(admin, null);
        }

        public void Admin(bool admin, UnturnedPlayer issuer)
        {
            if (admin)
            {
                if (issuer == null)
                {
                    SteamAdminlist.admin(CSteamID, new CSteamID(0));
                }
                else
                {
                    SteamAdminlist.admin(CSteamID, issuer.CSteamID);
                }
            }
            else
            {
                SteamAdminlist.unadmin(Player.channel.owner.playerID.steamID);
            }
        }

        public void Teleport(UnturnedPlayer target)
        {
            Vector3 d1 = target.Player.transform.position;
            Vector3 vector31 = target.Player.transform.rotation.eulerAngles;
            Teleport(d1, MeasurementTool.angleToByte(vector31.y));
        }

        public void Teleport(Vector3 position, float rotation)
        {
            if (VanishMode)
            {
                Player.channel.send("askTeleport", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
                Player.channel.send("askTeleport", ESteamCall.NOT_OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new Vector3(position.y, position.y + 1337, position.z), MeasurementTool.angleToByte(rotation));
                Player.channel.send("askTeleport", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
            }
            else
            {
                Player.teleportToLocation(position, rotation);
            }
        }

        public bool VanishMode
        {
            get => Player.GetComponent<UnturnedPlayerFeatures>().VanishMode;
            set => Player.GetComponent<UnturnedPlayerFeatures>().VanishMode = value;
        }

        public bool GodMode
        {
            get => Player.GetComponent<UnturnedPlayerFeatures>().GodMode;
            set => Player.GetComponent<UnturnedPlayerFeatures>().GodMode = value;
        }

        public Vector3 Position => Player.transform.position;

        public EPlayerStance Stance => Player.stance.stance;

        public float Rotation => Player.transform.rotation.eulerAngles.y;

        public bool Teleport(string nodeName)
        {
            Node node = LevelNodes.nodes.Where(n => n.type == ENodeType.LOCATION && ((LocationNode)n).name.ToLower().Contains(nodeName)).FirstOrDefault();
            if (node != null)
            {
                Vector3 c = node.point + new Vector3(0f, 0.5f, 0f);
                Player.sendTeleport(c, MeasurementTool.angleToByte(Rotation));
                return true;
            }
            return false;
        }

        public byte Stamina => Player.life.stamina;

        public string CharacterName => Player.channel.owner.playerID.characterName;

        public string SteamName => Player.channel.owner.playerID.playerName;

        public byte Infection
        {
            get => Player.life.virus;
            set
            {
                Player.life.askDisinfect(100);
                Player.life.askInfect(value);
            }
        }

        public uint Experience
        {
            get => Player.skills.experience;
            set
            {
                Player.skills.channel.send("tellExperience", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
                Player.skills.channel.send("tellExperience", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
            }
        }

        public int Reputation
        {
            get => Player.skills.reputation;
            set => Player.skills.askRep(value);
        }

        public byte Health => Player.life.health;

        public byte Hunger
        {
            get => Player.life.food;
            set
            {
                Player.life.askEat(100);
                Player.life.askStarve(value);
            }
        }

        public byte Thirst
        {
            get => Player.life.water;
            set
            {
                Player.life.askDrink(100);
                Player.life.askDehydrate(value);
            }
        }

        public bool Broken
        {
            get => Player.life.isBroken;
            set
            {
                Player.life.tellBroken(Provider.server, value);
                Player.life.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] { value });
            }
        }
        public bool Bleeding
        {
            get => Player.life.isBleeding;
            set
            {
                Player.life.tellBleeding(Provider.server, value);
                Player.life.channel.send("tellBleeding", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] { value });
            }
        }

        public bool Dead => Player.life.isDead;

        public void Heal(byte amount)
        {
            Heal(amount, null, null);
        }

        public void Heal(byte amount, bool? bleeding, bool? broken)
        {
            Player.life.askHeal(amount, bleeding != null ? bleeding.Value : Player.life.isBleeding, broken != null ? broken.Value : Player.life.isBroken);
        }

        public void Suicide()
        {
            Player.life.askSuicide(Player.channel.owner.playerID.steamID);
        }

        public EPlayerKill Damage(byte amount, Vector3 direction, EDeathCause cause, ELimb limb, CSteamID damageDealer)
        {
            Player.life.askDamage(amount, direction, cause, limb, damageDealer, out EPlayerKill playerKill);
            return playerKill;
        }

        public bool IsPro => Player.channel.owner.isPro;

        public InteractableVehicle CurrentVehicle => Player.movement.getVehicle();

        public bool IsInVehicle => CurrentVehicle != null;

        public void SetSkillLevel(UnturnedSkill skill, byte level)
        {
            GetSkill(skill).level = level;
            Player.skills.askSkills(CSteamID);
        }

        public byte GetSkillLevel(UnturnedSkill skill)
        {
            return GetSkill(skill).level;
        }

        public Skill GetSkill(UnturnedSkill skill)
        {
            PlayerSkills skills = Player.skills;
            return skills.skills[skill.Speciality][skill.Skill];
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(obj);
        }
    }
}
