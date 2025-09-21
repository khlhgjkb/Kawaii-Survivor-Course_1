using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private int damage = 10;
    [SerializeField] private bool pierceThroughEnemies = false;
    [SerializeField] private int maxPierceCount = 0;

    private Vector2 direction;
    private float speed;
    private LayerMask targetMask;
    private Rigidbody2D rb;
    private int piercedEnemies = 0;

    public void Initialize(Vector2 fireDirection, float projectileSpeed, int projectileDamage, LayerMask targetLayer)
    {
        direction = fireDirection;
        speed = projectileSpeed;
        damage = projectileDamage;
        targetMask = targetLayer;

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            // 如果没有刚体，使用协程移动
            StartCoroutine(MoveWithoutRigidbody());
        }

        // 设置生命周期
        Destroy(gameObject, lifetime);

        // 旋转子弹以匹配发射方向
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private IEnumerator MoveWithoutRigidbody()
    {
        while (true)
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否命中目标
        if (((1 << other.gameObject.layer) & targetMask) != 0)
        {
            AllEnemy enemy = other.GetComponent<AllEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // 播放命中效果
                if (impactEffect != null)
                {
                    ParticleSystem effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }

                // 处理穿透逻辑
                if (pierceThroughEnemies && piercedEnemies < maxPierceCount)
                {
                    piercedEnemies++;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        // 检查是否命中墙壁或其他障碍物
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            if (impactEffect != null)
            {
                ParticleSystem effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
                Destroy(effect.gameObject, effect.main.duration);
            }

            Destroy(gameObject);
        }
    }
}