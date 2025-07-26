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
    private void Awake()
    {
        All_Enemy.onDamageTaken += EnemyHitFallback;
    }
    private void OnDestory()
    {
        All_Enemy.onDamageTaken += EnemyHitFallback;
    }
    // Start is called before the first frame update
    void Start()
    {
        damageTextPool = new ObjectPool<DamageText>(CreateFunction,ActionOnGet,ActionOnRelease,ActionOnDestroy);
    }
    private DamageText CreateFunction()
    {
        return Instantiate(damageTextPrefab, transform);
    }
    private void ActionOnGet(DamageText damageText)
    {
        damageText.gameObject.SetActive(true);
    }
    private  void ActionOnRelease(DamageText damageText)
    {
        damageText.gameObject.SetActive(false);
    }
    private  void ActionOnDestroy(DamageText damageText)
    {
        Destroy(damageText.gameObject);
    }
    // Update is called once per frame


    private void EnemyHitFallback(int damage,Vector2 enemyPos)
    {
        Vector3 spawnPosition = enemyPos+Vector2.up * 2;
        DamageText damageTextInstance = damageTextPool.Get();
        damageTextInstance.transform.position = spawnPosition;
        damageTextInstance.Animate(damage);
        LeanTween.delayedCall(1, () => damageTextPool.Release(damageTextInstance));
    }
}
