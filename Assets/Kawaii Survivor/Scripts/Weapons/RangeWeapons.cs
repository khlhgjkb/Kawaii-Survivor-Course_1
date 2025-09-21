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

        // ȷ���з����
        if (firePoint == null)
        {
            // ���Բ�����Ϊ"FirePoint"���Ӷ���
            firePoint = transform.Find("FirePoint");
            if (firePoint == null)
            {
                Debug.LogWarning("No fire point found for ranged weapon! Using weapon position.");
                firePoint = transform;
            }
        }

        // ȷ�����ӵ�Ԥ����
        if (projectilePrefab == null)
        {
            Debug.LogError("No projectile prefab assigned to ranged weapon!");
        }
    }

    protected override void StartAttack()
    {
        base.StartAttack();
        hasFiredThisAttack = false; // ���÷����־
        Debug.Log("Զ��������ʼ����");
    }

    // ������Fire���������ԴӶ����¼��е���
    public void Fire()
    {
        // ��ֹ�ظ�����
        if (hasFiredThisAttack)
        {
            Debug.Log("�Ѿ�������ӵ��������ظ�����");
            return;
        }

        hasFiredThisAttack = true;
        Debug.Log("�����ӵ�");

        // ����ǹ������Ч��
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // ���������Ч
        if (fireSound != null)
        {
            AudioSource.PlayClipAtPoint(fireSound, transform.position);
        }

        // �����ӵ�
        for (int i = 0; i < projectilesPerShot; i++)
        {
            Vector2 fireDirection = CalculateFireDirection(i);
            Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            projectile.Initialize(fireDirection, projectileSpeed, damage, enemyMask);
        }

        // ���û��ʹ�ö����¼�������һ���ӳٺ�ֹͣ����
        if (!IsUsingAnimationEvents())
        {
            StartCoroutine(ReturnToIdleAfterDelay(0.5f));
        }
    }

    private bool IsUsingAnimationEvents()
    {
        // ����Ƿ��ж��������������ٶ�ʹ�ö����¼�
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

        // ���ݹ���ģʽѡ����׼����
        if (currentAttackMode == AttackMode.Manual)
        {
            // �ֶ�ģʽ����׼���λ��
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // ȷ��Z����Ϊ0
            baseDirection = (mousePosition - firePoint.position).normalized;
        }
        else
        {
            // �Զ�ģʽ����׼����ĵ���
            AllEnemy closestEnemy = GetClosetEnemy();
            if (closestEnemy != null)
            {
                baseDirection = (closestEnemy.transform.position - firePoint.position).normalized;
            }
            else
            {
                // û�е���ʱʹ����������
                baseDirection = transform.up;
            }
        }

        Debug.Log($"���䷽��: {baseDirection}");

        // ���ɢ��
        if (spreadAngle > 0)
        {
            // ����ɢ���Ƕ�
            float angle;

            if (projectilesPerShot > 1)
            {
                // ����ӵ����ȷֲ�
                float minAngle = -spreadAngle / 2f;
                float maxAngle = spreadAngle / 2f;
                angle = Mathf.Lerp(minAngle, maxAngle, (float)bulletIndex / Mathf.Max(1, projectilesPerShot - 1));
            }
            else
            {
                // �����ӵ����ɢ��
                angle = Random.Range(-spreadAngle, spreadAngle);
            }

            // Ӧ�ýǶ���ת
            Quaternion spreadRotation = Quaternion.Euler(0, 0, angle);
            baseDirection = spreadRotation * baseDirection;
        }

        return baseDirection;
    }

    // ��д��������
    protected override void Attack()
    {
        // ����Զ�������������߼���Fire������ʵ��
        Fire();
    }

    // ��Inspector����ӵ��԰�ť
    [Button("�������")]
    private void TestFire()
    {
        if (Application.isPlaying)
        {
            Debug.Log("���������ʼ");

            // ����ǹ������Ч��
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            // ���������Ч
            if (fireSound != null)
            {
                AudioSource.PlayClipAtPoint(fireSound, transform.position);
            }

            // �����ӵ�
            for (int i = 0; i < projectilesPerShot; i++)
            {
                Vector2 fireDirection = CalculateFireDirection(i);
                Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                projectile.Initialize(fireDirection, projectileSpeed, damage, enemyMask);
            }
        }
        else
        {
            Debug.Log("������Ϸ����ʱ�������");
        }
    }
}