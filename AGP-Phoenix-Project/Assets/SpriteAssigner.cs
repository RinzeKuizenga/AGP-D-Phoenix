using UnityEngine;

public class SpriteAssigner : MonoBehaviour
{
    public int skinNR;

    public Skins[] skins;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        
    }

    private void LateUpdate()
    {
        SkinChoice();
    }

    void SkinChoice()
    {
        if (spriteRenderer.sprite.name.Contains("HaroldSprite"))
        {
            string spriteName = spriteRenderer.sprite.name;
            spriteName = spriteName.Replace("HaroldSprite", "");
            int spriteNr = int.Parse(spriteName);

            spriteRenderer.sprite = skins[skinNR].sprites[spriteNr];
        }
    }

    [System.Serializable]
    public struct Skins
    {
        public Sprite[] sprites;
    }
}
