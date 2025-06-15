using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    [Header("Hitboxes")]

    [SerializeField]
    private GameObject pounceAttackHitbox;

    [Header("FAK Debug")]

    [SerializeField]
    private float FAK_timer;
    private float FAK_time = 3;

    void Start()
    {
        bossState = BossState.WALK_TOWARDS;
        FAK_timer = FAK_time;
    }

    void Update()
    {
        SetState();
        Move();
        HandleAttack();
        if (bossState == BossState.ATTACK)
        {
            pounceAttackHitbox.SetActive(true);
        } else
        {
            pounceAttackHitbox.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        // Force the head to track the player's position
        if (lookAtTarget != null)
        {
            Vector3 target = new Vector3(lookAtTarget.transform.position.x, neckBone.transform.position.y, lookAtTarget.transform.position.z);
            Debug.DrawRay(lookAtTarget.transform.position, target - lookAtTarget.transform.position, Color.green);

            Vector3 fromTargetOrigin = new Vector3(neckBone.transform.position.x, 0, neckBone.transform.position.z);
            Vector3 fromTarget = new Vector3(neckBone.transform.up.x, 0, neckBone.transform.up.z);
            Vector3 toTarget = target - neckBone.transform.position;
            float angle = Vector3.SignedAngle(fromTarget, toTarget, Vector3.up);
            Debug.Log(Vector3.Dot(fromTarget, toTarget));

            neckBone.transform.Rotate(Vector3.forward, angle);
            Debug.DrawRay(fromTargetOrigin, toTarget, Color.green);
        }
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, lookAtTarget.transform.position);
    }

    private void SetState()
    {
        if (DistanceToPlayer() > walkAroundRadius)
        {
            bossState = BossState.WALK_TOWARDS;
        } else if (bossState != BossState.WALK_AROUND)
        {
            bossState = BossState.WALK_AROUND;
            orbiter.transform.position = lookAtTarget.transform.position;
            orbitTarget.transform.position = transform.position;
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
                break;
            
            default:
                Debug.LogError("BossState has been set to an invalid value");
                break;
        }
    }

    private void HandleAttack()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lookAtTarget.transform.position, walkAroundRadius / 2);
    }
}
