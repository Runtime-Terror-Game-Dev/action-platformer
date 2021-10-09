using System.Collections;
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
        public GameObject hitbox;

        public float hitboxOffset;
        public float knockback = 2f;
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
        public string damagedState = "Neutral";
        public float AttackCoolDownDuration;
        private float AttackCoolDownTimer = 0;
        public float AttackHitboxDuration;
        public float AttackComboTiming = 0.1f;
        private float AttackHitboxTimer = 0;

        public float DamageFlashDuration = 0.2f;
        public float DamageFlashTimer = 0;
        private bool stopJump;
        /*internal new*/
        public Collider2D collider2d;
        /*internal new*/

        public Rigidbody2D playerrigidbody;
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
            hitbox.SetActive(false);
            playerrigidbody = GetComponent<Rigidbody2D>();
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
                hitbox.SetActive(true);
            }
            if (AttackHitboxTimer == 0)
            {
                if (AttackCoolDownTimer > 0)
                {
                    AttackCoolDownTimer -= Time.deltaTime;
                    attackState = AttackState.Cooldown;
                    hitbox.SetActive(false);
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
            if (DamageFlashTimer > 0)
            {
                DamageFlashTimer -= Time.deltaTime;
            }
            if (DamageFlashTimer <= 0)
            {
                DamageFlashTimer = 0;
                damagedState = "Neutral";
            }
            //damagedState colors
            switch (damagedState)
            {
                case "Neutral":
                    spriteRenderer.color = Color.green;
                    break;
                case "Impact":
                    spriteRenderer.color = Color.red;
                    break;
                case "Invulnerable":
                    spriteRenderer.color = Color.blue;
                    break;
            }

            UpdateJumpState();
            base.Update();
        }
        void Attack()
        {
            Debug.Log(damagedState);
            float prevAttackTimeAgo;
            //check for chain of attacks
            //if previous was a success, we can attack. if failure, we don't.
            //if exception because stack was empty (no previous attacks), we attack, and push to stack.

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

            // add flip hitbox
            if (move.x > 0.01f)
            {
                if (spriteRenderer.flipX != false)
                {
                    spriteRenderer.flipX = false;
                }
                hitbox.transform.localPosition = new Vector2(gameObject.transform.localPosition.x + hitboxOffset, gameObject.transform.localPosition.y); //hitbox tracks player
            }
            else if (move.x < -0.01f)
            {
                if (spriteRenderer.flipX != true)
                {
                    spriteRenderer.flipX = true;
                }
                hitbox.transform.localPosition = new Vector2(gameObject.transform.localPosition.x - hitboxOffset, gameObject.transform.localPosition.y); //hitbox tracks player
            }
            //TODO if in attack, prevent the player from flipping
            animator.SetBool("grounded", IsGrounded);

            targetVelocity = move * maxSpeed;
            if (damagedState != "Impact")
            {
                animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
                targetVelocity = move * maxSpeed;
            }
            else
            {
                targetVelocity = new Vector2(-knockback, knockback);
                //TODO make knockback dependent on direction of impact
                velocity.y = knockback;
                //only the x of targetvelocity is considered for velocity.x, velocity.y is separate
                animator.SetFloat("velocityX", Mathf.Abs(velocity.x));
            }

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
        //TODO add enum DamagedState across player and PlayerDamage and enemy
    }
}