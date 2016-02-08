using SerializableData;
using UnityEngine;
using UnityEngine.UI;

public class UILevelEditor : MonoBehaviour
{
    public Text uiHelperText;
    public InputField levelNameField;
    public GameObject pauseMenu;

    void Start()
    {
        UpdateText();
    }

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
    }

    public void Save()
    {
        pauseMenu.SetActive(false);

        var level = new SerializableData.Level();
        level.name = levelNameField.text;

        foreach (var n in Level.Instance.tiles)
        {
            level.nodes.Add(new Position(n.transform.position));
        }

        LevelManager.Instance.loadedLevels.Clear();
        LevelManager.Instance.loadedLevels.Add(level);
        LevelManager.Instance.SaveLevels();
        LevelManager.Instance.LoadLevels();
    }

    public bool IsEven(int value)
    {
        return value % 2 == 0;
    }

    private string message;
    private int count;

    public void SetMessage(string message)
    {
        this.message = message;
    }
    
    public void SetCount(int count)
    {
        this.count = count;
        UpdateText();
    }

    public void UpdateText()
    {
        if (Level.Instance.tiles.Count == 0)
        {
            SetMessage("You have no tiles.");
        }
        else if (IsEven(Level.Instance.tiles.Count) == false)
        {
            SetMessage("You have an uneven number of tiles.");
        }
        else if (levelNameField.text == "")
        {
            SetMessage("You havent given the level a name.");
        }
        else
        {
            SetMessage("");
        }

        uiHelperText.text = "Press ESC to open menu\ntiles:" + count + "\n" + message;
    }
}

