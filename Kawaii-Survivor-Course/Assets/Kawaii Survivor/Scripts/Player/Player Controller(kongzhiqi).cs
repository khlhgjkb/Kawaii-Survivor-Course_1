using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private MobileJoystick playerJoystick;
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        rig.velocity = Vector2.right;
    }
    private void FixedUpdate()
    {
        rig.velocity = playerJoystick.GetMoveVector() * moveSpeed * Time.deltaTime;
    }
}
