using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MetalInventory))]
public class Allomancer : MonoBehaviour
{
    [Header("Burning")]
    public Dictionary<MetalType, bool> burningStatus = new();
    private Dictionary<MetalType, MetalVialSO> metalDefs = new();

    private MetalInventory inventory;

    void Start()
    {
        inventory = GetComponent<MetalInventory>();

        foreach (var vial in inventory.allVials)
        {
            burningStatus[vial.metal] = false;
            metalDefs[vial.metal] = vial;
        }
    }

    void Update()
    {
        for (int i = 0; i < 8; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ToggleBurn((MetalType)i);
            }
        }

        foreach (var kvp in burningStatus)
        {
            if (kvp.Value)
            {
                var metal = kvp.Key;
                if (!metalDefs.TryGetValue(metal, out var def)) continue;

                float toBurn = def.burnRate * Time.deltaTime;
                float got = inventory.Burn(metal, toBurn);

                if (got < toBurn * 0.99f)
                {
                    Debug.Log($"Ran out of {metal} while burning.");
                    burningStatus[metal] = false;
                }
            }
        }
    }

    public void ToggleBurn(MetalType metal)
    {
        if (!burningStatus.ContainsKey(metal))
        {
            Debug.LogWarning($"Metal {metal} not in inventory.");
            return;
        }

        bool currentlyBurning = burningStatus[metal];

        if (currentlyBurning)
        {
            burningStatus[metal] = false;
            Debug.Log($"Stopped burning {metal}");
        }
        else
        {
            if (inventory.GetUnits(metal) <= 0f)
            {
                Debug.Log($"Can't burn {metal}, none left.");
                return;
            }

            burningStatus[metal] = true;
            Debug.Log($"Started burning {metal}");
        }
    }

    public bool IsBurning(MetalType metal)
    {
        return burningStatus.TryGetValue(metal, out bool value) && value;
    }
}