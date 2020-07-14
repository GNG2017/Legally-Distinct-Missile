using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using UnityEngine;

namespace Rocket.Unturned
{
    public class UnturnedPlayerMovement : UnturnedPlayerComponent
    {
        public bool VanishMode = false;
        private DateTime lastUpdate = DateTime.Now;
        private Vector3 lastVector = new Vector3(0, -1, 0);

        private void FixedUpdate()
        {
            PlayerMovement movement = Player.GetComponent<PlayerMovement>();

            if (!VanishMode)
            {
                if (U.Settings.Instance.LogSuspiciousPlayerMovement && lastUpdate.AddSeconds(1) < DateTime.Now)
                {
                    lastUpdate = DateTime.Now;

                    Vector3 positon = movement.real;

                    if (lastVector.y != -1)
                    {
                        float y = positon.y - lastVector.y;
                        if (y > 15)
                        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                            RaycastHit raycastHit = new RaycastHit();
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                            Physics.Raycast(positon, Vector3.down, out raycastHit);
                            Vector3 floor = raycastHit.point;
                            float distance = Math.Abs(floor.y - positon.y);
                            Core.Logging.Logger.Log(Player.DisplayName + " moved x:" + positon.x + " y:" + positon.y + "(+" + y + ") z:" + positon.z + " in the last second (" + distance + ")");
                        }
                    }
                    lastVector = movement.real;
                }
            }
        }
    }
}
