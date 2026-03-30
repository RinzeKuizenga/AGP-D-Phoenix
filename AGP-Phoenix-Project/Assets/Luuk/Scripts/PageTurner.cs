using UnityEngine;

public class PageTurner : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject page1;
    [SerializeField] public GameObject page2;
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
              
                BackButton.SetActive(true);
                page2.SetActive(true);
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
                page1.SetActive(true);
                timer = 0f;
                pageBackCounter = false;
            }
        }
    }


    public void BookPress()
    {
        animator.SetTrigger("Page1");
        NextPageButton.SetActive(false);
        page1.SetActive(false);
        isCounting = true;

    }
    public void PageBack()
    {
        animator.SetTrigger("PageBack");
        page2.SetActive(false);
        BackButton.SetActive(false);
        pageBackCounter = true;
       

    }
}
