using UnityEngine;

[CreateAssetMenu()]
public class CuttingProcessSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax;
}
