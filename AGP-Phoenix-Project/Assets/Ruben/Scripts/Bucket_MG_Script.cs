using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bucket_MG_Script : MonoBehaviour
{
    public Image targetImage;
    public Sprite emptyBucket;
    public Sprite fullBucket;
    public Slider sliderRope;
    public GameObject bucketMinigame;
    public Image fireImage;

    private int filledCountAmount;
    private bool bucketFilled = false;

    public Bucket_Winch Winch_BMG;
    private RoomHealth roomHealth;
    [SerializeField] private string roomName;
    private void Start()
    {
        fireImage.fillAmount = 1;
        roomHealth = GameObject.Find(roomName).GetComponent<RoomHealth>();
    }
    private void Update()
    {
        sliderRope.value = Winch_BMG.FullRotations;
        if (sliderRope.value == 5 && !bucketFilled)
        {
            Change2Full();   
        }
        if (sliderRope.value == -5 && bucketFilled)
        {
            Change2Empty();
            Debug.Log(filledCountAmount);
        }
        if (filledCountAmount == 5)
        {
            roomHealth.ResetHealth(); 
            Destroy(bucketMinigame);
            bucketMinigame.SetActive(false);
        }
    }
    public void Change2Empty()
    {
        filledCountAmount++;
        fireImage.fillAmount -= 0.2f;
        bucketFilled = false;
        targetImage.sprite = emptyBucket;
    }
    public void Change2Full()
    {
        bucketFilled = true;
        targetImage.sprite = fullBucket;
    }
}
