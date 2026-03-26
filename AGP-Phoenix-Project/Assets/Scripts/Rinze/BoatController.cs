using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public BoatHullManager boatHullManager;
    
    [Header("Boost Functions")]
    public Camera cam;
    public float normalFOV = 60f;
    public float boostFOV = 75f;
    public float fovTransitionSpeed = 60f;
    private float targetFOV;

    [Header("Wind Settings")]
    private float windTurn = 0f;
    public TextMeshProUGUI windText;
    public ParticleSystem windParticles;
    public Transform windposLeft;
    public Transform windposRight;

    public ParticleSystem rainParticles;
    public Transform rainPos;

    public Image windArrow;

    [Header("Boat Animations")]
    public Animator animator;

    [Header("Sound Effects")]
    [SerializeField] AudioClip sailUp;
    [SerializeField] AudioClip sailOut;
    [SerializeField] AudioClip sailSnap;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        boatHullManager = GetComponent<BoatHullManager>();
        windTurn = Random.Range(-3f, 3);
        targetFOV = normalFOV;
    }

    void Update()
    {
        currentMotorInput = 0f;
        currentTurnInput = 0f;

        animator.SetBool("SailDown", Input.GetKey(KeyCode.W));
        if (Input.GetKey(KeyCode.W))
        {
            currentMotorInput = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentMotorInput = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            currentTurnInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentTurnInput = 1f;
        }
        windText.text = $"{windTurn:F1}m/s";
    }

    void FixedUpdate()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.fixedDeltaTime * fovTransitionSpeed);

        if (Mathf.Abs(currentMotorInput) > 0.01f && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            targetFOV = boostFOV;
            animator.SetBool("SailBoost", true);
            Vector3 forwardForce = transform.forward * currentMotorInput * motorForce * 2;
            rb.AddForce(forwardForce * boatHullManager.SpeedMultiplier, ForceMode.Force);
        }
        else if (Mathf.Abs(currentMotorInput) > 0.01f)
        {
            targetFOV = normalFOV;
            animator.SetBool("SailBoost", false);
            Vector3 forwardForce = transform.forward * currentMotorInput * motorForce;
            rb.AddForce(forwardForce * boatHullManager.SpeedMultiplier, ForceMode.Force);
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

        Quaternion windRotation = Quaternion.Euler(0f, windTurn * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * windRotation);
        rb.linearVelocity *= waterResistance;

        if (windTurn > 1)
        {
            windParticles.transform.position = windposLeft.position;
            windParticles.transform.rotation = transform.rotation * Quaternion.Euler(90f, 0f, 0f);
            windArrow.rectTransform.localEulerAngles = new Vector3(0f, 180f, 90f);
            if (!windParticles.isPlaying) windParticles.Play();
        }
        else if (windTurn < -1)
        {
            windParticles.transform.position = windposRight.position;
            windArrow.rectTransform.localEulerAngles = new Vector3(0f, -180f, 90f);
            if (!windParticles.isPlaying) windParticles.Play();
        }
        else
        {
            windParticles.Stop();
        }
        rainParticles.transform.position = rainPos.position;
    }

    public void WindChange()
    {
        windTurn = Random.Range(-3f, 3);
    }

    public void PlaySailOutSound()
    {
        AudioManager.Instance.PlaySFX(sailOut, 1f);
    }
    public void PlaySailInSound()
    {
        AudioManager.Instance.PlaySFX(sailUp, 1f);
    }

    public void PlaySailSnapSound()
    {
        AudioManager.Instance.PlaySFX(sailSnap, 1f);
    }
}