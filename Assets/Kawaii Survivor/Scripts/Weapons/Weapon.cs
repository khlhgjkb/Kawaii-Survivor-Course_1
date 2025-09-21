using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public abstract class BaseWeapon : MonoBehaviour
{
    protected enum State
    {
        Idle,
        Attack
    }
    protected State state;

    // 添加攻击模式枚举
    public enum AttackMode
    {
        Auto,
        Manual
    }

    [Header("Attack Mode")]
    [SerializeField] protected AttackMode currentAttackMode = AttackMode.Auto;
    [SerializeField] protected float manualAttackCooldown = 0.5f;
    protected float lastManualAttackTime;
    [Header("Melee Settings")]
    [SerializeField] protected float aimLerp = 5f; // 将 aimLerp 移到更合适的位置
    [Header("Elements")]
    [SerializeField] protected Transform hitDetectionTransform;
    [SerializeField] protected float hitDetectionRadius;

    [Header(" Settings ")]
    [SerializeField] protected float range;
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected float attackDelay;

    [Header("Attack")]
    [SerializeField] protected int damage;

    // 添加攻击冷却标志
    protected bool isAttackOnCooldown = false;
    protected float attackTimer;
    protected List<AllEnemy> damagedEnemies = new List<AllEnemy>();

    // 事件 - 用于通知外部系统状态变化
    public System.Action OnAttackStarted;
    public System.Action OnAttackFinished;

    protected virtual void Start()
    {
        state = State.Idle;

        if (hitDetectionTransform == null)
        {
            hitDetectionTransform = transform;
            Debug.LogWarning("Hit detection transform not set, using weapon transform");
        }
    }

    protected virtual void Update()
    {
        HandleAttackLogic();
        AutoAim();

        // 添加按键切换攻击模式
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleAttackMode();
        }
    }

    protected virtual void HandleAttackLogic()
    {
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

    protected virtual void AutoAttackLogic()
    {
        // 自动攻击状态处理
        switch (state)
        {
            case State.Idle:
                ManageAttack(); // 在空闲状态下管理自动攻击
                break;

            case State.Attack:
                Attacking();
                break;
        }
    }

    protected virtual void ManualAttackLogic()
    {
        // 手动攻击输入检测
        if (Input.GetMouseButtonDown(0)) // 鼠标左键
        {
            Debug.Log($"鼠标左键按下 - 状态: {state}, 冷却时间: {Time.time - lastManualAttackTime}/{manualAttackCooldown}, 冷却标志: {isAttackOnCooldown}");

            if (Time.time - lastManualAttackTime >= manualAttackCooldown && state != State.Attack && !isAttackOnCooldown)
            {
                Debug.Log("满足攻击条件，开始攻击");
                StartAttack();
                lastManualAttackTime = Time.time;

                // 设置攻击冷却
                StartCoroutine(AttackCooldown());
            }
            else
            {
                Debug.Log($"不满足攻击条件 - 状态: {state}, 冷却: {isAttackOnCooldown}, 时间: {Time.time - lastManualAttackTime}/{manualAttackCooldown}");
            }
        }

        // 攻击状态处理
        if (state == State.Attack)
        {
            Debug.Log("攻击状态中");
            Attacking();
        }
    }
    protected virtual void OnGUI()
    {
        if (Camera.main != null)
        {
            // 显示武器状态信息
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 200, 100),
                     $"State: {state}\n" +
                     $"Mode: {currentAttackMode}\n" +
                     $"Cooldown: {isAttackOnCooldown}\n" +
                     $"Timer: {attackTimer:F2}/{attackDelay:F2}");
        }
    }

    // 攻击冷却协程
    private IEnumerator AttackCooldown()
    {
        isAttackOnCooldown = true;
        yield return new WaitForSeconds(manualAttackCooldown);
        isAttackOnCooldown = false;
    }

    protected virtual void ManageAttack()
    {
        IncrementAttackTimer();

        if (attackTimer >= attackDelay && state != State.Attack && !isAttackOnCooldown)
        {
            Debug.Log("满足自动攻击条件，开始攻击");
            attackTimer = 0;
            StartAttack();

            // 设置攻击冷却
            StartCoroutine(AttackCooldown());
        }
    }

    protected void IncrementAttackTimer()
    {
        attackTimer += Time.deltaTime;
    }

    protected virtual void StartAttack()
    {
        Debug.Log("开始攻击");

        // 确保不在攻击状态时才触发新攻击
        if (state == State.Attack)
        {
            Debug.Log("已在攻击状态，忽略新攻击请求");
            return;
        }

        state = State.Attack;
        damagedEnemies.Clear();

        // 通知外部系统攻击开始
        OnAttackStarted?.Invoke();

        // 直接调用攻击方法，让外部动画系统处理视觉表现
        Attack();

        // 设置一个超时保护，确保状态不会卡在Attack
        StartCoroutine(AttackTimeoutProtection());
    }

    private IEnumerator AttackTimeoutProtection()
    {
        // 等待最大攻击持续时间（根据攻击延迟计算）
        float timeout = attackDelay * 2f;
        yield return new WaitForSeconds(timeout);

        // 如果仍然在攻击状态，强制重置
        if (state == State.Attack)
        {
            Debug.LogWarning($"攻击状态超时，强制重置状态");
            StopAttack();
        }
    }

    protected virtual void Attacking()
    {
        // 攻击状态中的逻辑，子类可以重写
    }

    // 外部调用此方法来停止攻击（例如从动画事件中调用）
    public virtual void StopAttack()
    {
        Debug.Log("停止攻击");
        state = State.Idle;
        damagedEnemies.Clear();

        // 通知外部系统攻击结束
        OnAttackFinished?.Invoke();
    }

    // 切换攻击模式
    protected void ToggleAttackMode()
    {
        // 添加空引用检查
        if (this == null)
        {
            Debug.LogWarning("Cannot toggle attack mode - weapon object is null");
            return;
        }

        // 添加组件检查
        if (!isActiveAndEnabled)
        {
            Debug.LogWarning("Cannot toggle attack mode - weapon is not active");
            return;
        }

        currentAttackMode = (currentAttackMode == AttackMode.Auto) ?
                            AttackMode.Manual : AttackMode.Auto;

        Debug.Log($"攻击模式已切换为: {currentAttackMode}");
    }

    protected virtual void AutoAim()
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

            // 对于近战武器，可以调整瞄准逻辑
            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;

            // 应用瞄准平滑
            transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);
        }
        else
        {
            // 没有敌人时，保持当前方向或慢慢回归默认方向
            transform.up = Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime * aimLerp * 0.5f);
        }
    }
    // 抽象方法，子类必须实现
    protected abstract void Attack();

    protected AllEnemy GetClosetEnemy()
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

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitDetectionTransform.position, hitDetectionRadius);
    }
}