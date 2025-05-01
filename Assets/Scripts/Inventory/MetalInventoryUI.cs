using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class MetalInventoryUI : MonoBehaviour
{
    public MetalInventory inventory;
    public TMP_Text displayText; // Assign in Inspector

    void Update()
    {
        if (inventory == null || displayText == null) return;

        StringBuilder sb = new StringBuilder("Metals:\n");
        foreach (var vial in inventory.allVials)
        {
            float amount = inventory.GetUnits(vial.metal);
            sb.AppendLine($"{vial.metal}: {amount:F1}");
        }

        if (inventory.TryGetComponent<Allomancer>(out var allomancer))
        {
            bool hasBurning = false;
            sb.AppendLine("\nBurning:");
            foreach (var kvp in allomancer.burningStatus)
            {
                if (kvp.Value)
                {
                    sb.AppendLine($"- {kvp.Key}");
                    hasBurning = true;
                }
            }

            if (!hasBurning)
                sb.AppendLine("- None");
        }

        displayText.text = sb.ToString();
    }
}