using UnityEngine;
using UnityEngine.UIElements;


public class Compass_Marker
{
    public Transform target;
    public Color color;
    public VisualElement element;
    public VisualElement dot_element;
    public Label distance_label;
    public bool is_visible = true;
    public int cached_distance = -1;

    public Compass_Marker(Transform target, Color color)
    {
        this.target = target;
        this.color = color;
    }
}