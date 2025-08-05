using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RangeEnemyAttack : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private EnemyBullet bulletPrefab;
    private Player player;

    [Header("Attack Settings")]
    [SerializeField] private int bulletDamage = 10;
    [SerializeField] public float attackFrequency = 1f;
    [SerializeField] private float bulletSpeed = 10f;

    private float attackDelay;
    private float attackTimer;
    private Vector2 gizmosDirection;

    [Header("Bullet Pooling")]
    private ObjectPool<EnemyBullet> bulletPool;

    void Start()
    {
        // ���㹥���ӳ�
        attackFrequency = Mathf.Max(0.1f, attackFrequency);
        attackDelay = 1f / attackFrequency;
        attackTimer = attackDelay;

        // ȷ���������Ч
        if (shootingPoint == null)
        {
            Debug.LogWarning("Shooting point not assigned. Using enemy position.");
            shootingPoint = transform;
        }

        bulletPool = new ObjectPool<EnemyBullet>(CreateFunction, ActionOnGet, ActionOnRelease, ActionOnDestroy);
    }

    private EnemyBullet CreateFunction()
    {
        EnemyBullet bulletinstance = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
        bulletinstance.Configure(this);
        return bulletinstance;
    }

    private void ActionOnGet(EnemyBullet bullet)
    {
        bullet.Reload();
        bullet.transform.position = shootingPoint.position;
        bullet.gameObject.SetActive(true);
    }

    private void ActionOnRelease(EnemyBullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void ActionOnDestroy(EnemyBullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    public void StorePlayer(Player player)
    {
        this.player = player;
    }

    public void AutoAim()
    {
        if (player == null) return;

        // �����������
        gizmosDirection = (player.GetCenter() - (Vector2)shootingPoint.position).normalized;
    }

    public void ReleaseBullet(EnemyBullet bullet)
    {
        bulletPool.Release(bullet);
    }

    // ����ԭ�з���������
    public void MangerShooting()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackDelay)
        {
            attackTimer = 0;
            EnemyShoot();
        }
    }

    public void EnemyShoot()
    {
        if (player == null || bulletPrefab == null) return;

        EnemyBullet bulletInstance = bulletPool.Get();
        bulletInstance.Shoot(bulletDamage, gizmosDirection);
    }

    private void OnDrawGizmos()
    {
        if (shootingPoint == null) return;

        Gizmos.color = Color.red;

        // �������������
        Vector3 endPoint = shootingPoint.position + (Vector3)gizmosDirection * 3f;
        Gizmos.DrawLine(shootingPoint.position, endPoint);

        // ���Ƽ�ͷͷ��
        Vector3 arrowLeft = endPoint + Quaternion.Euler(0, 0, 135) * -gizmosDirection * 0.5f;
        Vector3 arrowRight = endPoint + Quaternion.Euler(0, 0, -135) * -gizmosDirection * 0.5f;

        Gizmos.DrawLine(endPoint, arrowLeft);
        Gizmos.DrawLine(endPoint, arrowRight);
    }
}