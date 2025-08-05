using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using NaughtyAttributes;

public class Weapon : MonoBehaviour
{
    enum State
    {
        Idle,
        Attack
    }
    private State state;

    // 添加攻击模式枚举
    public enum AttackMode
    {
        Auto,
        Manual
    }

    [Header("Attack Mode")]
    [SerializeField] private AttackMode currentAttackMode = AttackMode.Auto;
    [SerializeField] private BoxCollider2D hitCollider;
    [SerializeField] private float manualAttackCooldown = 0.5f;
    private float lastManualAttackTime;

    [Header("Elements")]
    [SerializeField] private Transform hitDetectionTransform;
    [SerializeField] private float hitDetectionRadius;

    [Header(" Settings ")]
    [SerializeField] private float range;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float attackDelay;

    [Header(" Animations ")]
    [SerializeField] private float aimLerp;
    private float attackTimer;
    private List<AllEnemy> damageadEnemies = new List<AllEnemy>();


    [Header("Attack")]
    [SerializeField] private int damage;
    [SerializeField] private Animator animator;

    void Start()
    {
        state = State.Idle;
    }

    void Update()
    {
        AutoAim(); // 所有模式都需要自动瞄准

        // 根据当前攻击模式执行不同逻辑
        switch (currentAttackMode)
        {
            case AttackMode.Auto:
                AutoAttackLogic();
                break;

            case AttackMode.Manual:
                ManualAttackLogic();
                break;
        }
    }

    private void AutoAttackLogic()
    {
        // 自动攻击状态处理
        switch (state)
        {
            case State.Idle:
                // 在AutoAim中处理自动攻击
                break;

            case State.Attack:
                Attacking();
                break;
        }
    }

    private void ManualAttackLogic()
    {
        // 手动攻击输入检测
        if (Input.GetMouseButtonDown(0)) // 鼠标左键
        {
            if (Time.time - lastManualAttackTime >= manualAttackCooldown)
            {
                StartAttack();
                lastManualAttackTime = Time.time;
            }
        }

        // 攻击状态处理
        if (state == State.Attack)
        {
            Attacking();
        }
    }

    // 在Inspector中切换攻击模式的按钮
    [Button("切换攻击模式")]
    private void ToggleAttackMode()
    {
        currentAttackMode = (currentAttackMode == AttackMode.Auto) ?
                            AttackMode.Manual : AttackMode.Auto;

        Debug.Log($"攻击模式已切换为: {currentAttackMode}");
    }

    private void AutoAim()
    {
        AllEnemy closestEnemy = GetClosetEnemy();
        Vector2 targetUpVector = Vector3.up;

        if (closestEnemy != null)
        {
            // 只在自动模式下管理自动攻击
            if (currentAttackMode == AttackMode.Auto)
            {
                ManageAttack();
            }

            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;
        }

        transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);
    }

    private void ManageAttack()
    {
        if (attackTimer >= attackDelay)
        {
            attackTimer = 0;
            StartAttack();
        }
    }

    private void IncreamentAttackTimer()
    {
        attackTimer += Time.deltaTime;
    }

    private void StartAttack()
    {
        animator.Play("Attack");
        state = State.Attack;
        animator.speed = 1f / attackDelay;
        damageadEnemies.Clear();
    }

    private void Attacking()
    {
        Attack();
    }

    private void StopAttack()
    {
        state = State.Idle;
        damageadEnemies.Clear();
    }

    private void Attack()
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll(hitDetectionTransform.position,hitCollider.bounds.size,hitDetectionTransform.localEulerAngles.z,enemyMask);

        for (int i = 0; i < enemies.Length; i++)
        {
            AllEnemy enemy = enemies[i].GetComponent<AllEnemy>();
            if (enemy == null) continue;

            if (!damageadEnemies.Contains(enemy))
            {
                enemy.TakeDamage(damage);
                damageadEnemies.Add(enemy);
            }
        }
    }

    private AllEnemy GetClosetEnemy()
    {
        AllEnemy closestEnemy = null;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, enemyMask);
        if (enemies.Length <= 0)
            return null;

        float minDistance = range;
        for (int i = 0; i < enemies.Length; i++)
        {
            AllEnemy enemyChecked = enemies[i].GetComponent<AllEnemy>();
            if (enemyChecked != null)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemyChecked.transform.position);
                if (distanceToEnemy < minDistance)
                {
                    closestEnemy = enemyChecked;
                    minDistance = distanceToEnemy;
                }
            }
        }
        return closestEnemy;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitDetectionTransform.position, hitDetectionRadius);
    }
}