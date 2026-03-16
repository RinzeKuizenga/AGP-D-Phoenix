using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{

    [Header("References")]
    [Tooltip("The player boat transform (used for relative spawn/despawn distances)")]
    public Transform player;

    [Header("Plank Prefabs  (small debris)")]
    public GameObject[] plankPrefabs;

    [Header("Wreck Prefabs  (large obstacles)")]
    public GameObject[] wreckPrefabs;

    [Header("Spawn Area")]
    [Tooltip("How far ahead of the player to spawn new debris")]
    public float spawnDistanceAhead = 60f;

    [Tooltip("Half-width of the spawn lane (objects spawn within ±laneHalfWidth on the Z axis)")]
    public float laneHalfWidth = 12f;

    [Tooltip("Objects that fall this far BEHIND the player are destroyed")]
    public float despawnDistanceBehind = 30f;

    [Header("Spawn Rates (seconds between spawns)")]
    public float plankSpawnInterval  = 1.5f;
    public float wreckSpawnInterval  = 6f;

    [Header("Cluster Settings  (planks often appear in groups)")]
    [Tooltip("Min planks per cluster")]
    public int minPlanksPerCluster = 2;
    [Tooltip("Max planks per cluster")]
    public int maxPlanksPerCluster = 5;
    [Tooltip("Spread radius of a cluster")]
    public float clusterRadius = 3f;
    
    private List<GameObject> _activeDebris = new List<GameObject>();
    private float _plankTimer;
    private float _wreckTimer;
    private float _difficultyTimer;
    private float _currentPlankInterval;
    private float _currentWreckInterval;

    void Start()
    {
        if (player == null)
            Debug.LogError("[DebrisSpawner] No player Transform assigned!");

        _currentPlankInterval = plankSpawnInterval;
        _currentWreckInterval = wreckSpawnInterval;

        // Stagger the first spawns so they don't all fire at frame 0
        _plankTimer      = Random.Range(0f, _currentPlankInterval);
        _wreckTimer      = Random.Range(0f, _currentWreckInterval);
    }

    void Update()
    {
        if (player == null) return;

        _plankTimer      += Time.deltaTime;
        _wreckTimer      += Time.deltaTime;

        // Spawn planks
        if (_plankTimer >= _currentPlankInterval)
        {
            _plankTimer = 0f;
            SpawnPlankCluster();
        }

        // Spawn wrecks
        if (_wreckTimer >= _currentWreckInterval)
        {
            _wreckTimer = 0f;
            SpawnWreck();
        }
        // Despawn debris that have drifted behind the player
        DespawnOldDebris();
    }

    private void SpawnPlankCluster()
    {
        if (plankPrefabs == null || plankPrefabs.Length == 0) return;

        Vector3 clusterOrigin = GetRandomSpawnPoint();
        int count = Random.Range(minPlanksPerCluster, maxPlanksPerCluster + 1);

        for (int i = 0; i < count; i++)
        {
            // Spread planks along the player's LOCAL right and forward axes
            // so the cluster always fans out perpendicular/parallel to travel direction
            Vector3 offset = (player.right   * Random.Range(-clusterRadius, clusterRadius))
                           + (player.forward * Random.Range(-clusterRadius * 0.5f, clusterRadius * 0.5f));
            offset.y = 0f;

            SpawnDebris(plankPrefabs, clusterOrigin + offset);
        }
    }
    
    private void SpawnWreck()
    {
        if (wreckPrefabs == null || wreckPrefabs.Length == 0) return;
        SpawnDebris(wreckPrefabs, GetRandomSpawnPoint());
    }
    
    private void SpawnDebris(GameObject[] prefabs, Vector3 position)
    {
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        // Random Y rotation so debris isn't all identically oriented
        Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        GameObject obj = Instantiate(prefab, position, rotation);
        _activeDebris.Add(obj);
    }

    private Vector3 GetRandomSpawnPoint()
    {
        // Start from the player
        Vector3 pos = player.position;

        // Push ahead along the player's facing direction
        pos += player.forward * spawnDistanceAhead;

        // Spread LEFT/RIGHT across the full lane width using the player's right axis
        pos += player.right * Random.Range(-laneHalfWidth, laneHalfWidth);

        // Add a small random depth offset so successive spawns don't all land
        // on the exact same forward distance — makes the field feel more natural
        pos += player.forward * Random.Range(-spawnDistanceAhead * 0.15f, spawnDistanceAhead * 0.15f);

        pos.y = 0f;
        return pos;
    }
    
    private void DespawnOldDebris()
    {
        for (int i = _activeDebris.Count - 1; i >= 0; i--)
        {
            if (_activeDebris[i] == null)
            {
                _activeDebris.RemoveAt(i);
                continue;
            }

            // Project the debris position onto the player's forward axis
            Vector3 toDebris = _activeDebris[i].transform.position - player.position;
            float forwardDot = Vector3.Dot(toDebris, player.forward);

            if (forwardDot < -despawnDistanceBehind)
            {
                Destroy(_activeDebris[i]);
                _activeDebris.RemoveAt(i);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Show spawn zone
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f);
        Vector3 spawnCenter = player.position + player.forward * spawnDistanceAhead;
        Gizmos.DrawCube(spawnCenter, new Vector3(laneHalfWidth * 2f, 1f, 4f));

        // Show despawn line
        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Vector3 despawnCenter = player.position - player.forward * despawnDistanceBehind;
        Gizmos.DrawCube(despawnCenter, new Vector3(laneHalfWidth * 2f, 1f, 4f));
    }
}