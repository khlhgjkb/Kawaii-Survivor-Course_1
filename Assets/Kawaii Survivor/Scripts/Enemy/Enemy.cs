using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Elements")]
    private Player player;

    [Header("Settings")]
    [SerializeField] private float moveSpeed;
    // Update is called once per frame
    void Update()
    {
       if (player != null)
            FollowPlayer();

    }

    public void StorePlayer(Player player)
    {
        this.player = player;
        
    }

   
    
    private void FollowPlayer()
    {
        Vector2 targetPosition = Vector2.MoveTowards(transform.position,player.transform.position,moveSpeed*Time.deltaTime);
        transform.position = targetPosition;
    }


    


}
