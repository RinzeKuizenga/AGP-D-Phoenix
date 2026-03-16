using System;
using UnityEngine;

/// <summary>
/// Serializable data for configuring compass markers in the editor.
/// </summary>
[Serializable]
public class Compass_Marker_Data
{
    public Transform target;
    public Texture2D icon;
    public Color color = Color.white;

}