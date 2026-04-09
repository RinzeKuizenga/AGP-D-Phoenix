using UnityEngine;

public class StitchPath : MonoBehaviour
{
    public Transform[] stitchPoints;
    public float stitchRadius = 0.3f;

    private int currentStitchIndex = 0;
    private bool isComplete = false;
    private bool isNearCurrent = false; // are we hovering the current target point?

    public bool TryStitch(Vector2 inputLocalPos)
    {
        if (isComplete || currentStitchIndex >= stitchPoints.Length) return false;

        // Use localPosition since everything is in canvas space
        Vector2 pointLocalPos = stitchPoints[currentStitchIndex].localPosition;
        float dist = Vector2.Distance(inputLocalPos, pointLocalPos);

        if (dist <= stitchRadius)
        {
            if (!isNearCurrent)
            {
                isNearCurrent = true;
                currentStitchIndex++;
                if (currentStitchIndex >= stitchPoints.Length)
                    isComplete = true;
                return true;
            }
        }
        else
        {
            isNearCurrent = false;
        }

        return false;
    }

    void OnDrawGizmos()
    {
        if (stitchPoints == null) return;
        for (int i = 0; i < stitchPoints.Length; i++)
        {
            if (stitchPoints[i] == null) continue;
            Gizmos.color = (i == currentStitchIndex) ? Color.green : Color.red;
            Gizmos.DrawWireSphere(stitchPoints[i].position, stitchRadius);
        }
    }

    public int GetCurrentTarget() => currentStitchIndex;
    public float GetProgress() => (float)currentStitchIndex / stitchPoints.Length;
    public bool IsComplete() => isComplete;
}