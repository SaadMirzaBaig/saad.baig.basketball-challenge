using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BallController : MonoBehaviour
{
    public PhysicMaterial ballPhysicsMaterial;

    [Header("Ball State")]
    public bool isInFlight = false;
    public bool hasScored = false;

    private Rigidbody rb;
    private Vector3 startPosition;

    public static Action<BallController> OnBallShotEvent;

    private void Awake()
    {
        SetupBallComponents();
        startPosition = transform.position;
    }

    private void SetupBallComponents()
    {
        // Setup Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.mass = 0.6f; // Standard basketball mass
        rb.drag = 0; // Basketball drag coefficient
        rb.useGravity = true;

        // Setup Collider
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.12f; // Standard basketball radius
        }

        // Apply physics material if provided
        if (ballPhysicsMaterial != null)
        {
            sphereCollider.material = ballPhysicsMaterial;
        }

        // Set tag for identification
        gameObject.tag = Constants.BALL_TAG;
    }


    private void FixedUpdate()
    {
        if (isInFlight)
        {

            // Check if ball has fallen too low or stopped
            if (transform.position.y < -2f  || transform.position.z > 5.5f )
            {

                Debug.Log("Ball missed resetting the position");
                ResetBall();
            }
        }
    }

 
    public void OnBallScored()
    {

        hasScored = true;

        Debug.Log("GOAL! Ball scored!");

        // Reset ball after short delay
        Invoke(nameof(ResetBall), 2f);
    }
    
    public void ResetBall()
    {
        // Reset position
        transform.position = startPosition;

        // Reset physics
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset state
        isInFlight = false;
        hasScored = false;

        Debug.Log("Ball reset to starting position");
    }

    public bool IsInFlight => isInFlight;
    public bool HasScored => hasScored;
}