using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    PlayerAnimator animator;
    StageProgress stageProgress;
    ObjectPool pool;

    public static int RingCount;
    public int LifeCount = 3;
    public TextMeshProUGUI RingDisplay;
    public TextMeshProUGUI LifeDisplay;
    public Renderer[] PlayerMesh;

    public int RingsPerOneUp = 100;
    public int RingsToLife { get; set; }
    public int MaxLostRings = 30; //The most rings Sonic can lose
    public float RingLossRadius = 0.3f; //Initial radius of lost rings
    public float RingLossHeight = 0.5f; //Y Velocity of lost rings upon spawning
    public float RingLossForce = 1f; //XZ Velocity of lost rings upon spawning

    //Shield values
    public bool HasShield { get; set; }
    public GameObject ShieldNormal;
    public ParticleSystem InvincibilityShield;
    public float IShieldDuration = 20;

    public float IFrameDuration;
    float t;
    public float FlickerRate = 5;
    float frame_t;
    int Framerate = 60;
    public bool IsInvincible { get; set; }
    public static bool IsDead;
    //Debugging
    [SerializeField] string RingLossKey;

    string ringCounter (int amt)
    {
        return string.Format("{00:000}", amt);
    }

    // Start is called before the first frame update
    void Start()
    {
        stageProgress = FindObjectOfType<StageProgress>();
        pool = ObjectPool.PoolManager;
        animator = GetComponent<PlayerAnimator>();
        ShieldNormal.SetActive(false);
        RingsToLife = RingsPerOneUp;
    }

    // Update is called once per frame
    void Update()
    {
        RingDisplay.text = ringCounter(RingCount);
        LifeDisplay.text = LifeCount.ToString();

        if (RingsToLife <= 0)
        {
            animator.PlayOneUp();
            LifeCount++;
            RingsToLife = RingsPerOneUp;
        }

        if (IsInvincible)
        {
            if (!HasShield)
            {
                ///Invincibility frames are handled much like the lost rings in terms of internal behavior. We have two timers, one counting
                ///down how many seconds Sonic can stay invincible, and another counting down the frames to toggle Sonic's mesh.
                ///Once the invincibility timer hits 0, we re-enable Sonic's mesh and disable invincibility.
                t -= Time.deltaTime;
                frame_t-= Time.deltaTime * Framerate;
                if (frame_t <= 0)
                {
                    //We use a for loop for enabling and disabling Sonic's mesh as it's an array, in case your custom model has multiple mesh renderers.
                    //For shadow consistency, instead of directly disabling the mesh renderer, we will just switch between rendering it fully and rendering only shadows.
                    for (int i = 0; i < PlayerMesh.Length; i++)
                    {
                        if (PlayerMesh[i].shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On)
                        {
                            PlayerMesh[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                        } else if (PlayerMesh[i].shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly)
                        {
                            PlayerMesh[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        }
                        frame_t = FlickerRate;
                    }
                }

                if (t <= 0)
                {
                    for (int i = 0; i < PlayerMesh.Length; i++)
                    {
                        PlayerMesh[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    }
                    IsInvincible = false;
                }
            } else
            {
                t -= Time.deltaTime;
                if (t <= 0)
                {
                    InvincibilityShield.Stop();
                    HasShield = false;
                    stageProgress.ChangeMusic("stage music", true);
                    IsInvincible = false;
                }
            }
        }

    }

    public void TakeDamage()
    {
        ///First we check if Sonic is invincible. If we isn't, we check his ring count to determine if he needs to take damage or if he's supposed to die.
        ///If he has rings, we check if his ring count is above the maximum amount of rings we can recover. If so, we drop that many rings. If not, we drop
        ///as many as he has. Afterwards we reset the invincibility and flicker timers, then enable invincibility.
        if (!IsInvincible)
        {
            if (HasShield)
            {
                animator.PlayHurtVoice();
                ShieldNormal.SetActive(false);
                HasShield = false;
            }
            else
            {
                if (RingCount > 0)
                {
                    animator.PlayHurtVoice();
                    int AmountToLose = RingCount;
                    if (AmountToLose > MaxLostRings)
                        AmountToLose = MaxLostRings;
                    LoseRings(AmountToLose);
                    RingCount = 0;
                    t = IFrameDuration;
                    frame_t = FlickerRate;
                    IsInvincible = true;
                }
                else
                {
                    animator.PlayDeathVoice();
                    IsDead = true;
                }
            }
        }
    }
    public void RemoveShield()
    {
        HasShield = false;
        ShieldNormal.SetActive(false);
        if (IsInvincible)
        {
            InvincibilityShield.Stop(true);
            stageProgress.ChangeMusic("Stage Music", true);
            IsInvincible = false;
        }
    }

    public void AddShield(int ShieldType)
    {
        if (HasShield && !IsInvincible)
        {
            HasShield = false;
            ShieldNormal.SetActive(false);
        } else if (HasShield && IsInvincible)
        {
            return;
        }
        switch (ShieldType)
        {
            case 0:
                ShieldNormal.SetActive(true);
                break;
            case 1:
                InvincibilityShield.Play();
                t = IShieldDuration;
                stageProgress.ChangeMusic("invincibility", false);
                IsInvincible = true;
                break;
        }
        HasShield = true;
    }

    void LoseRings(int amount)
    {
        ///What we'll be doing here is spawning a certain amount of rings in a circle around Sonic.
        ///What we do first is get the degree increment by dividing 360 by the amount of rings we need to drop. Then
        ///we use a for loop for actually spawning the rings.
        ///Inside the for loop, we first use Quaternion.AngleAxis and mutliply it by Sonic's forward direction to get the direction the
        ///ring will be spawning and spreading. Then we multiply that direction by the spawn radius and add it to Sonic's position
        ///so we know where to spawn the ring. Finally, we grab a Lost Ring from the object pool and set its rigidbody's
        ///velocity to direction * RingLossForce and transform.up * RingLossHeight.
        float Step = 360f / amount;
        for (int i = 0; i < amount; i++)
        {
            Quaternion _rotation = Quaternion.AngleAxis(Step * i, transform.up);
            Vector3 Direction = _rotation * transform.forward;
            Vector3 Position = transform.position + Direction * RingLossRadius;
            GameObject ring = pool.SpawnFromPool("LostRing", Position, transform.rotation);
            ring.GetComponent<Rigidbody>().velocity = Direction * RingLossForce + transform.up * RingLossHeight;
        }
        animator.ActionSoundSource.PlayOneShot(animator.GetSoundFromBank("Damage", animator.EnemySounds));
        RingsToLife = RingsPerOneUp;
    }
}
