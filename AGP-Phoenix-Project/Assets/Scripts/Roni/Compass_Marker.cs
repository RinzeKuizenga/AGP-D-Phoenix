using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Runtime representation of a single waypoint marker on the compass.
/// </summary>
public class Compass_Marker
{
    /// <summary>The world-space transform this marker tracks.</summary>
    public Transform target;

    /// <summary>Color of the marker dot.</summary>
    public Color color;

    /// <summary>Root UI element for this marker.</summary>
    public VisualElement element;

    /// <summary>The colored dot element.</summary>
    public VisualElement dot_element;

    /// <summary>Label showing distance to target (only visible on active marker).</summary>
    public Label distance_label;

    /// <summary>Cached visibility state to avoid repeated class toggles.</summary>
    public bool is_visible = true;

    /// <summary>Cached rounded distance for change detection.</summary>
    public int cached_distance = -1;

    public Compass_Marker(Transform target, Color color)
    {
        this.target = target;
        this.color = color;
    }
}