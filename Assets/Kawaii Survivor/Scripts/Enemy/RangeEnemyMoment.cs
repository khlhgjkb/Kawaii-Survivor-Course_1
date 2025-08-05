using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyMoment : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f; // 新增停止距离

    private Player player;
    private bool shouldFollowPlayer = false; // 新增移动状态标志

    void Update()
    {
        // 只有在应该跟随玩家时才移动
        if (shouldFollowPlayer && player != null)
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
        // 设置移动状态
        shouldFollowPlayer = true;
    }

    public void StopMovement()
    {
        // 设置停止移动状态
        shouldFollowPlayer = false;
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.transform.position - transform.position).normalized;

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
    }
}