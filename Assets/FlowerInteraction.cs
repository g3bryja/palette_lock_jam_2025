using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerInteraction : MonoBehaviour
{

    private bool isColliding;
    [SerializeField] Collider collider;

    void Update()
    {

        Debug.Log(isColliding);

    }

    // OnCollisionEnter() and OnCollisionExit() are built-in Unity methods
    public void OnCollisionEnter(Collision collision)
    {
        isColliding = true;
    }

    public void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }

}
