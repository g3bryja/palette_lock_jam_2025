using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private Interactable interactable;

    void OnCollisionEnter(Collision collision)
    {
        
    }

    void OnCollisionExit(Collision collision)
    {

    }

    void OnInteract() {
        if (interactable != null) {
            
        }
    }
}
