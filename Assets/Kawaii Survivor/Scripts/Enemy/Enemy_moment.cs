using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_moment : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackResistance = 0.5f;

    private Player player;
    private bool shouldMove = false;
    private bool isKnockbackActive = false;
    private Vector2 knockbackVelocity;
    private float knockbackDuration;
    private float knockbackTimer;
    private Vector2 originalPosition; // 用于存储击退前的目标位置

    void Update()
    {
        if (isKnockbackActive)
        {
            HandleKnockback();
        }
        else if (shouldMove && player != null)
        {
            MoveTowardsPlayer();
        }
    }

    private void HandleKnockback()
    {
        knockbackTimer += Time.deltaTime;

        if (knockbackTimer >= knockbackDuration)
        {
            // 击退结束，恢复移动
            isKnockbackActive = false;
            shouldMove = true; // 确保移动被重新启用
        }
        else
        {
            // 应用击退速度（随时间衰减）
            float decay = 1f - (knockbackTimer / knockbackDuration);
            transform.position += (Vector3)knockbackVelocity * decay * Time.deltaTime;
        }
    }

    public void StorePlayer(Player player)
    {
        this.player = player;
    }

    public void FollowPlayer()
    {
        shouldMove = true;
        isKnockbackActive = false; // 确保击退状态被重置
    }

    public void StopMovement()
    {
        shouldMove = false;
    }

    // 添加击退方法
    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        // 考虑击退抗性
        float effectiveForce = force * (1f - knockbackResistance);

        knockbackVelocity = direction.normalized * effectiveForce;
        knockbackDuration = duration;
        knockbackTimer = 0f;
        isKnockbackActive = true;

        // 临时停止移动，但会在击退结束后自动恢复
        shouldMove = false;

        // 存储当前位置，以便击退后恢复移动
        if (player != null)
        {
            originalPosition = player.transform.position;
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        // 计算到玩家的距离
        float distance = Vector2.Distance(transform.position, player.transform.position);

        // 只有当距离大于停止距离时才移动
        if (distance > stoppingDistance)
        {
            // 使用Vector2.MoveTowards平滑移动
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            // 在停止距离内时完全停止
            // 这对于近战敌人很重要，防止它们"抖动"
        }
    }

    // 调试方法
    public bool IsMoving()
    {
        return shouldMove && !isKnockbackActive;
    }

    public bool IsKnockbackActive()
    {
        return isKnockbackActive;
    }
}