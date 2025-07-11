using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private float range;
    [SerializeField] private LayerMask enemyMask; 

    [Header(" Animations ")]
    [SerializeField] private float aimLerp;
    // Start is called before the first frame update
    void Start()
    {


    }
    // Update is called once per frame
    void Update()
    {
        Enemy closestEnemy = null;
        Vector2 targetUpVector = Vector3.up;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, enemyMask);
        if (enemies.Length <= 0)
        {
            transform.up = Vector3.up; return;
        }
        float minDistance = range;
        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemyChecked = enemies[i].GetComponent<Enemy>();
            // 检查获取的组件是否为 null
            if (enemyChecked != null)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemyChecked.transform.position);
                if (distanceToEnemy < minDistance)
                {
                    closestEnemy = enemyChecked;
                    minDistance = distanceToEnemy;
                }
            }
        }
        // 使用 == 进行比较
        if (closestEnemy == null)
        {
            transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);
            //transform. up = targetUpVector;
            return;
        }
        targetUpVector = (closestEnemy.transform.position - transform.position).normalized;
        transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);
    }
        
    private void OnDrawGizmosSelected()
    {
        Gizmos. color = Color. magenta;
        Gizmos. DrawWireSphere(transform. position, range);
    
    }
}
