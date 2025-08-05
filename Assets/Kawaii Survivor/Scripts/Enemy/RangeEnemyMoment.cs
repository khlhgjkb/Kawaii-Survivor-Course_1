using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyMoment : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f; // ����ֹͣ����

    private Player player;
    private bool shouldFollowPlayer = false; // �����ƶ�״̬��־

    void Update()
    {
        // ֻ����Ӧ�ø������ʱ���ƶ�
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
        // �����ƶ�״̬
        shouldFollowPlayer = true;
    }

    public void StopMovement()
    {
        // ����ֹͣ�ƶ�״̬
        shouldFollowPlayer = false;
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.transform.position - transform.position).normalized;

        // ���㵽��ҵľ���
        float distance = Vector2.Distance(transform.position, player.transform.position);

        // ֻ�е��������ֹͣ����ʱ���ƶ�
        if (distance > stoppingDistance)
        {
            // ʹ��Vector2.MoveTowardsƽ���ƶ�
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position,
                moveSpeed * Time.deltaTime
            );
        }
    }
}