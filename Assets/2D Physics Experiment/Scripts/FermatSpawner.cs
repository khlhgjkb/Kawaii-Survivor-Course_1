using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FermatSpawner : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private TextMeshPro objectsSpawnedText;

    [Header(" Settings ")]
    [SerializeField] private bool enableSpawning;
    [SerializeField] private int spawnsPerFrame;
    [SerializeField] private int maxCount;
    [SerializeField] private Vector2 spawnBounds;

    [Header(" Fermat Settings ")]
    [SerializeField] private float baseRadius;

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
        Instantiate(prefab, GetFermatSpawnPosition(transform.childCount), Quaternion.identity, transform);
        objectsSpawnedText.text = "Objects Spawned : " + transform.childCount;
    }

    private Vector2 GetFermatSpawnPosition(int index)
    {
        float r = baseRadius * Mathf.Sqrt(index);
        float angle = Mathf.PI * 2 * ((3 - Mathf.Sqrt(5)) / 2) * index;

        float x = r * Mathf.Cos(angle);
        float y = r * Mathf.Sin(angle);

        return (Vector2)transform.position + new Vector2(x, y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnBounds);
    }
}
