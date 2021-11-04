using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    [ColorUsage(true, true)] public Color color;

    public Material outlineMaterial;
    public List<Renderer> mainRenderers;
    public List<SelectionColor> outlineRenderers;

    private Color lastColor;

    private void Start()
    {
        mainRenderers = GetComponentsInChildren<Renderer>().ToList();

        CreateOutlineRenderers();
    }

    [ContextMenu("Create Outline")]
    public void CreateOutlineRenderers()
    {
        List<Renderer> mainRenderers = GetComponentsInChildren<Renderer>().ToList();

        foreach (Renderer renderer in mainRenderers)
        {
            SelectionColor controller = renderer.gameObject.AddComponent<SelectionColor>();

            outlineRenderers.Add(controller);

        }
    }

    private void Update()
    {
        if(color != lastColor)
        {
            UpdateColors();
        }
    }

    private void UpdateColors()
    {
        foreach (SelectionColor selectionColor in outlineRenderers)
        {
            selectionColor.SetColor(color);
        }

        lastColor = color;
    }
}
