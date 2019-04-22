using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour {

    public Text languageText;
    public bool isEnglish = true;

    public static SettingsController Instance = null;
    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            isEnglish = true;
            languageText.text = "EN";
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void OnLanguageChanged() {
        isEnglish = !isEnglish;
        if (isEnglish) {
            languageText.text = "EN";
        }
        else
        {
            languageText.text = "TR";
        }
        FoldPanel.Instance.OpenPage(FoldPanel.Instance.lastOpenPage);
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
