using DG.Tweening;
using UnityEngine;

public class CatController : MonoBehaviour
{
    public enum BossState
    {
        IDLE,
        WALK_TOWARDS,
        WALK_AROUND,
        ATTACK
    }

    [Header("Animation State")]

    [SerializeField]
    private BossState bossState;

    [SerializeField]
    private Animator animator;

    [Header("Walk")]

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float walkRotationSpeed;

    [SerializeField]
    [Tooltip("Radius of the circular region around which the boss will stalk the player before attacking.")]
    private float walkAroundRadius;

    [SerializeField]
    private Vector3 walkAroundTarget;

    [SerializeField]
    private float walkAroundSpeed;

    [SerializeField]
    private GameObject orbiter;

    [SerializeField]
    private GameObject orbitTarget;

    [Header("Neck Animation Override")]

    [SerializeField]
    private GameObject neckBone;

    [SerializeField]
    private GameObject lookAtTarget;

    [SerializeField]
    private float neckRotationSpeed;

    [Header("Attack")]

    [SerializeField]
    private float orbitToAttackTime;

    [SerializeField]
    [Tooltip("Orbit-to-attack timer will add a random offset when initializing - this is the maximum value for that range.")]
    private float orbitToAttackOffsetMax;

    private float orbitToAttackTimer;

    [SerializeField]
    private float attackCooldownTime;

    private float attackCooldownTimer;

    [SerializeField]
    [Tooltip("Need a small delay when transitioning to the attack state to help smooth head movement - this is that duration.")]
    private float onAttackHeadOverrideTime;

    private float onAttackHeadOverrideTimer;

    [SerializeField]
    private float hitboxStartTime;

    private float hitboxStartTimer;
    
    [SerializeField]
    private float hitboxEndTime;

    private float hitboxEndTimer;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float jumpOffset;

    [SerializeField]
    private bool isJumping;

    [SerializeField]
    private float jumpDelayTime;

    private float jumpDelayTimer;

    [Header("Hitboxes")]

    [SerializeField]
    private GameObject pounceAttackHitbox;

    void Start()
    {
        bossState = BossState.WALK_TOWARDS;
        InitTimers();
    }

    void Update()
    {
        SetState();
        // Band-aid fix
        if (bossState != BossState.ATTACK && isJumping)
        {
            isJumping = false;
        }
        Move();
        if (bossState == BossState.WALK_AROUND)
        {
            UpdateTimer(ref orbitToAttackTimer);
        }
        UpdateTimer(ref attackCooldownTimer);
        UpdateTimer(ref onAttackHeadOverrideTimer);
        UpdateTimer(ref hitboxStartTimer);
        UpdateTimer(ref hitboxEndTimer);
        UpdateTimer(ref jumpDelayTimer);

        if (bossState == BossState.ATTACK && hitboxStartTimer <= 0 && hitboxEndTimer > 0)
        {
            pounceAttackHitbox.SetActive(true);
        } else
        {
            pounceAttackHitbox.SetActive(false);
        }

        if (isJumping == false)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    private void LateUpdate()
    {
        // Force the head to track the player's position
        if (lookAtTarget != null && (bossState != BossState.ATTACK || onAttackHeadOverrideTimer > 0))
        {
            Vector3 target = new Vector3(lookAtTarget.transform.position.x, neckBone.transform.position.y, lookAtTarget.transform.position.z);
            Debug.DrawRay(lookAtTarget.transform.position, target - lookAtTarget.transform.position, Color.green);

            Vector3 fromTargetOrigin = new Vector3(neckBone.transform.position.x, 0, neckBone.transform.position.z);
            Vector3 fromTarget = new Vector3(neckBone.transform.up.x, 0, neckBone.transform.up.z);
            Vector3 toTarget = target - neckBone.transform.position;
            float angle = Vector3.SignedAngle(fromTarget, toTarget, Vector3.up);

            neckBone.transform.Rotate(Vector3.forward, angle);
            Debug.DrawRay(fromTargetOrigin, toTarget, Color.green);
        }
    }

    private void InitTimers()
    {
        ResetTimer(ref orbitToAttackTimer, orbitToAttackTime, orbitToAttackOffsetMax);
    }

    private void ResetTimer(ref float timer, float duration, float offsetMax = 0)
    {
        timer = duration + Random.Range(0, offsetMax);
    }

    private void UpdateTimer(ref float timer)
    {
        timer -= Time.deltaTime;
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, lookAtTarget.transform.position);
    }

    private void SetState()
    {
        var originalState = bossState;

        if (DistanceToPlayer() > walkAroundRadius)
        {
            bossState = BossState.WALK_TOWARDS;
        }
        else if (bossState == BossState.WALK_AROUND && orbitToAttackTimer <= 0 && attackCooldownTimer <= 0)
        {
            bossState = BossState.ATTACK;
            ResetTimer(ref attackCooldownTimer, attackCooldownTime);
            ResetTimer(ref onAttackHeadOverrideTimer, onAttackHeadOverrideTime);
            ResetTimer(ref hitboxStartTimer, hitboxStartTime);
            ResetTimer(ref hitboxEndTimer, hitboxEndTime);
            ResetTimer(ref jumpDelayTimer, jumpDelayTime);
        }
        else if (bossState != BossState.WALK_AROUND && attackCooldownTimer <= 0)
        {
            bossState = BossState.WALK_AROUND;
            orbiter.transform.position = lookAtTarget.transform.position;
            Vector3 orbitTargetDirection = transform.position - orbiter.transform.position;
            orbitTarget.transform.position = orbiter.transform.position + Vector3.Normalize(orbitTargetDirection) * walkAroundRadius;
            ResetTimer(ref orbitToAttackTimer, orbitToAttackTime, orbitToAttackOffsetMax);
        }

        if (originalState != bossState)
        {
            animator.SetInteger("CatState", (int)bossState);
            Debug.Log((int)bossState);
        }
    }

    private void Move()
    {
        switch (bossState)
        {
            case BossState.IDLE:
                break;

            case BossState.WALK_TOWARDS:
                Vector3 direction = lookAtTarget.transform.position - transform.position;
                transform.position += direction * walkSpeed * Time.deltaTime;

                Vector3 target = new Vector3(lookAtTarget.transform.position.x, transform.position.y, lookAtTarget.transform.position.z);
                var currentRotation = transform.rotation;
                transform.LookAt(target);
                var targetRotation = transform.rotation;
                transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, walkRotationSpeed * Time.deltaTime);

                Debug.DrawRay(transform.position, lookAtTarget.transform.position - transform.position, Color.red);
                break;
            
            case BossState.WALK_AROUND:
                transform.position = Vector3.Lerp(transform.position, orbitTarget.transform.position, walkSpeed * Time.deltaTime);

                orbiter.transform.Rotate(Vector3.up, walkAroundSpeed * Time.deltaTime);
                currentRotation = transform.rotation;
                transform.LookAt(orbitTarget.transform.position);
                targetRotation = transform.rotation;
                transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, walkRotationSpeed * Time.deltaTime);

                Debug.DrawRay(transform.position, lookAtTarget.transform.position - transform.position, Color.yellow);
                break;
            
            case BossState.ATTACK:
                // Keep rotating orbiter target when attacking
                orbiter.transform.Rotate(Vector3.up, walkAroundSpeed * Time.deltaTime);

                if (pounceAttackHitbox.activeSelf && jumpDelayTimer <= 0)
                {
                    direction = lookAtTarget.transform.position - transform.position;
                    float distance = Vector3.Distance(transform.position, lookAtTarget.transform.position);
                    distance -= jumpOffset;
                    transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.Normalize(direction) * distance, jumpForce * Time.deltaTime);
                    //transform.DOJump(lookAtTarget.transform.position + direction * jumpOffset, jumpForce, 1, hitboxEndTime);
                    if (isJumping == false)
                    {
                        isJumping = true;
                    }
                }

                if (hitboxEndTime <= 0 && isJumping)
                {
                    isJumping = false;
                }

                currentRotation = transform.rotation;
                transform.LookAt(lookAtTarget.transform.position);
                targetRotation = transform.rotation;
                transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, walkRotationSpeed * Time.deltaTime);

                //Debug.Log("Attack");
                break;
            
            default:
                Debug.LogError("BossState has been set to an invalid value");
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lookAtTarget.transform.position, walkAroundRadius / 2);
    }
}
