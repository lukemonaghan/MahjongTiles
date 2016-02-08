using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace SerializableData
{
    [Serializable]
    public class Position
    {
        [SerializeField] protected float _X;
        [SerializeField] protected float _Y;
        [SerializeField] protected float _Z;

        public Position()
        {
            _X = _Y = _Z = 0.0f;
        }

        public Position(float x, float y, float z)
        {
            _X = x;
            _Y = y;
            _Z = z;
        }

        public Position(Vector3 pos)
        {
            _X = pos.x;
            _Y = pos.y;
            _Z = pos.z;
        }
 
        [XmlIgnore]
        public Vector3 Internal
        {
            get
            {
                return new Vector3(_X, _Y, _Z);
            }
        
            set
            {
                _X = value.x;
                _Y = value.y;
                _Z = value.z;
            }
        }

        public float x { get { return _X; } set { _X = value; } }
        public float y { get { return _Y; } set { _Y = value; } }
        public float z { get { return _Z; } set { _Z = value; } }

        // CBF
        // public Vector2 xy { get { return _X; } set { _X = value; } }
        // public Vector2 yz { get { return _Y; } set { _Y = value; } }
        // public Vector2 zx { get { return _Z; } set { _Z = value;} }
        // CBF

        public float this[int index] 
        {
            set
            {
                switch (index)
                {
                    case 0:_X = value;break;
                    case 1:_Y = value;break;
                    case 2:_Z = value;break;
                    default:throw new IndexOutOfRangeException();
                }
            }
            get
            {
                switch (index)
                {
                    case 0:return _X;
                    case 1:return _Y;
                    case 2:return _Z;
                    default:throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [Serializable]
    public class Level
    {
        public string name;
        public List<Position> nodes = new List<Position>(); 
    }
}

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<LevelManager>()); }
    }

    public string extension = ".lvl";
    public List<SerializableData.Level> loadedLevels = new List<SerializableData.Level>();

    /// <summary>
    /// Loads levels from the Application.persistentDataPath
    /// </summary>
    public void LoadLevels()
    {
        loadedLevels.Clear();

        // Load the default levels
        var dirInfo = new DirectoryInfo(Application.dataPath + Path.DirectorySeparatorChar + "Levels");
        var files = dirInfo.GetFiles("*" + extension);

        foreach (var f in files)
        {
            loadedLevels.Add(Load(f.FullName));
        }
        
        // Load the custom user levels
        dirInfo = new DirectoryInfo(Application.persistentDataPath);
        files = dirInfo.GetFiles("*" + extension);

        foreach (var f in files)
        {
            loadedLevels.Add(Load(f.FullName));
        }
    }

    /// <summary>
    /// Saves the currently loaded levels to Application.persistentDataPath
    /// </summary>
    public void SaveLevels()
    {
        var dirInfo = Application.persistentDataPath;

        foreach (var level in loadedLevels)
        {
            var file = dirInfo + Path.DirectorySeparatorChar + level.name + extension;
            Save(file,level);
        }
    }

    private void Save(string filename, SerializableData.Level level)
    {
        var serializer = new XmlSerializer(typeof(SerializableData.Level));
        var encoding = Encoding.GetEncoding("UTF-8");
        using (var stream = new StreamWriter(filename, false, encoding))
        {
            serializer.Serialize(stream, level);
        }
    }

    private SerializableData.Level Load(string filename)
    {
        if (!File.Exists(filename)) 
            throw new FileNotFoundException();

        SerializableData.Level level;
        var serializer = new XmlSerializer(typeof(SerializableData.Level));
        var encoding = Encoding.GetEncoding("UTF-8");
        using (var stream = new StreamReader(filename, encoding))
        {
            level = (SerializableData.Level)serializer.Deserialize(stream);
        }
        return level;
    }

    public void ShowPair()
    {
        var pairs = Level.Instance.GetPairs();
        var pair = pairs[UnityEngine.Random.Range(0, pairs.Count)];
        pair.Key.Pulse();
        pair.Value.Pulse();
    }

    public struct TileInfo
    {
        public Vector3 position;
        public GameParameters.Tiles tile;
    }

    private Dictionary<int, KeyValuePair<TileInfo, TileInfo>> moves = new Dictionary<int, KeyValuePair<TileInfo, TileInfo>>();

    public void Clear()
    {
        moves.Clear();
    }

    public void Record(Tile a, Tile b)
    {
        var infoa = new TileInfo()
        {
            position = a.transform.position,
            tile = a.tile
        };

        var infob = new TileInfo()
        {
            position = b.transform.position,
            tile = b.tile
        };

        moves.Add(moves.Count, new KeyValuePair<TileInfo, TileInfo>(infoa, infob));
    }

    public void Undo()
    {
        if (moves.Count == 0) { return; }

        var index = moves[moves.Count - 1];
        CreateTile(index.Key);
        CreateTile(index.Value);
        moves.Remove(moves.Count - 1);
        int pairs = Level.Instance.PairCount();
        UIManager.Instance.inGameMenu.UndoText(pairs);
    }

    public void CreateTile(TileInfo ti)
    {
        var OBJ = Instantiate(GameParameters.Instance.tile);
        var tile = OBJ.GetComponent<Tile>();
        tile.transitionType = Tile.TransitionType.Pop;
        tile.tile = ti.tile;
        tile.transform.position = ti.position;
        Level.Instance.tiles.Add(tile);
    }

    public void OpenDirectory()
    {
        var proc = new Process();
        proc.StartInfo.FileName = Application.persistentDataPath;
        proc.Start();
    }
}
