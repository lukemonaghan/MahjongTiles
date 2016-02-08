using UnityEngine;

public class UIManager : MonoBehaviour 
{
    // Static singleton Instance per level..
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            return _instance = _instance ?? FindObjectOfType<UIManager>();
        }
    }

    AudioSource _audioSource;
    public AudioSource AudioSource
    {
        get
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = GameParameters.Instance.tick;
            return _audioSource;
        }
    }

    public GameObject playMenu;
    public GameObject looseMenu;
    public GameObject winMenu;
    public UIInGame inGameMenu;
    public UILevelEditor levelEditor;
    public UILevelSelect levelSelect;

    public void SetActiveAll(bool value)
    {
        playMenu.SetActive(value);
        winMenu.SetActive(value);
        looseMenu.SetActive(value);
        inGameMenu.gameObject.SetActive(value);
        levelEditor.gameObject.SetActive(value);
        levelSelect.gameObject.SetActive(value);
    }

    public void OnEnable()
    {
        SetActiveAll(false);
        GameParameters.Instance.LoadColour();
        playMenu.SetActive(true);
    }

    public void StartGame(int level)
    {
        // Disable menus
        SetActiveAll(false);
        inGameMenu.gameObject.SetActive(true);

        // Create a new level
        GameParameters.Instance.CreateLevel(level);
    }

    public void LevelEditor()
    {
        SetActiveAll(false);
        levelEditor.gameObject.SetActive(true);

        // Create Level Editor
        GameParameters.Instance.CreateLevelEditor();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void DisplayWin()
    {
        SetActiveAll(false);
        winMenu.SetActive(true);
    }

    public void DisplayLoose()
    {
        SetActiveAll(false);
        looseMenu.SetActive(true);
    }

    public void PlayTick()
    {
        AudioSource.Play();
    }
}
