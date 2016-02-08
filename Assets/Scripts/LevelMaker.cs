using UnityEngine;

public class LevelMaker : Level
{
    public Renderer grid;
    public Tile currentTile;
    public Vector3 snapping = new Vector3(1,0.5f,1);

    void Start()
    {
        grid.material.color = GameParameters.Instance.selectionColour;
        currentTile = Instantiate(GameParameters.Instance.tile).GetComponent<Tile>();
        Destroy(currentTile.GetComponent<Collider>());
    }

    void OnDestroy()
    {
        base.OnDestroy();
        if (currentTile != null)
        {
            Destroy(currentTile.gameObject);
        }
    }

    void Update()
    {
        if (UIManager.Instance.levelEditor.pauseMenu.activeSelf)
        {
            currentTile.transform.position = Vector3.down * 5;
            return;
        }

        var hit = GetMousePosition();
        var pos = hit.point;
        pos.x = Mathf.Round(pos.x / snapping.x) * snapping.x;
        pos.y = Mathf.Round(pos.y / snapping.y) * snapping.y;
        pos.z = Mathf.Round(pos.z / snapping.z) * snapping.z;
        currentTile.transform.position = pos;

        if (Input.GetMouseButtonDown(0) && tiles.Count < 144)
        {
            var newTile = Instantiate(GameParameters.Instance.tile).GetComponent<Tile>();
            newTile.transform.position = currentTile.transform.position;
            newTile.canSelect = false;
            newTile.transitionType = Tile.TransitionType.None;
            tiles.Add(newTile);
            UIManager.Instance.levelEditor.SetCount(tiles.Count);
        }

        if (Input.GetMouseButtonDown(1) && hit.transform.tag == "Tile")
        {
            var tile = hit.transform.GetComponent<Tile>();
            Destroy(tile.gameObject);
            tiles.Remove(tile);
            UIManager.Instance.levelEditor.SetCount(tiles.Count);
        }

    }
}
