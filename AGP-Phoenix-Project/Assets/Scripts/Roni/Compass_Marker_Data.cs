using System;
using UnityEngine;

[Serializable]
public class Compass_Marker_Data
{
    [Tooltip("The transform this marker tracks.")]
    public Transform target;

    [Tooltip("Display name shown when this is the active destination.")]
    public string destination_name = "Bestemming";

    [Tooltip("Color of the marker dot when inactive.")]
    public Color color = Color.white;
}
