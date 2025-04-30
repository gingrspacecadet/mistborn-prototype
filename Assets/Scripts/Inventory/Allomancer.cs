// Allomancer.cs
using UnityEngine;

[RequireComponent(typeof(MetalInventory))]
public class Allomancer : MonoBehaviour
{
    private MetalInventory inventory;

    [Header("Burning")]
    public MetalType activeMetal;
    private MetalVialSO   activeDef;
    public bool          isBurning;

    void Start()
    {
        inventory = GetComponent<MetalInventory>();
    }

    void Update()
    {
        // Toggle burn via keys (example: Alpha1…Alpha8)
        for (int i = 0; i < 8; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ToggleBurn((MetalType)i);
                break;
            }
        }

        // While burning, consume over time
        if (isBurning && activeDef != null)
        {
            float toBurn = activeDef.burnRate * Time.deltaTime;
            float got    = inventory.Burn(activeMetal, toBurn);
            if (got < toBurn * 0.99f)
            {
                // you ran out of that metal
                StopBurn();
                Debug.Log($"Ran out of {activeMetal} while burning!");
            }

            // TODO: apply metal’s effect here (push/pull, speed buff, etc.)
        }
    }

    void ToggleBurn(MetalType m)
    {
        if (isBurning && activeMetal == m)
        {
            StopBurn();
        }
        else
        {
            StartBurn(m);
        }
    }

    void StartBurn(MetalType m)
    {
        var def = inventory.allVials.Find(v => v.metal == m);
        if (def == null || inventory.GetUnits(m) <= 0f) return;

        activeMetal = m;
        activeDef   = def;
        isBurning   = true;
        Debug.Log($"Started burning {m}");
    }

    void StopBurn()
    {
        isBurning = false;
        Debug.Log($"Stopped burning {activeMetal}");
    }
}