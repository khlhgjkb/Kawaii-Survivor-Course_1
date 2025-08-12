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

    // ��ӵ���ʵ��
    public static DamageTextManger Instance { get; private set; }

    private void Awake()
    {
        // ʵ�ֵ���ģʽ
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �糡������
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // �����¼�
        MeleeEnemy.onDamageTaken += EnemyHitFallback;
    }

    // ����������: OnDestory -> OnDestroy
    private void OnDestroy()
    {
        // ȡ���¼�����
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
        // ��ӿ����ü��
        if (this == null) return;

        Vector3 spawnPosition = enemyPos + Vector2.up * 2;
        DamageText damageTextInstance = damageTextPool.Get();
        damageTextInstance.transform.position = spawnPosition;
        damageTextInstance.Animate(damage);
        LeanTween.delayedCall(1, () => {
            // �ٴμ���Ƿ��ѱ�����
            if (this != null && damageTextInstance != null)
            {
                damageTextPool.Release(damageTextInstance);
            }
        });
    }
}
