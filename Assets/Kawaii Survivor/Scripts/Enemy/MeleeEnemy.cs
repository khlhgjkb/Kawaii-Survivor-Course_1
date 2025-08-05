using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(Enemy_moment))]
public class MeleeEnemy : AllEnemy
{
    [Header("Attack Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float attackFrequency;

    private float attackDelay;
    private float attackTimer;
    private bool canAttack = false;

    protected override void Start()
    {
        base.Start();
  
        attackDelay = 1f / Mathf.Max(0.1f, attackFrequency);
        attackTimer = attackDelay;
    }

    void Update()
    {
        if (attackTimer >= attackDelay)
        {
            TryAttack();
        }
        else
        {
            Wait();
        }
    }

    private void Attack()
    {
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log($"Melee enemy attacked player! Damage: {damage}");
        }
    }

    private void Wait()
    {
        attackTimer += Time.deltaTime;
    }

    private void TryAttack()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= playerDetectionRadius)
        {
            Attack();
            attackTimer = 0; // ÖØÖÃ¹¥»÷¼ÆÊ±Æ÷
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = canAttack ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            Vector3 labelPos = transform.position + Vector3.up * 1.5f;
            UnityEditor.Handles.Label(labelPos, $"Dist: {distance:F1}");
        }
    }
}