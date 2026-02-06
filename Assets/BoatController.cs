using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float motorForce = 25f;
    public float turnSpeed = 50f;
    public float maxSpeed = 20f;

    [Header("Water Resistance")]
    public float waterResistance = 0.95f; 

    private Rigidbody rb;
    private float currentMotorInput;
    private float currentTurnInput;

    [Header("Boost Functions")]
    public Camera cam;
    public float normalFOV = 60f;
    public float boostFOV = 75f;
    public float fovTransitionSpeed = 60f;

    private float targetFOV;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetFOV = normalFOV;
    }

    void Update()
    {
        currentMotorInput = 0f;
        currentTurnInput = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            print("Worky?");
            currentMotorInput = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
            currentMotorInput = -1f;

        if (Input.GetKey(KeyCode.A))
            currentTurnInput = -1f;
        else if (Input.GetKey(KeyCode.D))
            currentTurnInput = 1f;
    }

    void FixedUpdate()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.fixedDeltaTime * fovTransitionSpeed);

        if (Mathf.Abs(currentMotorInput) > 0.01f && Input.GetKey(KeyCode.LeftShift))
        {
            targetFOV = boostFOV;
            Vector3 forwardForce = transform.forward * currentMotorInput * motorForce * 2;
            rb.AddForce(forwardForce, ForceMode.Force);
        }
        else if (Mathf.Abs(currentMotorInput) > 0.01f)
        {
            targetFOV = normalFOV;
            Vector3 forwardForce = transform.forward * currentMotorInput * motorForce;
            rb.AddForce(forwardForce, ForceMode.Force);
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        if (rb.linearVelocity.magnitude > 0.5f)
        {
            float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / 5f);
            float turn = currentTurnInput * turnSpeed * speedFactor * Time.fixedDeltaTime;

            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        rb.linearVelocity *= waterResistance;
    }
}