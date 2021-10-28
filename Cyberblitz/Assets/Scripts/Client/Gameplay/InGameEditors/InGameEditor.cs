using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InGameEditor : MonoBehaviour
{
    protected Vector2 inputCoordinates;
    protected bool cursorDown;

    public Action OnUpdated;

    public void EditorInput(Vector2 input)
    {
        inputCoordinates = input;
    }

}
