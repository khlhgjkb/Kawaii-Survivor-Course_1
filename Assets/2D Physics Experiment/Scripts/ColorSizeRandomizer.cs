using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ColorSizeRandomizer : MonoBehaviour
{
    [Header(" Texture ")]
    [SerializeField] private Texture2D tex;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.mainTexture = tex;
        //GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(Random.Range(0f, 1f), .8f, 1f);

        Color color = Color.HSVToRGB((float)transform.GetSiblingIndex() / 2000, .8f, 1f);
        GetComponent<MeshRenderer>().material.color = color;

        //transform.localScale = Random.Range(.1f, .8f) * Vector2.one;
        transform.localScale = .3f * Vector2.one;
    }
}
