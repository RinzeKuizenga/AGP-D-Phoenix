using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public bool isAnimating { get; private set; }
    public bool IsLocked => isAnimating;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetAnimating(bool state)
    {
        isAnimating = state;
    }

}
