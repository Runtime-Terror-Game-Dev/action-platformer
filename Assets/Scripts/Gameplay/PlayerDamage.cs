using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;
using System.Threading;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player has died.
    /// </summary>
    /// <typeparam name="PlayerDamage"></typeparam>
    public class PlayerDamage : Simulation.Event<PlayerDamage>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;
            if (player.health.IsAlive)
            {
                Debug.Log("PlayerDamage");
                player.damagedState = "Impact";
                player.playerrigidbody.AddForce(new Vector2(1000f, 1000f), ForceMode2D.Impulse);
                player.DamageFlashTimer = player.DamageFlashDuration;
            }
        }
    }
}