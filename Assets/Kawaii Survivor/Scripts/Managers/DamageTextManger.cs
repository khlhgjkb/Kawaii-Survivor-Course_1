using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DamageTextManger : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private DamageText damageTextPrefab;
    [Header("Pooling")]
    private ObjectPool<DamageText> damageTextPool;

    // 添加单例实例
    public static DamageTextManger Instance { get; private set; }

    private void Awake()
    {
        // 实现单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景保留
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 订阅事件
        MeleeEnemy.onDamageTaken += EnemyHitFallback;
    }

    // 修正方法名: OnDestory -> OnDestroy
    private void OnDestroy()
    {
        // 取消事件订阅
        MeleeEnemy.onDamageTaken -= EnemyHitFallback;
    }

    void Start()
    {
        damageTextPool = new ObjectPool<DamageText>(CreateFunction, ActionOnGet, ActionOnRelease, ActionOnDestroy);
    }

    private DamageText CreateFunction()
    {
        return Instantiate(damageTextPrefab, transform);
    }

    private void ActionOnGet(DamageText damageText)
    {
        damageText.gameObject.SetActive(true);
    }

    private void ActionOnRelease(DamageText damageText)
    {
        damageText.gameObject.SetActive(false);
    }

    private void ActionOnDestroy(DamageText damageText)
    {
        Destroy(damageText.gameObject);
    }

    private void EnemyHitFallback(int damage, Vector2 enemyPos)
    {
        // 添加空引用检查
        if (this == null) return;

        Vector3 spawnPosition = enemyPos + Vector2.up * 2;
        DamageText damageTextInstance = damageTextPool.Get();
        damageTextInstance.transform.position = spawnPosition;
        damageTextInstance.Animate(damage);
        LeanTween.delayedCall(1, () => {
            // 再次检查是否已被销毁
            if (this != null && damageTextInstance != null)
            {
                damageTextPool.Release(damageTextInstance);
            }
        });
    }
}
