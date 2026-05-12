using TMPro;
using UnityEngine;
using static TacoManager;

public class TacoManagerUI : MonoBehaviour
{

    [SerializeField] private Transform _container;
    [SerializeField] private Transform _tacoTemplate;



    private void Awake()
    {
        _tacoTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        TacoManager.Instance.OnOrdersChanged += TacoManager_OnOrdersChanged;


        UpdateVisual();
    }

    private void OnDestroy()
    {
        TacoManager.Instance.OnOrdersChanged -= TacoManager_OnOrdersChanged;
    }

    private void TacoManager_OnOrdersChanged(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in _container)
        {
            if (child == _tacoTemplate)
            {
                continue;
            }
            Destroy(child.gameObject);
        }



        foreach (CustomerOrder customerOrder in TacoManager.Instance.GetGeneratedTacoOrders())
        {
            Transform recipeTransform = Instantiate(_tacoTemplate, _container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<TacoManagerSingleUI>().SetOrder(customerOrder);
        }
    }


}
