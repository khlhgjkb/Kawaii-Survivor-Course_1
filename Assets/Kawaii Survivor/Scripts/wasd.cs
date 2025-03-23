using System.Collections;
using System.Collections.Generic;

        using UnityEngine;

        [RequireComponent(typeof(Rigidbody2D))]
        public class TopDownPlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f; // 移动速度

        private Rigidbody2D rig; // Rigidbody2D 组件

        private void Start()
        {
            // 获取 Rigidbody2D 组件
            rig = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            // 获取键盘输入
            Vector2 moveVector = GetKeyboardInput();

            // 应用移动
            rig.velocity = moveVector * moveSpeed;
        }

        private Vector2 GetKeyboardInput()
        {
            // 获取键盘输入（WASD 或方向键）
            float horizontal = Input.GetAxisRaw("Horizontal"); // A/D 或 左右箭头
            float vertical = Input.GetAxisRaw("Vertical");     // W/S 或 上下箭头

            // 返回归一化的移动方向
            return new Vector2(horizontal, vertical).normalized;
        }
    }


    // Update is called once per frame

