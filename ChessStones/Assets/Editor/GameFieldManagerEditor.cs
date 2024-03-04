using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(GameFieldManager))]
public class GameFieldManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameFieldManager manager = (GameFieldManager)target;

        if (GUILayout.Button("PlaceFigures"))
        {
            manager.PlaceFigures();
        }

    }
}
