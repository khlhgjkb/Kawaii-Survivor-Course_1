using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Elements")]
    [SerializeField] private Animator animator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [NaughtyAttributes.Button]
    private void Animate()
    {
        animator.Play("Animate");
    }
}
