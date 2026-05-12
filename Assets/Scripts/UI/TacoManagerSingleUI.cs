using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TacoManager;
public class TacoManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _recipeNameText;
    [SerializeField] private Transform _iconContainer;
    [SerializeField] private Transform _iconTemplate;
    [SerializeField] private TextMeshProUGUI _tableNumberText;


    private void Awake()
    {
        _iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {

    }


    public void SetOrder(CustomerOrder order)
    {
        _recipeNameText.text = order.tacoSO.recipeName;
        _tableNumberText.text = "Mesa " + order.customer.GetAssignedTable().TableNumber;

        foreach (Transform child in _iconContainer)
        {
            if (child == _iconTemplate) continue;

            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in order.tacoSO.kitchenObjectSOList)
        {
            Transform iconTransform = Instantiate(_iconTemplate, _iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }

       
    }
}
