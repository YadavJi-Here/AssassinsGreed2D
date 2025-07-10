using UnityEngine;
using System.Collections;

public class PlayerStealth : MonoBehaviour
{
    public bool isHidden = false;

    private SpriteRenderer sr;
    private Coroutine fadeCoroutine;

    [SerializeField] private LayerMask bushLayer;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsBushLayer(other.gameObject))
        {
            SetHidden(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsBushLayer(other.gameObject))
        {
            SetHidden(false);
        }
    }

    public void SetHidden(bool hidden)
    {
        isHidden = hidden;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        if (hidden)
        {
            fadeCoroutine = StartCoroutine(FadeToAlpha(0f, 0.05f)); // fade out fast
            sr.sortingOrder = 0; // draw behind bush
        }
        else
        {
            fadeCoroutine = StartCoroutine(FadeToAlpha(1f, 0.25f)); // fade in slower
            sr.sortingOrder = 2; // draw in front
        }
    }

    private bool IsBushLayer(GameObject obj)
    {
        return ((1 << obj.layer) & bushLayer) != 0;
    }

    private IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        float startAlpha = sr.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, newAlpha);
            yield return null;
        }

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, targetAlpha);
    }
}