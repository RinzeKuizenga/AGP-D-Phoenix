using UnityEngine;

public class CrewAnimationSwapper : MonoBehaviour
{
    [SerializeField] int slotIndex;
    [SerializeField] SpriteRenderer spriteRenderer;

    private Sprite[] crewSprites;
    private bool shouldSwap = false;

    void Awake()
    {
        Crew crew = CrewRoster.SelectedCrew[slotIndex];
        if (crew == null || crew.animationSprites == null || crew.animationSprites.Length == 0) return;

        if (crew.crewMaterial != null)
            spriteRenderer.material = new Material(crew.crewMaterial);

        if (crew.animationSprites[0].name.StartsWith("HaroldSprite")) return;

        crewSprites = crew.animationSprites;
        shouldSwap = true;
    }

    void LateUpdate()
    {
        if (!shouldSwap || crewSprites == null) return;

        string spriteName = spriteRenderer.sprite.name;
        int underscoreIndex = spriteName.LastIndexOf('_');
        if (underscoreIndex < 0) return;

        if (int.TryParse(spriteName.Substring(underscoreIndex + 1), out int index))
        {
            if (index < crewSprites.Length)
                spriteRenderer.sprite = crewSprites[index];
        }
    }
}