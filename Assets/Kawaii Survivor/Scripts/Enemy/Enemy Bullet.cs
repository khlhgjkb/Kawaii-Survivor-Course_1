using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D),typeof(Collider2D))]
public class EnemyBullet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed; // 子弹速度改由子弹自行控制
    private int bulletDamage;
    [Header("Elements")]
    private Rigidbody2D rig;
    private  Collider2D collider;
    private RangeEnemyAttack rangeEnemyAttack;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        LeanTween.delayedCall(gameObject,5, () => rangeEnemyAttack.ReleaseBullet(this));
    }
    public void Configure(RangeEnemyAttack rangeEnemyAttack)
    {
        this.rangeEnemyAttack = rangeEnemyAttack;
    }
    public void Shoot(int damage, Vector2 direction)
    {
        bulletDamage = damage;
        GetComponent<Rigidbody2D>().velocity = direction * moveSpeed;
        transform.right = direction; // 统一朝向处理 
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Player player))
        {
            LeanTween.cancel(bulletDamage);
            player.TakeDamage(bulletDamage);
            this.collider.enabled = false;

            rangeEnemyAttack.ReleaseBullet(this);
        }
    }
    public void Reload()
    {
        rig.velocity = Vector2.zero;
        collider.enabled = true;
    }
}
