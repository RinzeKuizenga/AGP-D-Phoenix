using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraOrbiter : MonoBehaviour
{
    public Transform cubeTransform;
    private float mouseX = 0f;
    private float mouseY = 0f;
    public float sensitivity = 5f;
    private float orbitRadius = 5f;
    private float minimumOrbitDistance = 15f;
    private float maximumOrbitDistance = 50f;

    [Header("Side View Settings")]
    public Vector3 sideViewOffset = new Vector3(30f, 5f, 0f); // Offset from ship for side view
    public float transitionSpeed = 5f; // Speed of camera transition

    private bool isInSideView = false;
    public bool IsInSideView => isInSideView;
    private Vector3 savedOrbitPosition;
    private Quaternion savedOrbitRotation;
    private float savedOrbitRadius;
    private float targetOrbitRadius;

    [Header("Spatial Audios")]
    [SerializeField] AudioLowPassFilter oceanAmbience;

    [Header("Objects")]
    [SerializeField] GameObject waterCutter;

    void Start()
    {
        targetOrbitRadius = orbitRadius;
    }

    void Update()
    {
        if (!isInSideView)
        {
            // Normal orbit behavior
            if (Input.GetMouseButton(1))
            {
                transform.LookAt(cubeTransform);
                if (Input.GetMouseButton(1) && !EventSystem.current.IsPointerOverGameObject())
                {
                    mouseX = Input.GetAxis("Mouse X");
                    mouseY = Input.GetAxis("Mouse Y");
                    transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);
                }
            }
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) > 0.001f)
            {
                float zoomSpeed = 0.5f;
                targetOrbitRadius *= 1f - scroll * zoomSpeed;
                targetOrbitRadius = Mathf.Clamp(targetOrbitRadius, minimumOrbitDistance, maximumOrbitDistance);
            }
            orbitRadius = Mathf.Lerp(orbitRadius, targetOrbitRadius, Time.deltaTime * 10f);
            transform.position = cubeTransform.position - transform.forward * orbitRadius;
        }
        else
        {
            // Side view - follow ship position but stabilize rotation
            Vector3 targetPosition = cubeTransform.position + sideViewOffset;

            // Keep camera level (don't follow boat's pitch/roll)
            Quaternion stabilizedRotation = Quaternion.Euler(0f, cubeTransform.eulerAngles.y, 0f);
            Vector3 offsetDirection = stabilizedRotation * Vector3.right;
            targetPosition = cubeTransform.position + offsetDirection * sideViewOffset.x + Vector3.up * sideViewOffset.y;

            // Look at boat but keep camera level
            Vector3 lookDirection = cubeTransform.position - targetPosition;
            lookDirection.y = 0; // Flatten to prevent camera tilting
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * transitionSpeed);
        }
    }

    public void ToggleSideView()
    {
        if (!isInSideView)
        {
            // Switch to side view
            savedOrbitPosition = transform.position;
            savedOrbitRotation = transform.rotation;
            savedOrbitRadius = orbitRadius;
            isInSideView = true;
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 4.5f;
            oceanAmbience.cutoffFrequency = 1908f;
            waterCutter.SetActive(true);
            //Camera.main.nearClipPlane = 3.44f;
        }
        else
        {
            // Return to orbit view
            oceanAmbience.cutoffFrequency = 22000f;
            StartCoroutine(ReturnToOrbitView());
            waterCutter.SetActive(false);
        }
    }

    private IEnumerator ReturnToOrbitView()
    {
        isInSideView = false;
        Camera.main.orthographic = false;
        Camera.main.nearClipPlane = 0.57f;
        float elapsedTime = 0f;
        float duration = 1f / transitionSpeed;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, savedOrbitPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, savedOrbitRotation, t);

            yield return null;
        }

        transform.position = savedOrbitPosition;
        transform.rotation = savedOrbitRotation;
        orbitRadius = savedOrbitRadius;
    }
}