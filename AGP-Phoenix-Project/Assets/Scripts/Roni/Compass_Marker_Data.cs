using System;
using UnityEngine;

/// <summary>
/// Serializable data for configuring compass markers in the Inspector.
/// </summary>
[Serializable]
public class Compass_Marker_Data
{
    [Tooltip("The transform this marker tracks.")]
    public Transform target;

    [Tooltip("Color of the marker dot when inactive.")]
    public Color color = Color.white;
}