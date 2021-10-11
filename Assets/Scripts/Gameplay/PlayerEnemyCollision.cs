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
        public Hitbox hitbox;
        //Collider 2D can be any collision , producing errors, so change to Hitbox script

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        //TODO fix bug where killing an enemy with health script also kills player
        public override void Execute()
        {
            if (hitbox != null)
            {
                var enemyHealth = enemy.GetComponent<Health>();
                Schedule<EnemyDamage>().enemy = enemy;
                if (enemyHealth != null)
                {
                    enemyHealth.Decrement();
                    if (!enemyHealth.IsAlive)
                    {
                        Schedule<EnemyDeath>().enemy = enemy;
                    }
                }
                else
                {
                    Schedule<EnemyDeath>().enemy = enemy;
                }
            }
            if (player != null)
            {
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