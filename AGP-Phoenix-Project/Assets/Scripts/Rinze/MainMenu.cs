using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Animator titleScreen;
    void Start()
    {
        titleScreen = GetComponent<Animator>(); 
    }

    public void buttonPress()
    {
        titleScreen.SetTrigger("Start");
    }
}
