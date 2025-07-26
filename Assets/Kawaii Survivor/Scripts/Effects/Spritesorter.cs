using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spritesorter : MonoBehaviour
{

    [Header("Elements")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //spriteRenderer.sortingOrder = -(int)(transform.position.y * 10);
    }
}
