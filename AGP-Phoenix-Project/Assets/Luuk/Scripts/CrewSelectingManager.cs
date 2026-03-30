using System.Collections.Generic;
using UnityEngine;

public class CrewSelectingManager : MonoBehaviour
{
    [SerializeField] RectTransform[] slots = new RectTransform[5];
    [SerializeField] Crew[] allCrew;
    [SerializeField] GameObject[] crewPrefabs;
    [SerializeField]CharacterButton[] allButtons;

    private GameObject[] slotContents = new GameObject[5];
    private Crew[] slotCrew = new Crew[5];

    public void SelectCrewMember(GameObject crewImage, Crew crewData, bool isRandom = false)
    {
        for (int i = 0; i < slotContents.Length; i++)
        {
            if (slotContents[i] == crewImage)
            {
                slotContents[i] = null;
                slotCrew[i] = null;
                crewImage.transform.SetParent(crewImage.GetComponent<CharacterButton>().originalParent);
                crewImage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;
            }
        }

        if (!isRandom)
        {
            int filledCount = 0;
            foreach (GameObject slot in slotContents)
            {
                if (slot != null) filledCount++;
            }
            if (filledCount >= 3)
            {
                Debug.Log("You can only select 3 crew members!");
                return;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slotContents[i] == null)
            {
                slotContents[i] = crewImage;
                slotCrew[i] = crewData;
                crewImage.transform.SetParent(slots[i]);
                crewImage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;
            }
        }
    }
    public void FillEmptySlotsRandomly()
    {
        // Get all character buttons in scene

        List<Crew> pool = new List<Crew>();
        foreach (Crew c in allCrew)
        {
            bool alreadyPicked = false;
            foreach (Crew picked in slotCrew)
            {
                if (picked == c) { alreadyPicked = true; break; }
            }
            if (!alreadyPicked) pool.Add(c);
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slotContents[i] == null && pool.Count > 0)
            {
                int rand = Random.Range(0, pool.Count);
                Crew randomCrew = pool[rand];
                pool.RemoveAt(rand);

                // Find the matching button in the scene
                foreach (CharacterButton button in allButtons)
                {
                    if (button.crewData == randomCrew)
                    {
                        
                        SelectCrewMember(button.gameObject, randomCrew, true);
                        break;
                    }
                }
            }
        }
    }

}
