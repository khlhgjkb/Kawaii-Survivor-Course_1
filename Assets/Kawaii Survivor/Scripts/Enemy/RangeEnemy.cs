using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Enemy_moment), typeof(RangeEnemyAttack))]
public class RangeEnemy : AllEnemy
{
    private bool isAttacking = false;
    private Coroutine attackCoroutine;
    private bool isActive = false; // 新增：控制敌人是否激活

    protected override void Start()
    {
        base.Start();


        // 初始禁用敌人行为
        SetEnemyActive(false);

        attack = GetComponent<RangeEnemyAttack>();
        if (player != null)
        {
            attack.StorePlayer(player);
        }
    }

    // 重写基类的生成完成方法
    protected override void SpawnSequenceCompleted()
    {
        base.SpawnSequenceCompleted(); // 调用基类方法
        SetEnemyActive(true); // 生成完成后激活敌人
    }

    // 新增：控制敌人激活状态
    private void SetEnemyActive(bool active)
    {
        isActive = active;

        // 禁用/启用移动组件
        if (movement != null)
        {
            movement.enabled = active;
            if (!active) movement.StopMovement();
        }

        // 禁用/启用攻击组件
        if (attack != null)
        {
            attack.enabled = active;
            if (!active && attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }

        // 重置攻击状态
        isAttacking = false;
    }

    void Update()
    {
        // 只有在激活状态下才执行行为
        if (!isActive || !hasSpawned) return;

        MangeAttack();

        transform.localScale = player.transform.position.x > transform.position.x ? Vector3.one : Vector3.one.With(x:-1);
    }

    private void MangeAttack()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > playerDetectionRadius)
        {
            // 玩家在攻击范围外 - 移动
            if (movement != null)
            {
                movement.FollowPlayer();
            }

            // 停止攻击
            if (isAttacking)
            {
                isAttacking = false;
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }
            }
        }
        else
        {
            // 玩家在攻击范围内 - 停止移动并攻击
            if (movement != null)
            {
                movement.StopMovement();
            }

            // 开始攻击
            if (!isAttacking)
            {
                isAttacking = true;
                attackCoroutine = StartCoroutine(AttackRoutine());
            }

            // 更新瞄准方向
            attack.AutoAim();
        }
    }

    private IEnumerator AttackRoutine()
    {
        // 确保只在激活状态下攻击
        while (isActive && isAttacking && player != null)
        {
            attack.MangerShooting();
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || !isActive) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            Vector3 labelPos = transform.position + Vector3.up * 1.5f;
            UnityEditor.Handles.Label(labelPos, $"Dist: {distance:F1}");
        }
    }
}