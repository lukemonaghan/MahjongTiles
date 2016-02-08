using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour 
{
    public Text leftCount;
    public GameObject pauseMenu;
    public RawImage hoverTile;

    public void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Backspace))
#else
        if (Input.GetKeyDown(KeyCode.Escape))
#endif
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }

        var point = Level.Instance.GetMousePosition();
        var tile = point.collider ? point.collider.GetComponentInParent<Tile>() : null;
        hoverTile.texture = tile ? tile.tileRenderer.material.mainTexture : GameParameters.Instance.blank;
    }

    public int tileCount = 0;
    public int tileMatches = 0;

    public void Reset(int count)
    {
        tileCount = count;
        leftCount.text = "Press ESC to open menu\nTiles Left:" + tileCount + "\nMatches:" + tileMatches;
    }

    public void UndoText(int matches)
    {
        tileCount += 2;
        tileMatches = matches;
        leftCount.text = "Press ESC to open menu\nTiles Left:" + tileCount + "\nMatches:" + tileMatches;
    }

    public void UpdateText(int matches)
    {
        tileCount -= 2;
        tileMatches = matches;
        leftCount.text = "Press ESC to open menu\nTiles Left:" + tileCount + "\nMatches:" + tileMatches;
    }
}
