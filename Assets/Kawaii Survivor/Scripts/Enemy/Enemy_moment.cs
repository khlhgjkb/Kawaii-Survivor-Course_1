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
        else
        {
            // ��ֹͣ������ʱ��ȫֹͣ
            // ����ڽ�ս���˺���Ҫ����ֹ����"����"
        }
    }

    // ���Է���
    public bool IsMoving()
    {
        return shouldMove;
    }
}