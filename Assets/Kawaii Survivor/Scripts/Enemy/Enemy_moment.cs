using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy_moment : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f;

    private Player player;
    private bool shouldMove = false;

    void Update()
    {
        if (shouldMove && player != null)
        {
            MoveTowardsPlayer();
        }
    }

    public void StorePlayer(Player player)
    {
        this.player = player;
    }

    public void FollowPlayer()
    {
        shouldMove = true;
    }

    public void StopMovement()
    {
        shouldMove = false;
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
        return shouldMove;
    }
}