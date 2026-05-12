using UnityEngine;

[CreateAssetMenu()]
public class CookingProcessSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float cookingTimerMax;
}
