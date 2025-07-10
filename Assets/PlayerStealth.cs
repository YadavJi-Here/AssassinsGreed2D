using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
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
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeToAlpha(0f, 0.05f)); // fade out
            sr.sortingOrder = 0; // draw behind bush
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsBushLayer(other.gameObject))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeToAlpha(1f, 0.25f)); // fade in
            sr.sortingOrder = 2; // draw in front
        }
    }

    private bool IsBushLayer(GameObject obj)
    {
        return ((1 << obj.layer) & bushLayer) != 0;
    }

    private System.Collections.IEnumerator FadeToAlpha(float targetAlpha, float duration)
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