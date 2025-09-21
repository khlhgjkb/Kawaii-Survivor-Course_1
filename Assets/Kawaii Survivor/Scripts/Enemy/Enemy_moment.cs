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
    private Vector2 originalPosition; // ���ڴ洢����ǰ��Ŀ��λ��

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
            // ���˽������ָ��ƶ�
            isKnockbackActive = false;
            shouldMove = true; // ȷ���ƶ�����������
        }
        else
        {
            // Ӧ�û����ٶȣ���ʱ��˥����
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
        isKnockbackActive = false; // ȷ������״̬������
    }

    public void StopMovement()
    {
        shouldMove = false;
    }

    // ��ӻ��˷���
    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        // ���ǻ��˿���
        float effectiveForce = force * (1f - knockbackResistance);

        knockbackVelocity = direction.normalized * effectiveForce;
        knockbackDuration = duration;
        knockbackTimer = 0f;
        isKnockbackActive = true;

        // ��ʱֹͣ�ƶ��������ڻ��˽������Զ��ָ�
        shouldMove = false;

        // �洢��ǰλ�ã��Ա���˺�ָ��ƶ�
        if (player != null)
        {
            originalPosition = player.transform.position;
        }
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
        return shouldMove && !isKnockbackActive;
    }

    public bool IsKnockbackActive()
    {
        return isKnockbackActive;
    }
}