using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core.Assets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rocket.Core.Permissions
{
    public sealed class RocketPermissionsManager : MonoBehaviour, IRocketPermissionsProvider
    {
        private RocketPermissionsHelper helper;

        private void Start()
        {
            try
            {
                if (R.Settings.Instance.WebPermissions.Enabled)
                {
                    lastWebPermissionsUpdate = DateTime.Now;
                    helper = new RocketPermissionsHelper(new WebXMLFileAsset<RocketPermissions>(new Uri(R.Settings.Instance.WebPermissions.Url + "?instance=" + R.Implementation.InstanceId)));
                    updateWebPermissions = true;
                }
                else
                {
                    helper = new RocketPermissionsHelper(new XMLFileAsset<RocketPermissions>(Environment.PermissionFile));
                }

            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
            }
        }

        private bool updateWebPermissions = false;
        private DateTime lastWebPermissionsUpdate;

        private void FixedUpdate()
        {
            try
            {
                if (updateWebPermissions && R.Settings.Instance.WebPermissions.Interval > 0 && (DateTime.Now - lastWebPermissionsUpdate) > TimeSpan.FromSeconds(R.Settings.Instance.WebPermissions.Interval))
                {
                    lastWebPermissionsUpdate = DateTime.Now;
                    updateWebPermissions = false;
                    helper.permissions.Load((IAsset<RocketPermissions> asset) =>
                    {
                        updateWebPermissions = true;
                    });
                }
            }

            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
            }
        }

        public void Reload() => helper.permissions.Load();

        public bool HasPermission(IRocketPlayer player, List<string> permissions) => helper.HasPermission(player, permissions);

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups) => helper.GetGroups(player, includeParentGroups);

        public List<Permission> GetPermissions(IRocketPlayer player) => helper.GetPermissions(player);

        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions) => helper.GetPermissions(player, requestedPermissions);

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player) => helper.AddPlayerToGroup(groupId, player);

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player) => helper.RemovePlayerFromGroup(groupId, player);

        public RocketPermissionsGroup GetGroup(string groupId) => helper.GetGroup(groupId);

        public RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group) => helper.SaveGroup(group);

        public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group) => helper.AddGroup(group);

        public RocketPermissionsProviderResult DeleteGroup(RocketPermissionsGroup group) => helper.DeleteGroup(group.Id);

        public RocketPermissionsProviderResult DeleteGroup(string groupId) => helper.DeleteGroup(groupId);
    }
}