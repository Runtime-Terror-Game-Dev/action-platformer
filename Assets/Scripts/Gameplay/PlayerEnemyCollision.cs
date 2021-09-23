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

        public override void Execute()
        {
            var willHurtEnemy = false;
            if (hitbox != null && player == null)
                willHurtEnemy = hitbox.bounds.center.y <= enemy.Bounds.max.y;
            if (willHurtEnemy)
            {
                Debug.Log("will hurt enemy");
                var enemyHealth = enemy.GetComponent<Health>();
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
            else
            {
                Schedule<PlayerDeath>();
            }
        }
    }
}