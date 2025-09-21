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
            // ���û�и��壬ʹ��Э���ƶ�
            StartCoroutine(MoveWithoutRigidbody());
        }

        // ������������
        Destroy(gameObject, lifetime);

        // ��ת�ӵ���ƥ�䷢�䷽��
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
        // ����Ƿ�����Ŀ��
        if (((1 << other.gameObject.layer) & targetMask) != 0)
        {
            AllEnemy enemy = other.GetComponent<AllEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // ��������Ч��
                if (impactEffect != null)
                {
                    ParticleSystem effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }

                // ����͸�߼�
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
        // ����Ƿ�����ǽ�ڻ������ϰ���
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