using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorNormalizationExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        {
            // ����ԭʼ����
            Vector3 originalVector = new Vector3(34, 44, 0);

            // ��׼������
            Vector3 normalizedVector = originalVector.normalized;

            // ������
            Debug.Log("Original Vector: " + originalVector);
            Debug.Log("Normalized Vector: " + normalizedVector);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
