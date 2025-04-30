// MetalInventory.cs
using System.Collections.Generic;
using UnityEngine;

public class MetalInventory : MonoBehaviour
{
    [Header("All Vial Definitions")]
    public List<MetalVialSO> allVials; 

    // runtime state: how many units you actually have
    private Dictionary<MetalType, float> metalUnits 
        = new Dictionary<MetalType, float>();

    void Awake()
    {
        // initialize counts to zero for each type
        foreach (var vial in allVials)
            metalUnits[vial.metal] = 0f;
    }

    /// <summary> Add one full vial of the specified metal. </summary>
    public void AddVial(MetalType metal)
    {
        var def = allVials.Find(v => v.metal == metal);
        if (def != null)
            metalUnits[metal] += def.startingUnits;
    }

    /// <summary> Try to burn `amount` units; returns how many you actually burned. </summary>
    public float Burn(MetalType metal, float amount)
    {
        float available = metalUnits[metal];
        float burned    = Mathf.Min(available, amount);
        metalUnits[metal] = available - burned;
        return burned;
    }

    /// <summary> How many units of `metal` you have remaining. </summary>
    public float GetUnits(MetalType metal)
        => metalUnits.TryGetValue(metal, out var u) ? u : 0f;
}