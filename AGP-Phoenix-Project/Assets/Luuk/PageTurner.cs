using UnityEngine;

public class PageTurner : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject CrewOption1;
    [SerializeField] public GameObject CrewOption2;
    [SerializeField] public GameObject CrewOption3;
    [SerializeField] public GameObject CrewOption4;
    [SerializeField] public GameObject CrewOption5;
    [SerializeField] public GameObject CrewOption6;
    [SerializeField] public GameObject CrewOption7;
    [SerializeField] public GameObject CrewOption8;
    [SerializeField] public GameObject CrewOption9;
    [SerializeField] public GameObject NextPageButton;
    [SerializeField] public GameObject BackButton;
    [SerializeField] private float disableTime = 4.5f;
    private float timer = 0f;
    private bool isCounting = false;
    private bool pageBackCounter = false;

    void Update()
    {
        if (isCounting)
        {
            print("ik doe het");
            timer += Time.deltaTime;

            if (timer >= disableTime)
            {
               NextPageButton.SetActive(true);
                BackButton.SetActive(true);
                CrewOption4.SetActive(true);
                CrewOption5.SetActive(true);
                CrewOption6.SetActive(true);
                CrewOption7.SetActive(true);
                CrewOption8.SetActive(true);
                CrewOption9.SetActive(true);
                timer = 0f;
                isCounting = false;
            }
        }
        if (pageBackCounter)
        {
            timer += Time.deltaTime;
            if (timer >= disableTime)
            {
                NextPageButton.SetActive(true);
                CrewOption1.SetActive(true);
                CrewOption2.SetActive(true);
                CrewOption3.SetActive(true);
                timer = 0f;
                isCounting = false;
            }
        }
    }


    public void BookPress()
    {
        animator.SetTrigger("Page1");
        CrewOption1.SetActive(false);
        CrewOption2.SetActive(false);
        CrewOption3.SetActive(false);
        NextPageButton.SetActive(false);
        isCounting = true;

    }
    public void PageBack()
    {
        animator.SetTrigger("PageBack");
        CrewOption4.SetActive(false);
        CrewOption5.SetActive(false);
        CrewOption6.SetActive(false);
        CrewOption7.SetActive(false);
        CrewOption8.SetActive(false);
        CrewOption9.SetActive(false);
        BackButton.SetActive(false);
        NextPageButton.SetActive(false);
        pageBackCounter = true;
       

    }
}
