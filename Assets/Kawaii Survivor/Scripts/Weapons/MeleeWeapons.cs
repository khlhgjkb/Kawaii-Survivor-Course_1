using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MeleeWeapon : BaseWeapon
{
    [Header("Melee Specific Settings")]
    [SerializeField] private BoxCollider2D hitCollider;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private int maxTargets = 3;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private AudioClip hitSound;

    private bool hasHitThisAttack = false;
    private float attackAngle = 90f; // �����Ƕ�

    protected override void Start()
    {
        base.Start();

        // ȷ������ײ��
        if (hitCollider == null)
        {
            hitCollider = GetComponent<BoxCollider2D>();
            if (hitCollider == null)
            {
                Debug.LogError("No hit collider found on melee weapon!");
            }
        }
    }

    protected override void StartAttack()
    {
        base.StartAttack();
        hasHitThisAttack = false; // �������б�־
        Debug.Log("��ս������ʼ����");
    }

    // ��д����������ʵ�ֽ�ս�����߼�
    protected override void Attack()
    {
        // ��ֹ�ظ�����
        if (hasHitThisAttack)
        {
            Debug.Log("�Ѿ��������������ظ�����");
            return;
        }

        hasHitThisAttack = true;
        Debug.Log("ִ�н�ս����");

        // ִ�н�ս�������
        PerformMeleeAttack();

        // ��������Ч��
        if (hitEffect != null)
        {
            hitEffect.Play();
        }

        // ����������Ч
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        // ��ս����������ɣ�����Ҫ�ȴ�����
        StartCoroutine(CompleteAttackAfterDelay(0.1f));
    }

    private IEnumerator CompleteAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopAttack();
    }

    private void PerformMeleeAttack()
    {
        // ʹ�����μ�⣬�����Ͻ�ս����������
        Collider2D[] allEnemies = Physics2D.OverlapCircleAll(
            hitDetectionTransform.position,
            attackRadius,
            enemyMask
        );

        Debug.Log($"��⵽ {allEnemies.Length} ��Ǳ�ڵ���");

        // ���˳��ڹ�����Χ�ڵĵ���
        List<Collider2D> enemiesInRange = new List<Collider2D>();
        foreach (Collider2D enemy in allEnemies)
        {
            Vector2 directionToEnemy = (enemy.transform.position - hitDetectionTransform.position).normalized;
            float angle = Vector2.Angle(transform.up, directionToEnemy);

            if (angle <= attackAngle / 2f)
            {
                enemiesInRange.Add(enemy);
            }
        }

        Debug.Log($"�� {enemiesInRange.Count} �������ڹ�����Χ��");

        int hits = 0;
        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            if (hits >= maxTargets) break;

            AllEnemy enemy = enemiesInRange[i].GetComponent<AllEnemy>();
            if (enemy == null) continue;

            if (!damagedEnemies.Contains(enemy))
            {
                enemy.TakeDamage(damage);
                damagedEnemies.Add(enemy);
                hits++;

                Debug.Log($"�Ե��� {enemy.name} ��� {damage} ���˺�");

                // ��ӻ���Ч��
                ApplyKnockback(enemy);
            }
        }

        if (hits == 0)
        {
            Debug.Log("����δ�����κε���");
        }
    }

    private void ApplyKnockback(AllEnemy enemy)
    {
        // ������˷��򣨴�����ָ����ˣ�
        Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;

        // ��ȡ���˵��ƶ������Ӧ�û���
        Enemy_moment movement = enemy.GetComponent<Enemy_moment>();
        if (movement != null)
        {
            movement.ApplyKnockback(knockbackDirection, knockbackForce, knockbackDuration);
        }
        else
        {
            // ���÷��������û�� Enemy_moment �����ʹ�� Rigidbody2D
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    // ��Inspector����ӵ��԰�ť
    [Button("���Թ���")]
    private void TestAttack()
    {
        if (Application.isPlaying)
        {
            Debug.Log("���Թ�����ʼ");
            PerformMeleeAttack();
        }
        else
        {
            Debug.Log("������Ϸ����ʱ���Թ���");
        }
    }

    // ���ӻ�����
    private void OnDrawGizmosSelected()
    {
        // ���ƹ�����Χ
        Gizmos.color = Color.red;
        if (hitDetectionTransform != null)
        {
            Gizmos.DrawWireSphere(hitDetectionTransform.position, attackRadius);

            // ���ƹ�������
            Vector3 forward = hitDetectionTransform.up * attackRadius;
            Vector3 left = Quaternion.Euler(0, 0, attackAngle / 2) * forward;
            Vector3 right = Quaternion.Euler(0, 0, -attackAngle / 2) * forward;

            Gizmos.DrawLine(hitDetectionTransform.position, hitDetectionTransform.position + left);
            Gizmos.DrawLine(hitDetectionTransform.position, hitDetectionTransform.position + right);

            // �������λ���
            Vector3 prevPoint = hitDetectionTransform.position + left;
            for (int i = 1; i <= 10; i++)
            {
                float angle = attackAngle / 2 - (attackAngle / 10) * i;
                Vector3 nextPoint = hitDetectionTransform.position + Quaternion.Euler(0, 0, angle) * forward;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }

        // ���ƻ���ķ�Χ
        base.OnDrawGizmosSelected();
    }
}