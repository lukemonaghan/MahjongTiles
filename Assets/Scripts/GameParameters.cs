using UnityEngine;
using System.Collections.Generic;

/// A place to store miscellaneous global parameters.
public class GameParameters : ScriptableObject 
{
    // Convenience method for accessing GameParameters asset
    public static GameParameters Instance
    {
        get
        {
            if (_instance == null)
                _instance = (GameParameters)Resources.Load("GameParameters", typeof(GameParameters));
            return _instance;
        }
    }
    static GameParameters _instance;

    public enum Tiles
    {
        NULL_BLANK,
        Bamboo1,
        Bamboo2,
        Bamboo3,
        Bamboo4,
        Bamboo5,
        Bamboo6,
        Bamboo7,
        Bamboo8,
        Bamboo9,
        Man1,
        Man2,
        Man3,
        Man4,
        Man5,
        Man6,
        Man7,
        Man8,
        Man9,
        Pin1,
        Pin2,
        Pin3,
        Pin4,
        Pin5,
        Pin6,
        Pin7,
        Pin8,
        Pin9,
        DIRECTION_North,
        DIRECTION_East,
        DIRECTION_South,
        DIRECTION_West,
        DRAGON_Chun,
        DRAGON_Green,
        DRAGON_Haku,
        FLOWER,
        SEASON,
        NULL_END,
    }

    [System.Serializable]
    public class TileGroup
    {
        public Tiles tile;
        public Texture2D[] textures;
        public int count = 4;
        [System.NonSerialized] public int current;
    }

    [Header("Tiles")]
    public Texture2D blank;
    public TileGroup[] tiles;
    
    [Header("Prefabs")]
    public GameObject tile;
    public GameObject dustPoof;
    public GameObject levelEditor;

    [Header("Other")]
    public Color nonSelectableColour = new Color(0.5f,0.5f,0.5f,1);
    public Color selectionColour = Color.red;
    public AudioClip tick;
    [TextArea(3,99)] public string Credits = "";

    // Functions
    public Texture2D GetGroupTexture(Tiles tile,int subindex)
    {
        if (tile == Tiles.NULL_BLANK)
        {
            return blank;
        }

        foreach(var t in tiles)
        {
            if (t.tile == tile)
            {
                return t.textures[subindex];
            }
        }
        return null;
    }

    public void OnColourChange(Color newColour)
    {
        selectionColour = newColour;
        PlayerPrefs.SetFloat("C_R", newColour.r);
        PlayerPrefs.SetFloat("C_G", newColour.g);
        PlayerPrefs.SetFloat("C_B", newColour.b);
        PlayerPrefs.Save();
    }

    public void LoadColour()
    {
        Color c = Color.black;
        c.r = PlayerPrefs.GetFloat("C_R", 1);
        c.g = PlayerPrefs.GetFloat("C_G", 1);
        c.b = PlayerPrefs.GetFloat("C_B", 0);
        selectionColour = c;
    }

    public void LoadAudioLevel()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Vol", 1);
    }

    public void CreateLevel(int levelindex)
    {
        var obj = new GameObject();
        obj.transform.position = Vector3.zero;
        var level = obj.AddComponent<Level>();

        level.Create(LevelManager.Instance.loadedLevels[levelindex]);
    }

    public void CreateLevelEditor()
    {
        var editor = Instantiate(levelEditor);
        editor.transform.position = Vector3.zero;
    }
}
