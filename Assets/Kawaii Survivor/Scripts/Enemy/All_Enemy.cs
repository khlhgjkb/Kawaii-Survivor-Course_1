using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Enemy))]
public class All_Enemy : MonoBehaviour
{
    [Header("Compoents")]
    private Enemy movement;

    [Header("Elements")]
    private Player player;

    [Header("Spwn Sequence Related")]
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private SpriteRenderer spawnIndicator;
    private bool hasSpawned;

    [Header("Effects")]
    [SerializeField] private ParticleSystem passAwayParticles;

    [Header("Attack")]
    [SerializeField] private int damage;
    [SerializeField] private float attackFrequency;
    [SerializeField] private float playerDetectionRadius;
    private float attackDelay;
    private float attackTimer;

    [Header("DEBUG")]
    [SerializeField] private bool gizmos;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Enemy>();

        player = FindFirstObjectByType<Player>();

        if (player == null)
        {
            Debug.LogWarning("No player found, Auto-destroying...");
            Destroy(gameObject);
        }

        StartSpawnSqeuence();

        //隐藏渲染器
        renderer.enabled = false;
        //显示生成指示器
        spawnIndicator.enabled = true;

        //缩放生成指示器
        Vector3 targetScale = spawnIndicator.transform.localScale * 1.2f;
        LeanTween.scale(spawnIndicator.gameObject, targetScale, .3f)
            .setLoopPingPong(4)
            .setOnComplete(SpawnSequenceCompleted);

        attackDelay = 1f / attackFrequency;
        Debug.Log("Attack Delay : " + attackDelay);
    }

    private void StartSpawnSqeuence()
    {
        SetRenderesVisibility(false);
        //隐藏渲染器
        renderer.enabled = false;
        //显示生成指示器
        spawnIndicator.enabled = true;

        //缩放生成指示器
        Vector3 targetScale = spawnIndicator.transform.localScale * 1.2f;
        LeanTween.scale(spawnIndicator.gameObject, targetScale, .3f)
            .setLoopPingPong(4)
            .setOnComplete(SpawnSequenceCompleted);
    }

    private void SpawnSequenceCompleted()
    {
        SetRenderesVisibility();
        hasSpawned = true;

        movement.StorePlayer(player);
    }

    private void SetRenderesVisibility(bool visibility=true)
    {
        //隐藏渲染器
        renderer.enabled = visibility;
        //显示生成指示器
        spawnIndicator.enabled = !visibility;
    }
    // Update is called once per frame
    void Update()
    {
        if (attackTimer >= attackDelay)
            TryAttack();
        else
            Wait();
    }

    private void Wait()
    {
        attackTimer += Time.deltaTime;
    }

    private void TryAttack()
    {
        float distanceToplayert = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToplayert <= playerDetectionRadius)
        {
            Attack();
        }
    }

    private void Attack()
    {

        attackTimer = 0;

        player.TakeDamage(damage);
    }

    private void PassAway()
    {
        //解除粒子效果的父对象关系并播放
        passAwayParticles.transform.SetParent(null);
        passAwayParticles.Play();
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (!gizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}
