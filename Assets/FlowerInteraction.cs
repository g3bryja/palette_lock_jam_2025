using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerInteraction : MonoBehaviour
{

    private bool isColliding;
    [SerializeField] Collider collider;

    [SerializeField] float interactionDuration = 5;

    private float interactionTimer;

    private bool isFinished;

    void Awake()
    {
        interactionTimer = interactionDuration;
    }

    void Update()
    {
        if (isColliding)
        {
            interactionTimer = interactionTimer - Time.deltaTime;
        }

        if (interactionTimer <= 0 && !isFinished)
        {
            FinishPollinating();
        }
        
    }

    public void StartPollinating()
    {
        isColliding = true;
        Debug.Log("Stating to pollinate");

    }

    public void StopPollinating()
    {
        isColliding = false;
    }

    public void FinishPollinating()
    {
        Debug.Log("Flower consumed! +30");
        isFinished = true;
    }


}
