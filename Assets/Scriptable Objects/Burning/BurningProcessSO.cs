using UnityEngine;

[CreateAssetMenu()]
public class BurningProcessSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burningTimerMax;
}
