using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PinchDetection : MonoBehaviour
{
    [SerializeField]
    private float zoomSpeed = 1.0f;

    private float previousDistance = 0;
    private bool isPinching = false;
    private Vector2 fstFinger;
    private Vector2 sndFinger;

    private void Update()
    {
        if (isPinching)
        {
            Zoom();
        }
    }

    private void Zoom()
    {
        Vector2 zoomPosition = (fstFinger + sndFinger) / 2;
        float distance = Vector2.Distance(fstFinger, sndFinger);

        //zooming in or out
        float zoomDelta = distance - previousDistance;
        previousDistance = distance;


        transform.localScale *= zoomDelta * zoomSpeed;
        transform.position = zoomPosition;

    }

    public void OnFirstFinger(InputAction.CallbackContext context)
    {
        fstFinger = context.ReadValue<Vector2>();
    }

    public void OnSecondFinger(InputAction.CallbackContext context)
    {
        sndFinger = context.ReadValue<Vector2>();

        if (context.phase == InputActionPhase.Started)
        {
            isPinching = true;
            return;
        }

        if (context.canceled)
        {
            isPinching = false;
            return;
        }
    }
}
