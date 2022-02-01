using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeImage : MonoBehaviour
{
    [SerializeField]
    private Image image;

    [SerializeField]
    private float displayTime;

    [SerializeField]
    private float hideTime;

    public void Display(float duration)
    {
        StartCoroutine(DisplayCoroutine(duration));
    }

    public void Hide(float duration)
    {
        StartCoroutine(HideCoroutine(duration));
    }

    public void DisplayAndHide (float duration)
    {
        StartCoroutine(DisplayAndHideCoroutine(duration));
    }

    IEnumerator DisplayAndHideCoroutine(float duration)
    {
        duration *= 0.5f;
        yield return DisplayCoroutine(duration);
        yield return HideCoroutine(duration);
    }

    IEnumerator DisplayCoroutine (float duration)
    {
        image.enabled = true;
        float t = 0;

        while (t < duration + 0.05f)
        {
            image.color = SetAlpha(t / duration, image.color);
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator HideCoroutine(float duration)
    {
        image.enabled = true;
        float t = duration;

        while (t > -0.05f)
        {
            image.color = SetAlpha(t / duration, image.color);
            t -= Time.deltaTime;
            yield return null;
        }

        image.enabled = false;
    }

    private Color SetAlpha(float percentage, Color color)
    {
        color.a = percentage;
        return color;
    }
}
