using UnityEngine;
using UnityEngine.UI;

public class UILoose : MonoBehaviour
{
    public Text info;

    void OnEnable()
    {
        info.text = "Tiles Left:" + UIManager.Instance.inGameMenu.tileCount;
    }
}
