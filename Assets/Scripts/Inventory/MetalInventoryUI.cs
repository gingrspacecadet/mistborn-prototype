using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class MetalInventoryUI : MonoBehaviour
{
    public MetalInventory inventory;
    public TMP_Text displayText;

    public Color defaultColor = Color.white;
    public Color burningColor = Color.cyan;

    private Dictionary<MetalType, string> metalIcons = new Dictionary<MetalType, string>
    {
        { MetalType.Iron,  "iron" },
        { MetalType.Steel, "steel" },
        { MetalType.Tin,   "tin" },
        { MetalType.Pewter,"pewter" },
        { MetalType.Zinc,  "zinc" },
        { MetalType.Brass, "brass" },
        { MetalType.Copper,"copper" },
        { MetalType.Bronze,"bronze" },
    };

    void Update()
    {
        if (inventory == null || displayText == null) return;

        var allomancer = inventory.GetComponent<Allomancer>();
        var sb = new StringBuilder();

        foreach (var vial in inventory.allVials)
        {
            float amount = inventory.GetUnits(vial.metal);
            bool isBurning = allomancer &&
                allomancer.burningStatus.TryGetValue(vial.metal, out bool burning) && burning;

            string icon = metalIcons.TryGetValue(vial.metal, out var name)
                ? $"<sprite name={name}> " : "";

            string color = isBurning
                ? ColorUtility.ToHtmlStringRGB(burningColor)
                : ColorUtility.ToHtmlStringRGB(defaultColor);

            sb.Append($"<color=#{color}>{icon}{vial.metal}: {amount:F1}</color>\u00A0\u00A0");
        }

        displayText.text = sb.ToString();
    }
}