using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AllEnemy : MonoBehaviour
{
    [Header("Components")]
    protected Enemy_moment movement;
    protected RangeEnemyAttack attack;
    [Header("Health")]
    [SerializeField] protected int maxHealth;
    protected int health;
    [Header("Elements")]
    protected Player player;
    [Header("Spawn Sequence")]
    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] protected SpriteRenderer spawnIndicator;
    [SerializeField] protected Collider2D collider;
    protected bool hasSpawned;

    [Header("Effects")]
    [SerializeField] protected ParticleSystem passAwayParticles;
    [Header("Attack Settings")]
    [SerializeField] protected float playerDetectionRadius;
    [Header("Actions")]
    public static Action<int, Vector2> onDamageTaken;

    [Header("DEBUG")]
    [SerializeField] protected bool showGizmos = true;

    protected virtual void Start()
    {
        health = maxHealth;
        movement = GetComponent<Enemy_moment>();

        // 确保所有关键状态重置
        hasSpawned = false;
        collider.enabled = false;
        SetRenderersVisibility(false);

        // 延迟查找玩家
        StartCoroutine(FindPlayerCoroutine());
        StartSpawnSequence();
    }
    private IEnumerator FindPlayerCoroutine()
    {
        int maxAttempts = 10;
        int attempts = 0;

        while (player == null && attempts < maxAttempts)
        {
            player = FindAnyObjectByType<Player>();
            attempts++;
            yield return new WaitForSeconds(0.1f);
        }

        if (player == null)
        {
            Debug.LogError("No player found! Disabling enemy.");
            Destroy(gameObject);
            yield break;
        }

        movement.StorePlayer(player);
    }

    private void StartSpawnSequence()
    {
        SetRenderersVisibility(false);
        Vector3 targetScale = spawnIndicator.transform.localScale * 1.2f;
        LeanTween.scale(spawnIndicator.gameObject, targetScale, .3f)
        .setLoopPingPong(4)
        .setOnComplete(SpawnSequenceCompleted);
    }

    protected virtual void SpawnSequenceCompleted()
    {
        SetRenderersVisibility(true);
        hasSpawned = true;
        collider.enabled = true;
        if (movement != null) movement.FollowPlayer();
    }
    private void SetRenderersVisibility(bool visibility = true)
    {
        renderer.enabled = visibility;
        spawnIndicator.enabled = !visibility;
    }
    public void TakeDamage(int damage)
    {
        if (!hasSpawned) return;

        int realDamage = Mathf.Min(damage, health);
        health -= realDamage;

        onDamageTaken?.Invoke(realDamage, transform.position);
        if (health <= 0)
            PassAway();
    }
    private void PassAway()
    {
        if (passAwayParticles != null)
        {
            passAwayParticles.transform.SetParent(null);
            passAwayParticles.Play();
            Destroy(passAwayParticles.gameObject, passAwayParticles.main.duration);
        }

        Destroy(gameObject);
    }
}

