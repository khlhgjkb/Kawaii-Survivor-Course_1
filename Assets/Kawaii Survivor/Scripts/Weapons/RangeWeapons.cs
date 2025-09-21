using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RangedWeapon : BaseWeapon
{
    [Header("Ranged Specific Settings")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private int projectilesPerShot = 1;
    [SerializeField] private float spreadAngle = 5f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip fireSound;

    private bool hasFiredThisAttack = false;

    protected override void Start()
    {
        base.Start();

        // 确保有发射点
        if (firePoint == null)
        {
            // 尝试查找名为"FirePoint"的子对象
            firePoint = transform.Find("FirePoint");
            if (firePoint == null)
            {
                Debug.LogWarning("No fire point found for ranged weapon! Using weapon position.");
                firePoint = transform;
            }
        }

        // 确保有子弹预制体
        if (projectilePrefab == null)
        {
            Debug.LogError("No projectile prefab assigned to ranged weapon!");
        }
    }

    protected override void StartAttack()
    {
        base.StartAttack();
        hasFiredThisAttack = false; // 重置发射标志
        Debug.Log("远程武器开始攻击");
    }

    // 公开的Fire方法，可以从动画事件中调用
    public void Fire()
    {
        // 防止重复发射
        if (hasFiredThisAttack)
        {
            Debug.Log("已经发射过子弹，忽略重复发射");
            return;
        }

        hasFiredThisAttack = true;
        Debug.Log("发射子弹");

        // 播放枪口闪光效果
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // 播放射击音效
        if (fireSound != null)
        {
            AudioSource.PlayClipAtPoint(fireSound, transform.position);
        }

        // 发射子弹
        for (int i = 0; i < projectilesPerShot; i++)
        {
            Vector2 fireDirection = CalculateFireDirection(i);
            Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            projectile.Initialize(fireDirection, projectileSpeed, damage, enemyMask);
        }

        // 如果没有使用动画事件，设置一个延迟后停止攻击
        if (!IsUsingAnimationEvents())
        {
            StartCoroutine(ReturnToIdleAfterDelay(0.5f));
        }
    }

    private bool IsUsingAnimationEvents()
    {
        // 检查是否有动画器，如果有则假定使用动画事件
        Animator animator = GetComponent<Animator>();
        return animator != null && animator.enabled;
    }

    private IEnumerator ReturnToIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopAttack();
    }

    private Vector2 CalculateFireDirection(int bulletIndex = 0)
    {
        Vector2 baseDirection;

        // 根据攻击模式选择瞄准方向
        if (currentAttackMode == AttackMode.Manual)
        {
            // 手动模式：瞄准鼠标位置
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // 确保Z坐标为0
            baseDirection = (mousePosition - firePoint.position).normalized;
        }
        else
        {
            // 自动模式：瞄准最近的敌人
            AllEnemy closestEnemy = GetClosetEnemy();
            if (closestEnemy != null)
            {
                baseDirection = (closestEnemy.transform.position - firePoint.position).normalized;
            }
            else
            {
                // 没有敌人时使用武器朝向
                baseDirection = transform.up;
            }
        }

        Debug.Log($"发射方向: {baseDirection}");

        // 添加散布
        if (spreadAngle > 0)
        {
            // 计算散布角度
            float angle;

            if (projectilesPerShot > 1)
            {
                // 多个子弹均匀分布
                float minAngle = -spreadAngle / 2f;
                float maxAngle = spreadAngle / 2f;
                angle = Mathf.Lerp(minAngle, maxAngle, (float)bulletIndex / Mathf.Max(1, projectilesPerShot - 1));
            }
            else
            {
                // 单发子弹随机散布
                angle = Random.Range(-spreadAngle, spreadAngle);
            }

            // 应用角度旋转
            Quaternion spreadRotation = Quaternion.Euler(0, 0, angle);
            baseDirection = spreadRotation * baseDirection;
        }

        return baseDirection;
    }

    // 重写攻击方法
    protected override void Attack()
    {
        // 对于远程武器，攻击逻辑在Fire方法中实现
        Fire();
    }

    // 在Inspector中添加调试按钮
    [Button("测试射击")]
    private void TestFire()
    {
        if (Application.isPlaying)
        {
            Debug.Log("测试射击开始");

            // 播放枪口闪光效果
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            // 播放射击音效
            if (fireSound != null)
            {
                AudioSource.PlayClipAtPoint(fireSound, transform.position);
            }

            // 发射子弹
            for (int i = 0; i < projectilesPerShot; i++)
            {
                Vector2 fireDirection = CalculateFireDirection(i);
                Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                projectile.Initialize(fireDirection, projectileSpeed, damage, enemyMask);
            }
        }
        else
        {
            Debug.Log("请在游戏运行时测试射击");
        }
    }
}