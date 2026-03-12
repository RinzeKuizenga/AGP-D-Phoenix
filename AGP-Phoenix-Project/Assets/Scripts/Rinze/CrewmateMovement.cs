using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class CrewmateMovement : MonoBehaviour
{
    Transform target;
    [SerializeField] float speed = 1f;

    public bool isSelected = false;

    private void Update()
    {
        if (isSelected && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Yep");
            Camera.main.GetComponent<CameraOrbiter>().ZoomOut();
        }
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
}