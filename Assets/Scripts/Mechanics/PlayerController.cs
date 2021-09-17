﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;
        public AudioClip[] attackAudioClips;
        private int attackAudioClipIndex;
        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 5;
        // private String spriteColor;
        public JumpState jumpState = JumpState.Grounded;
        public AttackState attackState = AttackState.Neutral;
        public float AttackCoolDownDuration;
        private float AttackCoolDownTimer = 0;
        public float AttackHitboxDuration;
        public float AttackComboTiming = 0.1f;
        private float AttackHitboxTimer = 0;
        private bool stopJump;
        /*internal new*/
        public Collider2D collider2d;
        /*internal new*/
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        public Bounds Bounds => collider2d.bounds;
        private Stack<float> AttackStack = new Stack<float>(); //elapsed times between attacks
        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
                else if (Input.GetButtonDown("Fire1"))
                    Attack();
            }
            else
            {
                move.x = 0;
            }
            //sequencing of hitbox and cooldown
            if (AttackHitboxTimer > 0)
            {
                AttackHitboxTimer -= Time.deltaTime;
                attackState = AttackState.Attacking;
            }
            if (AttackHitboxTimer == 0)
            {
                if (AttackCoolDownTimer > 0)
                {
                    AttackCoolDownTimer -= Time.deltaTime;
                    attackState = AttackState.Cooldown;
                }
            }
            if (AttackCoolDownTimer <= 0)
            {
                AttackCoolDownTimer = 0;
                attackState = AttackState.Neutral;
            }
            if (AttackHitboxTimer <= 0)
            {
                AttackHitboxTimer = 0;
            }
            if (AttackStack.Count > 0)
            {
                float recordedTime = AttackStack.Pop();
                AttackStack.Push(recordedTime + Time.deltaTime);
            }
            //attackState colors
            switch (attackState)
            {
                case AttackState.Neutral:
                    spriteRenderer.color = Color.green;
                    break;
                case AttackState.Attacking:
                    spriteRenderer.color = Color.red;
                    break;
                case AttackState.Cooldown:
                    spriteRenderer.color = Color.blue;
                    break;
            }

            UpdateJumpState();
            base.Update();
        }
        void Attack()
        {
            float prevAttackTimeAgo;
            //check for chain of attacks
            //if previous was a success, we can attack. if failure, we don't.
            //if exception because stack was empty (no previous attacks), we attack, and push to stack.

            //broken exception for now
            // try
            // {
            //     success = AttackStack.Pop();
            // }
            // catch (SystemException.InvalidOperationException)
            // {
            //     success = true;
            // }

            if (AttackStack.Count > 0)
            {
                prevAttackTimeAgo = AttackStack.Pop();
            }
            else
                prevAttackTimeAgo = 0;

            if (attackState == AttackState.Neutral) //base attack
            {
                //checking whether to push a new success or a new failure (executed combo)
                //check elapsed time, not attackcooldowntimer
                if (prevAttackTimeAgo <= (AttackCoolDownDuration + AttackComboTiming))
                {
                    // success
                    Debug.Log("success");
                    PlayRandom2();
                    AttackCoolDownTimer = AttackCoolDownDuration;
                    AttackHitboxTimer = AttackHitboxDuration;
                    AttackStack.Push(0); //starting new time
                }
                else
                {
                    Debug.Log("failure");
                    // failure
                    //playbadsound
                    audioSource.PlayOneShot(ouchAudio);
                    AttackCoolDownTimer = AttackCoolDownDuration * 3;
                    AttackStack.Clear();
                }
            }
            if (attackState == AttackState.Cooldown) //punish if they attack during cooldown
            {
                Debug.Log("failure");
                // failure
                audioSource.PlayOneShot(ouchAudio);
                AttackCoolDownTimer = AttackCoolDownDuration * 3;
                AttackStack.Clear();
            }


        }
        //add color tryparsehtml utility conversion function

        int RepeatCheck(int previousIndex, int range)
        {
            int index = Random.Range(0, range);

            while (index == previousIndex)
            {
                index = Random.Range(0, range);
            }
            return index;
        }
        void PlayRandom2()
        {
            attackAudioClipIndex = RepeatCheck(attackAudioClipIndex, attackAudioClips.Length);
            audioSource.PlayOneShot(attackAudioClips[attackAudioClipIndex]);
        }
        void PlayRoundRobin()
        {

            if (attackAudioClipIndex < attackAudioClips.Length)
            {
                audioSource.PlayOneShot(attackAudioClips[attackAudioClipIndex]);
                attackAudioClipIndex++;
            }

            else
            {
                attackAudioClipIndex = 0;
                audioSource.PlayOneShot(attackAudioClips[attackAudioClipIndex]);
                attackAudioClipIndex++;
            }
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }
        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
        public enum AttackState
        {
            Neutral,
            Attacking,
            Cooldown
        }
    }
}