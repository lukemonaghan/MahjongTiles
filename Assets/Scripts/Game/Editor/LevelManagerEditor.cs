using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        var handle = target as LevelManager;
        DrawDefaultInspector();
        if (GUILayout.Button("Load"))
        {
            handle.LoadLevels();
        }
        if (GUILayout.Button("Save"))
        {
            handle.SaveLevels();
        }
        if (GUILayout.Button("Open"))
        {
            handle.OpenDirectory();
        }
    }
}
