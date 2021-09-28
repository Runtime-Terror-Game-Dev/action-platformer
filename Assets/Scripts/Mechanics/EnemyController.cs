using System.Collections;
using System.Collections.Generic;
using System;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        public PatrolPath path;
        public AudioClip ouch;

        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;
        private Collider2D playerControllerHitbox;


        public Bounds Bounds => _collider.bounds;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        //jank
        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            var hitbox = collision.gameObject.GetComponent<Hitbox>();
            try
            {
                if (hitbox)
                {
                    playerControllerHitbox = hitbox.gameObject.GetComponent<Collider2D>();
                }
            }
            catch (SystemException e)
            {
                playerControllerHitbox = null;
            }
            //Hitbox should be set.
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
                ev.hitbox = null;
            }
            //custom hitbox detection
            if (playerControllerHitbox != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = null;
                ev.hitbox = playerControllerHitbox;
                ev.enemy = this;
            }
        }

        void Update()
        {
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
        }

    }
}