using System.Reflection;
using UnityEngine;

public class MetalPickup : MonoBehaviour
{
    public MetalType metalType;       // Set in inspector (e.g., Iron)
    public AudioClip pickupSound;     // optional

    void OnTriggerEnter(Collider other)
    {
        var inv = other.GetComponent<MetalInventory>();
        if (inv != null)
        {
            inv.AddVial(metalType);
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            Destroy(gameObject);
        }
    }
}