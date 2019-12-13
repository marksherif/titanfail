using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class TitanRigidbodyFirstPersonController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
            public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;
            public Animator anim;


#if !MOBILE_INPUT
            private bool m_Running;
#endif
            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
                if (input == Vector2.zero) return;
                if (input.x > 0 || input.x < 0)
                {
                    //strafe
                    CurrentTargetSpeed = StrafeSpeed;
                }
                if (input.y < 0)
                {
                    //backwards
                    CurrentTargetSpeed = BackwardSpeed;
                }
                if (input.y > 0)
                {
                    //forwards
                    //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                    CurrentTargetSpeed = ForwardSpeed;
                }
#if !MOBILE_INPUT
                if (Input.GetKey(RunKey))
                {
                    CurrentTargetSpeed *= RunMultiplier;
                    m_Running = true;
                }
                else
                {
                    m_Running = false;
                }
                //base.PlayFootStepAudio();
#endif
            }

#if !MOBILE_INPUT
            public bool Running
            {
                get { return m_Running; }
            }
#endif
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }


        public Camera cam;
        public GameObject PlayerFPS;
        public GameObject PlayerHUD;
        public GameObject TitanHUD;
        public GameObject Titan;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;
        private bool core_deployed = false;
        private int jump_count = 0;
        private bool dash;
        private int dash_count = 3;
        private float health = 400;
        private float time = 15.0f;
        public Image health_bar;
        public Image core_ability_meter_bar;
        public float coreAbilityMeter;

        public Text dash_text;
        [SerializeField] public AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] public AudioClip m_Dash;           // the sound played when character leaves the ground.
        [SerializeField] public AudioClip m_LandSound;           // the sound played when character touches back on ground.
        public AudioSource m_AudioSource1;
        public AudioSource m_AudioSource2;

        private bool defensive_enabled = false;
        private bool defensive_cooldown_over = true;

        private void PlayLandingSound()
        {
            m_AudioSource1.clip = m_LandSound;
            m_AudioSource1.Play();
            //m_NextStep = m_StepCycle + .5f;
        }

        private void PlayDashSound()
        {
            m_AudioSource1.clip = m_Dash;
            m_AudioSource1.Play();
        }

        public void PlayFootStepAudio1()
        {
            if (!m_IsGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = UnityEngine.Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource1.clip = m_FootstepSounds[n];
            m_AudioSource1.PlayOneShot(m_AudioSource1.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource1.clip;
        }

        public void PlayFootStepAudio2()
        {
            if (!m_IsGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = UnityEngine.Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource2.clip = m_FootstepSounds[n];
            m_AudioSource2.PlayDelayed(0.25f);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource2.clip;
        }

        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
#if !MOBILE_INPUT
                return movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init(transform, cam.transform);
            coreAbilityMeter = 0;                          // a playerTitan has an empty core ability meter
        }


        private void Update()
        {
            RotateView();
            health_bar.fillAmount = health / 400f;

            core_ability_meter_bar.fillAmount = coreAbilityMeter / 100f;

            dash_text.text = dash_count + "/3";

            if ((CrossPlatformInputManager.GetButtonDown("TitanDefensiveAbility") && defensive_cooldown_over))
            {
                defensive_enabled = true;
                StartCoroutine(disableDefensiveAbility(10.0f));
            }
            if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
            {
                // Instead of changing all the logic behind the jump (since titans dont jump) i'll just set the m_jump
                // to false and my new var "dash" to true
                m_Jump = false;
                if (dash_count > 0)
                    dash = true;
            }
            if (CrossPlatformInputManager.GetButtonDown("TitanCore") && coreAbilityMeter == 100f)
            {
                coreAbilityMeter = 0;
                //PerformCore();

            }
            if (CrossPlatformInputManager.GetButtonDown("TitanEmbark")) //dis-embarking from titan
            {
                PlayerFPS.transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                PlayerFPS.transform.rotation = transform.rotation;
                var clone = Instantiate(Titan, new Vector3(transform.position.x, transform.position.y + 10, transform.position.z - 5), transform.rotation);
                clone.SetActive(true);
                gameObject.SetActive(false);
                TitanHUD.SetActive(false);
                PlayerFPS.SetActive(true);
                PlayerHUD.SetActive(true);
                PlayerFPS.GetComponent<RigidbodyFirstPersonController>().startRefillAfterTitan();
            }
            if (!defensive_cooldown_over)
            {
                time -= Time.deltaTime;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "EnemyBullet")
            {
                if (!defensive_enabled)
                {

                    health -= collision.gameObject.GetComponent<bullet_hit>().bullet_damage;
                    Destroy(collision.gameObject);

                    if (health <= 0)
                    {
                        PlayerFPS.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1, gameObject.transform.position.z);
                        PlayerFPS.transform.rotation = gameObject.transform.rotation;
                        gameObject.SetActive(false);
                        TitanHUD.SetActive(false);
                        PlayerFPS.SetActive(true);
                        PlayerHUD.SetActive(true);
                        PlayerFPS.GetComponent<RigidbodyFirstPersonController>().titan_deployed = false;
                        PlayerFPS.GetComponent<RigidbodyFirstPersonController>().startRefillAfterTitan();
                    }
                }
            }
        }

        IEnumerator disableDefensiveAbility(float time)
        {
            yield return new WaitForSeconds(time);

            defensive_enabled = false;
            defensive_cooldown_over = false;
            callCooldown();
        }

        public void callCooldown()
        {
            StartCoroutine(defensiveAbilityCooldown(15.0f));
        }

        IEnumerator defensiveAbilityCooldown(float time)
        {
            yield return new WaitForSeconds(time);

            defensive_cooldown_over = true;
            time = 15.0f;
        }


        IEnumerator RefillDash(float time)
        {
            yield return new WaitForSeconds(time);

            dash_count += 1;
        }


        private void FixedUpdate()
        {
            GroundCheck();
            Vector2 input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
                if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                    if (!m_AudioSource1.isPlaying)
                        PlayFootStepAudio1();
                    if (!m_AudioSource2.isPlaying && movementSettings.Running)
                        PlayFootStepAudio2();
                }
                if (dash)
                {
                    dash_count -= 1;
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier()*70, ForceMode.Impulse);
                    PlayDashSound();
                    if (dash_count == 0)
                    {
                        StartCoroutine(RefillDash(5));
                    }
                }
            }

            if (m_IsGrounded)
            {
                jump_count = 0;
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                    jump_count += 1;
                    PlayDashSound();
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                if (m_Jump && jump_count < 1)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                    jump_count += 1;
                    PlayDashSound();
                }
                else
                {
                    m_RigidBody.drag = 0f;
                    if (m_PreviouslyGrounded && !m_Jumping)
                    {
                        StickToGroundHelper();
                    }
                }
            }
            m_Jump = false;
            dash = false;
        }


        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {

            Vector2 input = new Vector2
            {
                x = CrossPlatformInputManager.GetAxis("Horizontal"),
                y = CrossPlatformInputManager.GetAxis("Vertical")
            };
            movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation(transform, cam.transform);

            if (m_IsGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
                PlayLandingSound();
            }
        }
        void PerformCore()
        {
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 10.0f);
            int i = 0;
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].gameObject.CompareTag("EnemyTitan") || hitColliders[i].gameObject.CompareTag("EnemyPilot"))
                {
                    i = hitColliders.Length;
                    //gameObject.transform.LookAt(hitColliders[i].gameObject.transform);
                    //run script
                    // script target = hitColliders[i].gameObject
                }
            }
        }
    }
}
