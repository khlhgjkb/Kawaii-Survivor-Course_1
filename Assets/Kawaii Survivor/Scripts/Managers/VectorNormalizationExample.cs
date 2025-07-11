using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorNormalizationExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        {
            // 定义原始向量
            Vector3 originalVector = new Vector3(34, 44, 0);

            // 标准化向量
            Vector3 normalizedVector = originalVector.normalized;

            // 输出结果
            Debug.Log("Original Vector: " + originalVector);
            Debug.Log("Normalized Vector: " + normalizedVector);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
