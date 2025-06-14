using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CatController : MonoBehaviour
{
    public enum BossState
    {
        WALK,
        POUNCE_ATTACK
    }

    [Header("Animation State")]

    [SerializeField]
    private BossState bossState;

    [SerializeField]
    private Animator animator;

    [Header("Neck Animation Override")]

    [SerializeField]
    private GameObject neckBone;

    [SerializeField]
    private GameObject lookAtTarget;

    [Header("Hitboxes")]

    [SerializeField]
    private GameObject pounceAttackHitbox;

    [Header("FAK Debug")]

    [SerializeField]
    private float FAK_timer;
    private float FAK_time = 3;

    void Start()
    {
        bossState = BossState.WALK;
        FAK_timer = FAK_time;
    }

    void Update()
    {
        if (FAK_timer > 0)
        {
            FAK_timer -= Time.deltaTime;

            if (FAK_timer <= 0)
            {
                bossState = BossState.POUNCE_ATTACK;
                animator.SetInteger("CatState", (int)bossState);
            }
        }

        if (bossState == BossState.POUNCE_ATTACK)
        {
            pounceAttackHitbox.SetActive(true);
        } else
        {
            pounceAttackHitbox.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (lookAtTarget != null)
        {
            Vector3 target = new Vector3(lookAtTarget.transform.position.x, neckBone.transform.position.y, lookAtTarget.transform.position.z);
            Debug.DrawRay(lookAtTarget.transform.position, target - lookAtTarget.transform.position, Color.green);

            Vector3 fromTarget = new Vector3(neckBone.transform.position.x, 0, neckBone.transform.position.z);
            Vector3 toTarget = target - neckBone.transform.position;
            float angle = Vector3.SignedAngle(fromTarget, toTarget, Vector3.up);

            neckBone.transform.Rotate(Vector3.forward, angle);
        }
    }
}
