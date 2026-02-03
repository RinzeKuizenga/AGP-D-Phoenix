using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float motorForce = 25f;
    public float turnSpeed = 50f;
    public float maxSpeed = 20f;

    [Header("Water Resistance")]
    public float waterResistance = 0.95f; // Slows down the boat naturally

    private Rigidbody rb;
    private float currentMotorInput;
    private float currentTurnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get input using KeyCode for instant response
        currentMotorInput = 0f;
        currentTurnInput = 0f;

        // Forward/Backward
        if (Input.GetKey(KeyCode.W))
        {
            print("Worky?");
            currentMotorInput = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
            currentMotorInput = -1f;

        // Left/Right
        if (Input.GetKey(KeyCode.A))
            currentTurnInput = -1f;
        else if (Input.GetKey(KeyCode.D))
            currentTurnInput = 1f;
    }

    void FixedUpdate()
    {
        // Apply forward/backward force
        if (Mathf.Abs(currentMotorInput) > 0.01f)
        {
            Vector3 forwardForce = transform.forward * currentMotorInput * motorForce;
            rb.AddForce(forwardForce, ForceMode.Force);
        }

        // Limit max speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Apply steering (only when moving)
        if (rb.linearVelocity.magnitude > 0.5f)
        {
            // Turn based on speed (boats turn better when moving)
            float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / 5f);
            float turn = currentTurnInput * turnSpeed * speedFactor * Time.fixedDeltaTime;

            // Apply rotation
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // Apply water resistance to slow down naturally
        rb.linearVelocity *= waterResistance;
    }
}