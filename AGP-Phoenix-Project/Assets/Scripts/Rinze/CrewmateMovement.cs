using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class CrewmateMovement : MonoBehaviour
{
    Transform target;
    [SerializeField] float speed = 1f;

    [SerializeField] CanvasGroup popupScreen;
    [SerializeField] float targetAlpha;

    public bool isSelected = false;

    private void Update()
    {
        popupScreen.alpha = Mathf.Lerp(popupScreen.alpha, targetAlpha, Time.deltaTime * 8f);
        if (target == null) return;
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

    }

    public void MoveToArea(Transform newTarget)
    {
        if (!isSelected) return;

        target = newTarget;
        isSelected = false;

    }


    void OnMouseDown()
    {
        Debug.Log("Ribbit");
        isSelected = true;
        Camera.main.GetComponent<CameraOrbiter>().ZoomIn(this.transform);
    }

    void OnMouseEnter()
    {
        targetAlpha = 1f;
    }

    void OnMouseExit()
    {
        targetAlpha = 0f;
    }
}