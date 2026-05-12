using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private Transform _iconTemplate;

    [SerializeField] private PlateKitchenObject _plateKitchenObject;

    private void Awake()
    {
        _iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        _plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

    }

    private void OnDestroy()
    {
        _plateKitchenObject.OnIngredientAdded -= PlateKitchenObject_OnIngredientAdded;
       

    }



    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        Debug.Log("PlateIconsUI: PlateKitchenObject_OnIngredientAdded");
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in transform)
        {
            if (child == _iconTemplate) continue;

            Destroy(child.gameObject);
        }

        if (_plateKitchenObject == null)
        {
            return;
        }

        foreach (KitchenObjectSO kitchenObjectSO in _plateKitchenObject.GetTacoIngredientsInPlateListSO())
        {
            Transform iconTransform = Instantiate(_iconTemplate, transform);

            iconTransform.gameObject.SetActive(true);

            iconTransform.GetComponent<PlateIconsSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
        }
    }
}