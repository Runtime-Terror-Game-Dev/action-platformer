using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;
using System.Collections;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the health component on an enemy has a hitpoint value of  0.
    /// </summary>
    /// <typeparam name="EnemyDamage"></typeparam>
    public class EnemyDamage : Simulation.Event<EnemyDamage>
    {
        public EnemyController enemy;

        public override void Execute()
        {
            Debug.Log("enemy damage");
            if (enemy._audio && enemy.ouch)
                enemy._audio.PlayOneShot(enemy.ouch);
            enemy.control.enabled = false;
            enemy.damagedState = "Impact";
            enemy.DamageFlashTimer = enemy.DamageFlashDuration;
        }
    }
}