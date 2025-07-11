using System.Collections;
using System.Collections.Generic;

        using UnityEngine;

        [RequireComponent(typeof(Rigidbody2D))]
        public class TopDownPlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f; // �ƶ��ٶ�

        private Rigidbody2D rig; // Rigidbody2D ���

        private void Start()
        {
            // ��ȡ Rigidbody2D ���
            rig = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            // ��ȡ��������
            Vector2 moveVector = GetKeyboardInput();

            // Ӧ���ƶ�
            rig.velocity = moveVector * moveSpeed;
        }

        private Vector2 GetKeyboardInput()
        {
            // ��ȡ�������루WASD �������
            float horizontal = Input.GetAxisRaw("Horizontal"); // A/D �� ���Ҽ�ͷ
            float vertical = Input.GetAxisRaw("Vertical");     // W/S �� ���¼�ͷ

            // ���ع�һ�����ƶ�����
            return new Vector2(horizontal, vertical).normalized;
        }
    }


    // Update is called once per frame

