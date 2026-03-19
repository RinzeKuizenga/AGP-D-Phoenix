using UnityEngine;
using UnityEngine.UI;

public class Nail_Minigame : MonoBehaviour
{
    public Image targetImage;
    public Sprite newSprite;
    public int colliderID;

    public GameObject parentObject;

    public void ChangeSprite()
    {
        targetImage.sprite = newSprite;

        if (colliderID == 2)
        {
            foreach (Transform child in parentObject.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}