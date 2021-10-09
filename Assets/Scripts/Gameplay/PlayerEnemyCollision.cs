using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Simulation.Event<PlayerEnemyCollision>
    {
        public EnemyController enemy;
        public PlayerController player;
        public Collider2D hitbox;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        //TODO fix bug where killing an enemy with health script also kills player
        public override void Execute()
        {
            if (hitbox != null)
            {
                var enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.Decrement();
                    if (!enemyHealth.IsAlive)
                    {
                        Schedule<EnemyDeath>().enemy = enemy;
                    }
                }
                // else
                // {
                //     Schedule<EnemyDeath>().enemy = enemy;
                // }
                Debug.Log("willHurtEnemy = true");
            }
            else //we assume player ran into enemy
            {
                Debug.Log("willHurtEnemey = false");
                var playerHealth = player.GetComponent<Health>();
                if (playerHealth != null) //always true
                {
                    if (playerHealth.currentHP == 1) //last hit
                    {
                        Schedule<PlayerDeath>();
                    }
                    else
                    {
                        playerHealth.Decrement();
                        Schedule<PlayerDamage>();
                    }
                }
            }
        }
    }
}