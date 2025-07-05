using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    private SpriteRenderer sr;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bush"))
        {
            // Cancel previous fade if running
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeToAlpha(0f, 0.25f)); // Fade out
            sr.sortingOrder = 0; // Behind bush
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bush"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeToAlpha(1f, 0.25f)); // Fade in
            sr.sortingOrder = 2; // In front of bush
        }
    }

    private System.Collections.IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        float startAlpha = sr.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            sr.color = new Color(1f, 1f, 1f, newAlpha);
            yield return null;
        }

        sr.color = new Color(1f, 1f, 1f, targetAlpha);
    }
}