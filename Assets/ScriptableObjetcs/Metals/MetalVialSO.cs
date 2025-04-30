// MetalVialSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewMetalVial", menuName = "Mistborn/Metal Vial")]
public class MetalVialSO : ScriptableObject
{
    public MetalType metal;
    public Sprite    icon;
    [Tooltip("Units of metal in this vial.")]
    public int       startingUnits = 20;
    [Tooltip("Units of metal burned per second when active.")]
    public float     burnRate = 1f;
}