using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Aselsan/UIPageConfig")]
public class PageContent : ScriptableObject
{
    public string PageIdentifier;
    public List<GameObject> ButtonPrefabs;
    public List<Sprite> Images;
    public GameObject TextPrefab;
    public GameObject TextPrefabTR;
    public List<string> TransparentObjects;
    public List<string> BlinkingObjects;
    public List<string> PageSpecificHiddenObjects;
    public List<string> PageSpecificVisibleObjects;
    public string HeaderText;
    public string HeaderTextTR;
    public int CameraPathID;
}
