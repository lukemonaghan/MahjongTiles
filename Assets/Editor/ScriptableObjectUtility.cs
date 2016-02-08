using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
		string path = "Assets/Resources/";
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + typeof(T).ToString() + ".asset");
		AssetDatabase.CreateAsset(asset, assetPathAndName);
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}
	
	[MenuItem("GameObject/Resource/GameParameters")]
	public static void CreateGameParameters()
	{
		CreateAsset<GameParameters>();
	}
}
