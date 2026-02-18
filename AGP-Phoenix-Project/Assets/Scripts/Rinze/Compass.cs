using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    public RectTransform compassBarTransform;
    public RectTransform northMarkerTransform;
    public RectTransform southMarkerTransform;
    public RectTransform eastMarkerTransform;
    public RectTransform westMarkerTransform;
    public Transform cameraObjectTransform;

    void Update()
    {
        SetMarkerPosition(northMarkerTransform, cameraObjectTransform.position + Vector3.forward * 1000);
        SetMarkerPosition(southMarkerTransform, cameraObjectTransform.position + Vector3.back * 1000);
        SetMarkerPosition(eastMarkerTransform, cameraObjectTransform.position + Vector3.right * 1000);
        SetMarkerPosition(westMarkerTransform, cameraObjectTransform.position + Vector3.left * 1000);
    }

    private void SetMarkerPosition(RectTransform markerTransform, Vector3 worldPosition)
    {
        Vector3 directionToTarget = worldPosition - cameraObjectTransform.position;
        float signedAngle = Vector3.SignedAngle(new Vector3(cameraObjectTransform.forward.x, 0, cameraObjectTransform.forward.z), new Vector3(directionToTarget.x, 0, directionToTarget.z), Vector3.up);
        float compassPosition = Mathf.Clamp(signedAngle / Camera.main.fieldOfView, -0.5f, 0.5f);
        markerTransform.anchoredPosition = new Vector2(compassBarTransform.rect.width * compassPosition, 0);
    }
}