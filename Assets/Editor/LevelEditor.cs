using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var handle = target as Level;

        if (GUILayout.Button("Save"))
        {
            var level = new SerializableData.Level();
            level.name = handle.name;

            foreach (var h in handle.nodes)
            {
                level.nodes.Add(new SerializableData.Position(h.transform.position));
            }

            LevelManager.Instance.loadedLevels.Add(level);
        }
    }
}
