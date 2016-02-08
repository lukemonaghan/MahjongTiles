using System.Collections.Generic;
using UnityEngine;

public class UILevelSelect : MonoBehaviour
{
    public RectTransform scrollPanel;
    public List<UILevelSelectItem> levels = new List<UILevelSelectItem>();

    public void OnEnable()
    {
        LevelManager.Instance.LoadLevels();

        for (int index = 0; index < LevelManager.Instance.loadedLevels.Count; index++)
        {
            var level = LevelManager.Instance.loadedLevels[index];
            UILevelSelectItem lsi;
            if (levels.Count <= index)
            {
                var levelOBJ = Instantiate(levels[0].gameObject);
                levelOBJ.transform.SetParent(levels[0].transform.parent);
                var levelRect = levelOBJ.transform as RectTransform;
                levelRect.anchoredPosition = new Vector2(0, 64 * -index);
                levelRect.sizeDelta = new Vector2(0, 64);
                lsi = levelOBJ.GetComponent<UILevelSelectItem>();
                levels.Add(lsi);
            }
            else
            {
                lsi = levels[index];
                lsi.gameObject.SetActive(true);
            }

            lsi.text.text = level.name;
            lsi.button.onClick.RemoveAllListeners();
            var levelIndex = index;
            lsi.button.onClick.AddListener(() => UIManager.Instance.StartGame(levelIndex));
        }
        for (int index = LevelManager.Instance.loadedLevels.Count; index < levels.Count; ++index)
        {
            levels[index].gameObject.SetActive(false);
        }

        scrollPanel.sizeDelta = new Vector2(scrollPanel.sizeDelta.x, 64 * LevelManager.Instance.loadedLevels.Count);
    }
}
