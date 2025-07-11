using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int health ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        int realDamage = Mathf.Min(damage,health);
        health -= realDamage;

        if (health <= 0)
        {
            PassAway();
        }
    }
    private void PassAway()
    {
        Debug.Log("Ded");
    }
}


