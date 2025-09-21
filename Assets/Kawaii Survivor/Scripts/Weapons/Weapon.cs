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

    // ��ӹ���ģʽö��
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
    [SerializeField] protected float aimLerp = 5f; // �� aimLerp �Ƶ������ʵ�λ��
    [Header("Elements")]
    [SerializeField] protected Transform hitDetectionTransform;
    [SerializeField] protected float hitDetectionRadius;

    [Header(" Settings ")]
    [SerializeField] protected float range;
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected float attackDelay;

    [Header("Attack")]
    [SerializeField] protected int damage;

    // ��ӹ�����ȴ��־
    protected bool isAttackOnCooldown = false;
    protected float attackTimer;
    protected List<AllEnemy> damagedEnemies = new List<AllEnemy>();

    // �¼� - ����֪ͨ�ⲿϵͳ״̬�仯
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

        // ��Ӱ����л�����ģʽ
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleAttackMode();
        }
    }

    protected virtual void HandleAttackLogic()
    {
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

    protected virtual void AutoAttackLogic()
    {
        // �Զ�����״̬����
        switch (state)
        {
            case State.Idle:
                ManageAttack(); // �ڿ���״̬�¹����Զ�����
                break;

            case State.Attack:
                Attacking();
                break;
        }
    }

    protected virtual void ManualAttackLogic()
    {
        // �ֶ�����������
        if (Input.GetMouseButtonDown(0)) // ������
        {
            Debug.Log($"���������� - ״̬: {state}, ��ȴʱ��: {Time.time - lastManualAttackTime}/{manualAttackCooldown}, ��ȴ��־: {isAttackOnCooldown}");

            if (Time.time - lastManualAttackTime >= manualAttackCooldown && state != State.Attack && !isAttackOnCooldown)
            {
                Debug.Log("���㹥����������ʼ����");
                StartAttack();
                lastManualAttackTime = Time.time;

                // ���ù�����ȴ
                StartCoroutine(AttackCooldown());
            }
            else
            {
                Debug.Log($"�����㹥������ - ״̬: {state}, ��ȴ: {isAttackOnCooldown}, ʱ��: {Time.time - lastManualAttackTime}/{manualAttackCooldown}");
            }
        }

        // ����״̬����
        if (state == State.Attack)
        {
            Debug.Log("����״̬��");
            Attacking();
        }
    }
    protected virtual void OnGUI()
    {
        if (Camera.main != null)
        {
            // ��ʾ����״̬��Ϣ
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 200, 100),
                     $"State: {state}\n" +
                     $"Mode: {currentAttackMode}\n" +
                     $"Cooldown: {isAttackOnCooldown}\n" +
                     $"Timer: {attackTimer:F2}/{attackDelay:F2}");
        }
    }

    // ������ȴЭ��
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
            Debug.Log("�����Զ�������������ʼ����");
            attackTimer = 0;
            StartAttack();

            // ���ù�����ȴ
            StartCoroutine(AttackCooldown());
        }
    }

    protected void IncrementAttackTimer()
    {
        attackTimer += Time.deltaTime;
    }

    protected virtual void StartAttack()
    {
        Debug.Log("��ʼ����");

        // ȷ�����ڹ���״̬ʱ�Ŵ����¹���
        if (state == State.Attack)
        {
            Debug.Log("���ڹ���״̬�������¹�������");
            return;
        }

        state = State.Attack;
        damagedEnemies.Clear();

        // ֪ͨ�ⲿϵͳ������ʼ
        OnAttackStarted?.Invoke();

        // ֱ�ӵ��ù������������ⲿ����ϵͳ�����Ӿ�����
        Attack();

        // ����һ����ʱ������ȷ��״̬���Ῠ��Attack
        StartCoroutine(AttackTimeoutProtection());
    }

    private IEnumerator AttackTimeoutProtection()
    {
        // �ȴ���󹥻�����ʱ�䣨���ݹ����ӳټ��㣩
        float timeout = attackDelay * 2f;
        yield return new WaitForSeconds(timeout);

        // �����Ȼ�ڹ���״̬��ǿ������
        if (state == State.Attack)
        {
            Debug.LogWarning($"����״̬��ʱ��ǿ������״̬");
            StopAttack();
        }
    }

    protected virtual void Attacking()
    {
        // ����״̬�е��߼������������д
    }

    // �ⲿ���ô˷�����ֹͣ����������Ӷ����¼��е��ã�
    public virtual void StopAttack()
    {
        Debug.Log("ֹͣ����");
        state = State.Idle;
        damagedEnemies.Clear();

        // ֪ͨ�ⲿϵͳ��������
        OnAttackFinished?.Invoke();
    }

    // �л�����ģʽ
    protected void ToggleAttackMode()
    {
        // ��ӿ����ü��
        if (this == null)
        {
            Debug.LogWarning("Cannot toggle attack mode - weapon object is null");
            return;
        }

        // ���������
        if (!isActiveAndEnabled)
        {
            Debug.LogWarning("Cannot toggle attack mode - weapon is not active");
            return;
        }

        currentAttackMode = (currentAttackMode == AttackMode.Auto) ?
                            AttackMode.Manual : AttackMode.Auto;

        Debug.Log($"����ģʽ���л�Ϊ: {currentAttackMode}");
    }

    protected virtual void AutoAim()
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

            // ���ڽ�ս���������Ե�����׼�߼�
            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;

            // Ӧ����׼ƽ��
            transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);
        }
        else
        {
            // û�е���ʱ�����ֵ�ǰ����������ع�Ĭ�Ϸ���
            transform.up = Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime * aimLerp * 0.5f);
        }
    }
    // ���󷽷����������ʵ��
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