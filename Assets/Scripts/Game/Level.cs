using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Level : MonoBehaviour 
{
    // Static singleton Instance per level..
    private static Level _instance;
    public static Level Instance
    {
        get
        {
            return _instance = _instance ?? FindObjectOfType<Level>();
        }
    }

    public List<Transform> nodes = new List<Transform>();
    [HideInInspector] public List<Tile> tiles = new List<Tile>();
    [HideInInspector] public Tile selectedTile = null;

    void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(Instance.gameObject);
                _instance = this;
            }
        }
    }

    protected void OnDestroy()
    {
        foreach (var t in tiles)
        {
            if (t != null)
            {
                Destroy(t.gameObject);
            }
        }
        LevelManager.Instance.Clear();
    }

	public void Create(SerializableData.Level level) 
    {
        ClearNodes();
	    foreach (var n in level.nodes)
	    {
	        var node = new GameObject().transform;
	        node.position = n.Internal;
	        nodes.Add(node);
	    }

	    GenerateTiles();
        int pair = FillTiles();
        while (pair == 0)
        {
            pair = FillTiles();
        }

        UIManager.Instance.inGameMenu.Reset(nodes.Count);
        ClearNodes();
    }

    void GenerateTiles()
    {
        for (int i = 0; i < nodes.Count; ++i)
        {
            var tileOBJ = Instantiate(GameParameters.Instance.tile);
            var tile = tileOBJ.GetComponent<Tile>();
            tiles.Add(tile);
        }
    }

    int FillTiles()
    {
        nodes = nodes.Randomize().ToList();

        var groups = GameParameters.Instance.tiles;

        for (int i = 0; i < nodes.Count; i += 2)
        {
            int currentGroup = Random.Range(0, groups.Length);
            while (groups[currentGroup].current == groups[currentGroup].count)
            {
                currentGroup = Random.Range(0, groups.Length);
            }
            SetupTile(i, currentGroup);
            SetupTile(i+1, currentGroup);
        }

        foreach (var g in groups)
        {
            g.current = 0;
        }

        FillSelections();
        int pairs = PairCount();
        UIManager.Instance.inGameMenu.UpdateText(pairs);
        return pairs;
    }

    void SetupTile(int index,int currentGroup)
    {
        var groups = GameParameters.Instance.tiles;

        var tile = tiles[index];
        tile.transform.position = nodes[index].position;

        tile.tile = groups[currentGroup].tile;
        tile.subIndex = Mathf.Min(groups[currentGroup].current++, groups[currentGroup].textures.Length - 1);
        tile.blockRenderer.material.color = GameParameters.Instance.nonSelectableColour;
    }

    void ClearNodes()
    {
        foreach (var n in nodes)
        {
            Destroy(n.gameObject);
        }
        nodes.Clear();
    }

    public bool CanSelect(Tile tile)
    {
        return ((tile.leftTiles.Count == 0 || tile.rightTiles.Count == 0) && tile.topTiles.Count == 0);
    }

    public void FillSelections()
    {
        foreach (var tile in tiles)
        {
            var pos = tile.transform.position;
            foreach (var t in tiles)
            {
                var dirx = t.transform.position.x - pos.x;
                var diry = t.transform.position.y - pos.y;
                var dirz = t.transform.position.z - pos.z;
                // to the left?
                if (dirx < -0.95f && dirx > -1.05f && Mathf.Abs(dirz) < 1.5f && Mathf.Abs(diry) < 0.1f)
                {
                    tile.leftTiles.Add(t);
                }
                // to the right?
                if (dirx > 0.95f && dirx < 1.05f && Mathf.Abs(dirz) < 1.5f && Mathf.Abs(diry) < 0.1f)
                {
                    tile.rightTiles.Add(t);
                }
                // Something ontop?
                if (t.transform.position.y > pos.y && Mathf.Abs(dirx) < 1.0f && Mathf.Abs(dirz) < 2.0f)
                {
                    tile.topTiles.Add(t);
                    t.bottomTiles.Add(tile);
                }
            }
        }
    }

    public int PairCount()
    {
        var list = GetPairs();
        var count = list.Count;
        return count;
    }

    public List<KeyValuePair<Tile, Tile>> GetPairs()
    {
        var values = new List<KeyValuePair<Tile, Tile>>();
        var selectables = tiles.Where(CanSelect).ToList();
        var indi = new Dictionary<int,Tile>();
        foreach (var t in selectables)
        {
            t.blockRenderer.material.color = Color.white;
            if (indi.ContainsKey((int)t.tile))
            {
                values.Add(new KeyValuePair<Tile, Tile>(indi[(int)t.tile],t));
                indi.Remove((int)t.tile);
            }
            else
            {
                indi.Add((int)t.tile,t);
            }
        }
        return values;
    }

    public int TileCount()
    {
        return tiles.Count; 
    }

    public RaycastHit GetMousePosition()
    {
        var cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawLine(ray.origin,ray.direction * 100,Color.cyan);
        Physics.Raycast(ray, out hit);
        return hit;
    }

    public void RemoveTile(Tile tile)
    {
        tiles.Remove(tile);
        if (tile.bottomTiles.Count > 0)
        {
            foreach(var t in tile.bottomTiles)
            {
                t.topTiles.Remove(tile);
                if (CanSelect(t))
                {
                    t.blockRenderer.material.color = Color.white;
                }
            }
        }
        if (tile.leftTiles.Count > 0)
        {
            foreach(var t in tile.leftTiles)
            {
                t.rightTiles.Remove(tile);
                if (CanSelect(t))
                {
                    t.blockRenderer.material.color = Color.white;
                }
            }
        }
        if (tile.rightTiles.Count > 0)
        {
            foreach (var t in tile.rightTiles)
            {
                t.leftTiles.Remove(tile);
                if (CanSelect(t))
                {
                    t.blockRenderer.material.color = Color.white;
                }
            }
        }
    }
}

public static class IEnumerableExtensions
{
    public static IEnumerable<t> Randomize<t>(this IEnumerable<t> target)
    {
        var r = new System.Random();
        return target.OrderBy(x => (r.Next()));
    }
}
