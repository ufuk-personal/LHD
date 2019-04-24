using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoldPanel : MonoBehaviour {

    public RectTransform rt;
    public RectTransform foldIcon1;
    public RectTransform foldIcon2;
    public bool isFolded = true;
    public float foldedX = 590;
    public float unfoldedX = 0;
    public float foldedIconZRot = 90;
    public float unfoldedIconZRot = 270;
    public float posLerpMultiplier = 0.1f;
    public float rotLerpMultiplier = 0.1f;
    public List<PageContent> pages;
    public List<PageSpecificObject> PageSpecificObjects;
    public RectTransform ButtonContainer;
    public Text HeaderText;
    public ImageSlideShowContainer ImageContainer;
    public RectTransform TextContainer;

    private Dictionary<string, PageContent> pageCache;
    private Vector3 targetPos;
    private Quaternion targetIconRot;
    private bool hasPosTarget = false;
    private bool hasRotTarget = false;

    public static FoldPanel Instance = null;

    public void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Start()
    {
        pageCache = new Dictionary<string, PageContent>();
        foreach(PageContent page in pages)
        {
            pageCache.Add(page.PageIdentifier, page);
        }

        rt = this.GetComponent<RectTransform>();
        if (isFolded)
        {
            rt.anchoredPosition = new Vector3(foldedX, rt.anchoredPosition.y);
            foldIcon1.localRotation = Quaternion.Euler(0, 0, foldedIconZRot);
            foldIcon2.localRotation = Quaternion.Euler(0, 0, foldedIconZRot);
        }
        else
        {
            rt.anchoredPosition = new Vector3(unfoldedX, rt.anchoredPosition.y);
            foldIcon1.localRotation = Quaternion.Euler(0, 0, unfoldedIconZRot);
            foldIcon2.localRotation = Quaternion.Euler(0, 0, unfoldedIconZRot);
        }
        PageSpecificObjects.AddRange(GameObject.FindObjectsOfType<PageSpecificObject>());
        OpenPage(pages[0].PageIdentifier);
    }

    public string lastOpenPage;

    public void OnClosePageObjects(string pageID)
    {
        if (!pageCache.ContainsKey(pageID))
        {
            return;
        }

        PageContent page = pageCache[pageID];
        foreach (TransparentObject transparentObj in GameObject.FindObjectsOfType<TransparentObject>())
        {
            foreach (string transparentObjID in page.TransparentObjects)
            {
                if(transparentObj.objectID == transparentObjID)
                {
                    Color c = transparentObj.GetComponent<Renderer>().material.color;
                    Color newC = new Color(c.r, c.g, c.b, 1f);
                    transparentObj.GetComponent<Renderer>().material.SetColor("_Color", newC);
                    transparentObj.GetComponent<Renderer>().materials[0].SetColor("_Color", newC);
                }
            }
        }

        foreach (string visibleObject in page.PageSpecificVisibleObjects)
        {
            foreach (PageSpecificObject pso in this.PageSpecificObjects)
            {
                if (pso.objectID == visibleObject)
                {
                    pso.gameObject.SetActive(false);
                }
            }
        }
        foreach (string hiddenObject in page.PageSpecificHiddenObjects)
        {
            foreach (PageSpecificObject pso in this.PageSpecificObjects)
            {
                if (pso.objectID == hiddenObject)
                {
                    pso.gameObject.SetActive(true);
                }
            }
        }
        
        BlinkingTarget[] blinks = Resources.FindObjectsOfTypeAll<BlinkingTarget>();
        foreach (string blinkID in page.BlinkingObjects)
        {
            foreach (BlinkingTarget bt in blinks)
            {
                if (bt.objectID == blinkID)
                {
                    bt.EndBlinking();
                }
            }
        }

    }
    public void OnOpenPageObjects(string pageID)
    {
        if (!pageCache.ContainsKey(pageID))
        {
            return;
        }
        PageContent page = pageCache[pageID];
        foreach (TransparentObject transparentObj in GameObject.FindObjectsOfType<TransparentObject>())
        {
            foreach (string transparentObjID in page.TransparentObjects)
            {
                if (transparentObj.objectID == transparentObjID)
                {
                    Color c = transparentObj.GetComponent<Renderer>().material.color;
                    Color newC = new Color(c.r, c.g, c.b, 0.2f);
                    transparentObj.GetComponent<Renderer>().material.SetColor("_Color", newC);
                    transparentObj.GetComponent<Renderer>().materials[0].SetColor("_Color", newC);
                }
            }
        }

        foreach(string visibleObject in page.PageSpecificVisibleObjects)
        {
            foreach(PageSpecificObject pso in this.PageSpecificObjects)
            {
                if(pso.objectID == visibleObject)
                {
                    pso.gameObject.SetActive(true);
                }
            }
        }
        foreach (string hiddenObject in page.PageSpecificHiddenObjects)
        {
            foreach (PageSpecificObject pso in this.PageSpecificObjects)
            {
                if (pso.objectID == hiddenObject)
                {
                    pso.gameObject.SetActive(false);
                }
            }
        }

        BlinkingTarget[] blinks = Resources.FindObjectsOfTypeAll<BlinkingTarget>();
        foreach (string blinkID in page.BlinkingObjects)
        {
            foreach (BlinkingTarget bt in blinks)
            {
                if (bt.objectID == blinkID)
                {
                    bt.StartBlinking();
                }
            }
        }
    }
    public void OpenPage(string pageIdentifier) {

        if (!pageCache.ContainsKey(pageIdentifier))
        {
            Debug.LogError("Missing Page");
            return;
        }

        Camera.main.fieldOfView = 60f;

        OnClosePageObjects(lastOpenPage);
        OnOpenPageObjects(pageIdentifier);
        
        PageContent page = pageCache[pageIdentifier];

        ButtonContainer.DestroyAllChildren();
        foreach (GameObject pageButton in page.ButtonPrefabs) {
            Instantiate(pageButton, ButtonContainer, false);
        }
        float bcSize = (Mathf.CeilToInt((page.ButtonPrefabs.Count + 0f) / 3f) * 97) + 9;
        ButtonContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bcSize);

        float icSize = ImageContainer.SetupSlideShow(page.Images);
        
        float tcSize = 981 - (icSize + bcSize);



        TextContainer.DestroyAllChildren();
        if (page.TextPrefab != null)
        {
            TextContainer.gameObject.SetActive(true);
            TextContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tcSize);
            if (SettingsController.Instance.isEnglish || page.TextPrefabTR == null)
            {
                Instantiate(page.TextPrefab, TextContainer, false);
            }
            else
            {
                Instantiate(page.TextPrefabTR, TextContainer, false);
            }
        }
        else
        {
            TextContainer.gameObject.SetActive(false);
        }
        if (page.CameraPathID != -1)
        {
            CameraController.Instance.SwitchToPath(page.CameraPathID);
        }
        if (SettingsController.Instance.isEnglish || page.HeaderTextTR == "" )
        {
            HeaderText.text = page.HeaderText;
        }
        else
        {
            HeaderText.text = page.HeaderTextTR;
        }
        lastOpenPage = pageIdentifier;
    }

    public void OnFoldButtonClicked() {
        isFolded = !isFolded;
        if (isFolded)
        {
            targetPos = new Vector3(foldedX, rt.anchoredPosition.y);
            targetIconRot = Quaternion.Euler(0, 0, foldedIconZRot); ;
        }
        else
        {
            targetPos = new Vector3(unfoldedX, rt.anchoredPosition.y);
            targetIconRot = Quaternion.Euler(0, 0, unfoldedIconZRot); ;
        }
        hasPosTarget = true;
        hasRotTarget = true;
    }

    public void OnGUI()
    {
        if (hasPosTarget)
        {
            if (Vector3.Distance(rt.anchoredPosition, targetPos) > 0.1f)
            {
                rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, targetPos, posLerpMultiplier);
            }
            else
            {
                rt.anchoredPosition = targetPos;
                hasPosTarget = false;
            }
        }

        if (hasRotTarget)
        {
            if (Vector3.Distance(foldIcon1.localRotation.eulerAngles, targetIconRot.eulerAngles) > 0.1f)
            {
                foldIcon1.localRotation = Quaternion.Lerp(foldIcon1.localRotation, targetIconRot, rotLerpMultiplier);
                foldIcon2.localRotation = Quaternion.Lerp(foldIcon2.localRotation, targetIconRot, rotLerpMultiplier);
            }
            else
            {
                foldIcon1.localRotation = targetIconRot;
                foldIcon2.localRotation = targetIconRot;
                hasRotTarget = false;
            }
        }
    }
}
