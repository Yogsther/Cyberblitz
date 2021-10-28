using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    private static Level level;

    private void OnEnable()
    {
        level = (Level)target;

        level.UpdateLevelElements();


        if (level.showGameObjectsInEditor)
        {
            level.ShowElementGameObjects();
        }
    }

    private void OnDisable()
    {
        level.HideElementGameObjects();
    }

    private void OnSceneGUI()
    {
        Draw();
    }

    private void Draw()
    {
        foreach (LevelElement element in level.levelElements)
        {

            //Handles.RectangleHandleCap(1, element.gridPosition.ToFlatVector3(), Quaternion.identity, 1f, EventType.DragPerform);

            Vector2 newPos = Handles.DoPositionHandle(element.gridTransform.position.ToFlatVector3(), Quaternion.identity).FlatVector3ToVector2();

            if (newPos != element.gridTransform.position)
            {
                Undo.RecordObject(level, "Move Collider");
                element.gridTransform.position = newPos.RoundToVector2Int();
                level.UpdateLevelElements();
            }
        }
    }
}