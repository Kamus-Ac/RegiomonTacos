using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeliveryBlock : BaseBlock
{
    [SerializeField] List<Transform> _seatsTransform;
    

    public int TableNumber { get; set; }

    public bool IsOccupied { get; set; } = false;
    [SerializeField] private TextMeshProUGUI _tableNumberUI;

    private TacoSO _deliveryTacoSO;
    private Customer _customer;

    private void Start()
    {
        
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //only accepts plates
                TacoManager.Instance.DeliverOrder(plateKitchenObject, this.GetCustomer()); 
                player.GetKitchenObject().DestroySelf();
            }

        }
    }




    public Transform ChooseSeat()
    {
        List<Transform> availableSeats = new List<Transform>();
        foreach (Transform seat in _seatsTransform)
        {
            availableSeats.Add(seat);
        }

        return availableSeats[UnityEngine.Random.Range(0, availableSeats.Count)];
    }

    public void SetTableNumber(int tableNumber)
    {
        _tableNumberUI.text = tableNumber.ToString();
    }

    public void SetTacoSO(TacoSO customerOrder)
    {
        _deliveryTacoSO = customerOrder;

    }

    public TacoSO GetTacoSO()
    {
        return _deliveryTacoSO;
    }


    public Customer GetCustomer()
    {
        return _customer;
    }

    public void SetCustomer(Customer customer)
    {
        _customer = customer;
    }

}
