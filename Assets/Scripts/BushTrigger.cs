using UnityEngine;

public class BushTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStealth stealth = other.GetComponent<PlayerStealth>();
            if (stealth != null)
            {
                stealth.SetHidden(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStealth stealth = other.GetComponent<PlayerStealth>();
            if (stealth != null)
            {
                stealth.SetHidden(false);
            }
        }
    }
}