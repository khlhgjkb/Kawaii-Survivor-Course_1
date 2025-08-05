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

    // ��ӹ���ģʽö��
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
        AutoAim(); // ����ģʽ����Ҫ�Զ���׼

        // ���ݵ�ǰ����ģʽִ�в�ͬ�߼�
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
        // �Զ�����״̬����
        switch (state)
        {
            case State.Idle:
                // ��AutoAim�д����Զ�����
                break;

            case State.Attack:
                Attacking();
                break;
        }
    }

    private void ManualAttackLogic()
    {
        // �ֶ�����������
        if (Input.GetMouseButtonDown(0)) // ������
        {
            if (Time.time - lastManualAttackTime >= manualAttackCooldown)
            {
                StartAttack();
                lastManualAttackTime = Time.time;
            }
        }

        // ����״̬����
        if (state == State.Attack)
        {
            Attacking();
        }
    }

    // ��Inspector���л�����ģʽ�İ�ť
    [Button("�л�����ģʽ")]
    private void ToggleAttackMode()
    {
        currentAttackMode = (currentAttackMode == AttackMode.Auto) ?
                            AttackMode.Manual : AttackMode.Auto;

        Debug.Log($"����ģʽ���л�Ϊ: {currentAttackMode}");
    }

    private void AutoAim()
    {
        AllEnemy closestEnemy = GetClosetEnemy();
        Vector2 targetUpVector = Vector3.up;

        if (closestEnemy != null)
        {
            // ֻ���Զ�ģʽ�¹����Զ�����
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