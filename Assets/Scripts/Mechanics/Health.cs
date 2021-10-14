using System;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Represents the current vital statistics of some game entity.
    /// </summary>
    public class Health : MonoBehaviour
    {
        /// <summary>
        /// The maximum hit points for the entity.
        /// </summary>
        public int maxHP = 1;

        /// <summary>
        /// Indicates if the entity should be considered 'alive'.
        /// </summary>
        public bool IsAlive => currentHP > 0;

        public int currentHP;

        private bool debug = true;
        public HealthBar healthBar;

        /// <summary>
        /// Increment the HP of the entity.
        /// </summary>
        public void Increment(int hp=1)
        {
            // Try not to do this. Slow, unecessary, skips events.
            //currentHP = Mathf.Clamp(currentHP+hp, 0, maxHP);

            //currentHP += hp;

            if ((currentHP+=hp) >= maxHP)
            {
                currentHP = maxHP;
                // Healed to max, particles/FX as desired
            }
            else
            {
                // Healed partially, particles/FX as desired
            }
            if (debug) Debug.Log(name + " has healed " + hp + " HealthPoints!\n HP: " + currentHP + "/" + maxHP);
        }

        /// <summary>
        /// Decrement the HP of the entity. Will trigger a HealthIsZero event when
        /// current HP reaches 0.
        /// </summary>
        public void Decrement(int hp=1)
        {
            currentHP -= hp;

            if (debug) Debug.Log(name+" has taken "+hp+" damage!\n HP: "+currentHP+"/"+maxHP);
            healthBar.SetHealth(currentHP);

            if (currentHP <= 0)
            {
                var ev = Schedule<HealthIsZero>();
                ev.entity = gameObject; //the gameObject that this health script is attached to
                ev.health = this; //the script itself
            }
        }

        /// <summary>
        /// Decrement the HP of the entitiy until HP reaches 0.
        /// </summary>
        public void Kill()
        {
            Decrement(currentHP);
        }

        void Awake()
        {
            currentHP = maxHP;
            healthBar.SetMaxHealth(maxHP);
        }
    }
}
