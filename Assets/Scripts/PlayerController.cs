using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    private Vector2 input;
    private CharacterController characterController;
    private Vector3 direction;
    private bool dashInput;

    [SerializeField] private float speed;

    [SerializeField] private float dashSpeed;

    [SerializeField] private float dashDuration;
    private float dashTimer;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

    }

    private void Update()
    {

        if (!dashInput)
        {
            characterController.Move(direction * speed * Time.deltaTime);
        }
        else
        {
            characterController.Move(direction * dashSpeed * Time.deltaTime);
        }


        dashTimer = dashTimer - Time.deltaTime;
        if (dashTimer <= 0)
        {
            dashInput = false;
        }

    }

    public void Move(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0.0f, input.y);

        // Debug.Log(direction);

    }

    public void Dash(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            dashInput = true;
            dashTimer = dashDuration;
        }

        // Debug.Log(context.phase);
    }

    public void OnTriggerEnter(Collider other)
    {
        try
        {
            other.transform.parent.GetComponent<FlowerInteraction>().StartPollinating();
        }
        catch (NullReferenceException){
            Debug.Log("lmao");
        }
    }


    public void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerEXIT");
    }


}
