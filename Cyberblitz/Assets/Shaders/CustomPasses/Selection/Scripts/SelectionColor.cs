using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SelectionColor : MonoBehaviour
{
    [ColorUsage(true, true)] public Color selectionColor = new Color(1f, 0.5f, 0f, 1f);

    // Start is called before the first frame update
    void Start()
    {
        SetColor(selectionColor);
    }

    void OnValidate()
    {
        SetColor(selectionColor);
    }

    public void SetColor(Color color)
    {
        var rndr = GetComponent<Renderer>();

        var propertyBlock = new MaterialPropertyBlock();
        rndr.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_SelectionColor", color);

        rndr.SetPropertyBlock(propertyBlock);
    }
}
