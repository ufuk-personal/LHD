using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingTarget : MonoBehaviour {

    public string objectID;

    private Color originalColorMaterial;
    private List<Color> originalColorMaterials;
    private Color currentColor;

    public void StartBlinking() {
        Renderer r = this.GetComponent<Renderer>();
        originalColorMaterial = r.material.color;

        originalColorMaterials = new List<Color>();
        foreach(Material m in r.materials)
        {
            originalColorMaterials.Add(m.color);
        }

        StartCoroutine(Blink());
    }

    float blinkProgress = 0f;

    public IEnumerator Blink() {

        yield return new WaitForEndOfFrame();
        blinkProgress += Time.deltaTime*2f;
        if (blinkProgress > 360f)
        {
            blinkProgress = blinkProgress % 360;
        }

        Renderer r = this.GetComponent<Renderer>();
        Color currentColor = r.material.color;
        currentColor.g = originalColorMaterial.g + Mathf.Abs(Mathf.Sin(blinkProgress)) * (1.0f - originalColorMaterial.g);
        currentColor.r = 0.4f + 0.6f * Mathf.Abs(Mathf.Sin(blinkProgress)) * (originalColorMaterial.r);
        currentColor.b = 0.4f + 0.6f * Mathf.Abs(Mathf.Sin(blinkProgress)) * (originalColorMaterial.b);

        r.material.SetColor("_Color", currentColor);

        for( int i = 0; i < r.materials.Length; i++)
        {
            currentColor = r.materials[i].color;
            currentColor.g = originalColorMaterials[i].g + Mathf.Abs(Mathf.Sin(blinkProgress)) * (1.0f - originalColorMaterials[i].g);
            currentColor.r = 0.4f + 0.6f * Mathf.Abs(Mathf.Sin(blinkProgress)) * (originalColorMaterials[i].r);
            currentColor.b = 0.4f + 0.6f * Mathf.Abs(Mathf.Sin(blinkProgress)) * (originalColorMaterials[i].b);

            r.materials[i].SetColor("_Color", currentColor);
        }

        yield return Blink();
    }

    public void EndBlinking() {
        StopAllCoroutines();

        Renderer r = this.GetComponent<Renderer>();
        Color currentColor = r.material.color;
        currentColor.r = originalColorMaterial.r;
        currentColor.g = originalColorMaterial.g;
        currentColor.b = originalColorMaterial.b;

        r.material.SetColor("_Color", currentColor);

        for (int i = 0; i < r.materials.Length; i++)
        {
            currentColor = r.materials[i].color;
            currentColor.r = originalColorMaterials[i].r;
            currentColor.g = originalColorMaterials[i].g;
            currentColor.b = originalColorMaterials[i].b;
            r.materials[i].SetColor("_Color", currentColor);
        }

        blinkProgress = 0;
    }
}
