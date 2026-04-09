using UnityEngine;
using UnityEngine.UI;

public class Bucket_MG_Script : MonoBehaviour
{
    private string roomName = "RoomPrefab";
    private Image targetImage;
    private Image fireImage;
    private Slider sliderRope;
    private Bucket_Winch Bucket_Winch;
    private RoomHealth roomHealth;
    public Sprite emptyBucket;
    public Sprite fullBucket;
    private int filledCountAmount;
    private bool bucketFilled = false;

    private void Awake()
    {
        Bucket_Winch = GetComponentInChildren<Bucket_Winch>(true);
        sliderRope = GetComponentInChildren<Slider>(true);

        foreach (Image img in GetComponentsInChildren<Image>(true))
        {
            if (img.gameObject.name == "BucketImage") targetImage = img;
            if (img.gameObject.name == "FireImage") fireImage = img;
        }

        Debug.Log("Winch: " + Bucket_Winch);
        Debug.Log("Slider: " + sliderRope);
        Debug.Log("TargetImage: " + targetImage);
        Debug.Log("FireImage: " + fireImage);
        Debug.Log("EmptyBucket: " + emptyBucket);
        Debug.Log("FullBucket: " + fullBucket);
    }

    private void Start()
    {
        fireImage.fillAmount = 1;
        roomHealth = GameObject.Find(roomName)?.GetComponent<RoomHealth>();
        Debug.Log("RoomHealth: " + roomHealth);
    }

    private void Update()
    {
        if (Bucket_Winch == null || sliderRope == null || fireImage == null || targetImage == null) return;

        sliderRope.value = Bucket_Winch.FullRotations;

        if (sliderRope.value == 5 && !bucketFilled)
            Change2Full();

        if (sliderRope.value == -5 && bucketFilled)
        {
            Change2Empty();
            Debug.Log(filledCountAmount);
        }

        if (filledCountAmount == 5)
        {
            roomHealth.ResetHealth();
            gameObject.SetActive(false);
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