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
    private bool isActive = false; // ���������Ƶ����Ƿ񼤻�

    protected override void Start()
    {
        base.Start();


        // ��ʼ���õ�����Ϊ
        SetEnemyActive(false);

        attack = GetComponent<RangeEnemyAttack>();
        if (player != null)
        {
            attack.StorePlayer(player);
        }
    }

    // ��д�����������ɷ���
    protected override void SpawnSequenceCompleted()
    {
        base.SpawnSequenceCompleted(); // ���û��෽��
        SetEnemyActive(true); // ������ɺ󼤻����
    }

    // ���������Ƶ��˼���״̬
    private void SetEnemyActive(bool active)
    {
        isActive = active;

        // ����/�����ƶ����
        if (movement != null)
        {
            movement.enabled = active;
            if (!active) movement.StopMovement();
        }

        // ����/���ù������
        if (attack != null)
        {
            attack.enabled = active;
            if (!active && attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }

        // ���ù���״̬
        isAttacking = false;
    }

    void Update()
    {
        // ֻ���ڼ���״̬�²�ִ����Ϊ
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
            // ����ڹ�����Χ�� - �ƶ�
            if (movement != null)
            {
                movement.FollowPlayer();
            }

            // ֹͣ����
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
            // ����ڹ�����Χ�� - ֹͣ�ƶ�������
            if (movement != null)
            {
                movement.StopMovement();
            }

            // ��ʼ����
            if (!isAttacking)
            {
                isAttacking = true;
                attackCoroutine = StartCoroutine(AttackRoutine());
            }

            // ������׼����
            attack.AutoAim();
        }
    }

    private IEnumerator AttackRoutine()
    {
        // ȷ��ֻ�ڼ���״̬�¹���
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