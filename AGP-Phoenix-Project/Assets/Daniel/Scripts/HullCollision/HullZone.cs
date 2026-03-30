using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class HullZone : HullSection
{
    void Reset()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    void OnDrawGizmos()
    {
        // Always draw the zone box so you can see it while placing zones in the editor
        var col = GetComponent<BoxCollider>();
        if (col == null) return;

        Color zoneColor = State switch
        {
            DamageState.Intact    => new Color(0f,   1f,   0f,   0.15f),
            DamageState.Damaged   => new Color(1f,   0.6f, 0f,   0.25f),
            DamageState.Critical  => new Color(1f,   0f,   0f,   0.35f),
            DamageState.Destroyed => new Color(0.1f, 0.1f, 0.1f, 0.5f),
            _                     => new Color(0f,   1f,   0f,   0.15f)
        };

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = zoneColor;
        Gizmos.DrawCube(col.center, col.size);

        // Outline
        Gizmos.color = zoneColor + new Color(0, 0, 0, 0.4f);
        Gizmos.DrawWireCube(col.center, col.size);

        // Label
        Gizmos.matrix = Matrix4x4.identity;

#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            transform.TransformPoint(col.center + Vector3.up * (col.size.y * 0.5f + 0.3f)),
            $"{sectionName}\n{currentHealth:F0}/{maxHealth:F0} HP",
            new GUIStyle { normal = { textColor = Color.white }, fontSize = 11, fontStyle = FontStyle.Bold }
        );
#endif
    }
}