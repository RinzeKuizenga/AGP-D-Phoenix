using System.Collections;
using UnityEngine;

public class StorageMGmanager : MonoBehaviour
{
    private RoomHealth roomHealth;
    [SerializeField] private string roomName;
    private void Start()
    {
        roomHealth = GameObject.Find(roomName).GetComponent<RoomHealth>();
    }
    public GameObject storageMinigame;

    public int nailsLeft = 10;
    private bool gameFinished = false;

    public void OnClicked()
    {
        nailsLeft--;
    }

    void Update()
    {
        Debug.Log(nailsLeft);

        if (nailsLeft == 0 && !gameFinished)
        {
            gameFinished = true;
            StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);

        Debug.Log("Storage MiniGame completed!!!");
        roomHealth.ResetHealth();
        Destroy(storageMinigame);
        storageMinigame.SetActive(false);
    }
}