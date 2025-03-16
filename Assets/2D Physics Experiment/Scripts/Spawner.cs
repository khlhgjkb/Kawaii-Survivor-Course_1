using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spawner : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private TextMeshPro objectsSpawnedText;

    [Header(" Settings ")]
    [SerializeField] private bool enableSpawning;
    [SerializeField] private int spawnsPerFrame;
    [SerializeField] private int maxCount;
    [SerializeField] private Vector2 spawnBounds;

    // Update is called once per frame
    void Update()
    {
        if (!enableSpawning)
            return;

        if (transform.childCount >= maxCount)
            return;

        for (int i = 0; i < spawnsPerFrame; i++)
        {
            SpawnPrefab();

            if (transform.childCount >= maxCount)
                return;
        }
    }

    private void SpawnPrefab()
    {
        Instantiate(prefab, GetSpawnPosition(), Quaternion.identity, transform);
        objectsSpawnedText.text = "Objects Spawned : " + transform.childCount;
    }

    private Vector2 GetSpawnPosition()
    {
        float x = Random.Range(-spawnBounds.x / 2, spawnBounds.x / 2);
        float y = Random.Range(-spawnBounds.y / 2, spawnBounds.y / 2);

        return (Vector2)transform.position + new Vector2(x, y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnBounds);
    }
}
