using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageAwareButton : MonoBehaviour {

    [SerializeField] private GameObject textTR;
    [SerializeField] private GameObject textEN;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (SettingsController.Instance.isEnglish) {
            if (!textEN.activeSelf)
            {
                textEN.SetActive(true);
            }
            if (textTR.activeSelf)
            {
                textTR.SetActive(false);
            }
        }
        else
        {
            if (!textTR.activeSelf)
            {
                textTR.SetActive(true);
            }
            if (textEN.activeSelf)
            {
                textEN.SetActive(false);
            }
        }
	}
}
