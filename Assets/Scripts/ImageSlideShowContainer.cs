using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class ImageSlideShowContainer : MonoBehaviour
{
    public Image slideImage;
    private List<Sprite> images;
    private int currentIndex = 0;

    public float SetupSlideShow(List<Sprite> images)
    {
        this.images = images;

        StopAllCoroutines();
        if (this.images == null || this.images.Count == 0) {
            this.gameObject.SetActive(false);
            return 0f;
        }

        this.gameObject.SetActive(true);

        currentIndex = 0;
        slideImage.sprite = this.images[currentIndex];
        
        if (this.images.Count > 1)
        {
            StartCoroutine(SlideShow());
        }

        return 250f;
    }

    public IEnumerator SlideShow()
    {
        yield return new WaitForSeconds(2);
        currentIndex++;
        currentIndex = currentIndex % this.images.Count;
        slideImage.sprite = this.images[currentIndex];
        yield return SlideShow();

    }

}
