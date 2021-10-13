using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;
using System.Threading;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player takes damage.
    /// </summary>
    /// <typeparam name="PlayerDamage"></typeparam>
    public class PlayerDamage : Simulation.Event<PlayerDamage>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            Debug.Log("player damage");
            var player = model.player;
            if (player.health.IsAlive)
            {
                player.damagedState = "Impact";
                player.audioSource.PlayOneShot(player.ouchAudio);
                player.DamageFlashTimer = player.DamageFlashDuration;
            }
        }
    }
}