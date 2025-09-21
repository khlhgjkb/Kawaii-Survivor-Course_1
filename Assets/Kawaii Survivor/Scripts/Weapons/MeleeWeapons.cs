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
    private float attackAngle = 90f; // 攻击角度

    protected override void Start()
    {
        base.Start();

        // 确保有碰撞体
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
        hasHitThisAttack = false; // 重置命中标志
        Debug.Log("近战武器开始攻击");
    }

    // 重写攻击方法，实现近战攻击逻辑
    protected override void Attack()
    {
        // 防止重复攻击
        if (hasHitThisAttack)
        {
            Debug.Log("已经攻击过，忽略重复攻击");
            return;
        }

        hasHitThisAttack = true;
        Debug.Log("执行近战攻击");

        // 执行近战攻击检测
        PerformMeleeAttack();

        // 播放命中效果
        if (hitEffect != null)
        {
            hitEffect.Play();
        }

        // 播放命中音效
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        // 近战攻击立即完成，不需要等待动画
        StartCoroutine(CompleteAttackAfterDelay(0.1f));
    }

    private IEnumerator CompleteAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopAttack();
    }

    private void PerformMeleeAttack()
    {
        // 使用扇形检测，更符合近战武器的特性
        Collider2D[] allEnemies = Physics2D.OverlapCircleAll(
            hitDetectionTransform.position,
            attackRadius,
            enemyMask
        );

        Debug.Log($"检测到 {allEnemies.Length} 个潜在敌人");

        // 过滤出在攻击范围内的敌人
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

        Debug.Log($"有 {enemiesInRange.Count} 个敌人在攻击范围内");

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

                Debug.Log($"对敌人 {enemy.name} 造成 {damage} 点伤害");

                // 添加击退效果
                ApplyKnockback(enemy);
            }
        }

        if (hits == 0)
        {
            Debug.Log("攻击未命中任何敌人");
        }
    }

    private void ApplyKnockback(AllEnemy enemy)
    {
        // 计算击退方向（从武器指向敌人）
        Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;

        // 获取敌人的移动组件并应用击退
        Enemy_moment movement = enemy.GetComponent<Enemy_moment>();
        if (movement != null)
        {
            movement.ApplyKnockback(knockbackDirection, knockbackForce, knockbackDuration);
        }
        else
        {
            // 备用方案：如果没有 Enemy_moment 组件，使用 Rigidbody2D
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    // 在Inspector中添加调试按钮
    [Button("测试攻击")]
    private void TestAttack()
    {
        if (Application.isPlaying)
        {
            Debug.Log("测试攻击开始");
            PerformMeleeAttack();
        }
        else
        {
            Debug.Log("请在游戏运行时测试攻击");
        }
    }

    // 可视化调试
    private void OnDrawGizmosSelected()
    {
        // 绘制攻击范围
        Gizmos.color = Color.red;
        if (hitDetectionTransform != null)
        {
            Gizmos.DrawWireSphere(hitDetectionTransform.position, attackRadius);

            // 绘制攻击扇形
            Vector3 forward = hitDetectionTransform.up * attackRadius;
            Vector3 left = Quaternion.Euler(0, 0, attackAngle / 2) * forward;
            Vector3 right = Quaternion.Euler(0, 0, -attackAngle / 2) * forward;

            Gizmos.DrawLine(hitDetectionTransform.position, hitDetectionTransform.position + left);
            Gizmos.DrawLine(hitDetectionTransform.position, hitDetectionTransform.position + right);

            // 绘制扇形弧线
            Vector3 prevPoint = hitDetectionTransform.position + left;
            for (int i = 1; i <= 10; i++)
            {
                float angle = attackAngle / 2 - (attackAngle / 10) * i;
                Vector3 nextPoint = hitDetectionTransform.position + Quaternion.Euler(0, 0, angle) * forward;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }

        // 绘制基类的范围
        base.OnDrawGizmosSelected();
    }
}