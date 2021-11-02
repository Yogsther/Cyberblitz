using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    [ColorUsage(true, true)] public Color color;

    public Material outlineMaterial;
    public List<Renderer> renderers;

    private Color lastColor;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>().ToList();

        foreach(Renderer renderer in renderers)
        {
            renderer.materials = new Material[] { renderer.material, outlineMaterial };
        }
    }

    private void Update()
    {
        if(color != lastColor)
        {
            foreach(Renderer renderer in renderers)
            {
                renderer.materials[1].SetColor("_Color", color);
            }

            lastColor = color;
        }
    }
}
