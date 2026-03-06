using UnityEngine;
using UnityEngine.UI;

public class EyeButton : MonoBehaviour
{
    [SerializeField] public Image backgroundStar;
    [SerializeField] public Image buttonImg;

    [SerializeField] public Sprite crossedEye;
    [SerializeField] public Sprite crossedEyeYellow;
    [SerializeField] public Sprite eye;
    [SerializeField] public Sprite eyeYellow;

    [SerializeField] CameraOrbiter cameraOrbiter;
    void FixedUpdate()
    {
        backgroundStar.rectTransform.Rotate(0f, 0f, 20f * Time.deltaTime);
    }

    public void SpriteHover()
    {
        if (cameraOrbiter.IsInSideView)
        {
            buttonImg.sprite = crossedEyeYellow;
        }
        else
        {
            buttonImg.sprite = eyeYellow;
        }
    }

    public void SpriteChanger()
    {
        if (cameraOrbiter.IsInSideView)
        {
            buttonImg.sprite = crossedEye;
        }
        else
        {
            buttonImg.sprite = eye;
        }
    }

}
