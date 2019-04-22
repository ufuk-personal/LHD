using UnityEngine;
using System.Collections;

public class PageOpener : MonoBehaviour
{
    public void OpenPage(string pageIdentifier) {
        FoldPanel.Instance.OpenPage(pageIdentifier);
    }
}
