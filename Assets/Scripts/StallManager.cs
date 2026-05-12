using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static PlateKitchenObject;
using static UnityEditor.PlayerSettings;

public class StallManager : MonoBehaviour
{
    public static StallManager Instance;

    [Header("Clientes")]
    [SerializeField] private List<GameObject> customerPrefabs;
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public int maxCustomers = 3;
    public float spawnInterval = 5f;

    [Header("Mesas")]
    [SerializeField] private List<DeliveryBlock> _tables;
    List<Customer> currentCustomers;
    
    int currentCustomersCount = 0;



    void Awake()
    {
        Instance = this;

        int i = 1;
        foreach (DeliveryBlock table in _tables)
        {
            // Initialize each table if needed
            table.TableNumber = i;
            table.SetTableNumber(i);
            i++;
        }
        currentCustomers = new List<Customer>();

    }

    void Start()
    {
        InvokeRepeating(nameof(SpawnCustomer), 2f, spawnInterval);


    }

    public List<DeliveryBlock> GetTables()
    {
        return _tables;
    }

    public List<Customer> GetCurrentCustomers()
    {
        return currentCustomers;
    }


    private GameObject ChooseCustomer()
    {
        return customerPrefabs[UnityEngine.Random.Range(0, customerPrefabs.Count)];
    }

    void SpawnCustomer()
    {
        if (currentCustomers.Count >= maxCustomers)
            return;

        if(!GameManager.Instance.IsGamePlaying()) return;

        DeliveryBlock freeTable = GetFreeTable();

        if (freeTable == null)
            return;

        GameObject newCustomer = Instantiate(ChooseCustomer(), spawnPoint.position, Quaternion.identity);

        Customer npc = newCustomer.GetComponent<Customer>();
        npc.AssignTable(freeTable);
        freeTable.SetCustomer(npc);
        currentCustomers.Add(npc);
    }

    public void CustomerLeaves(Customer npc)
    {
        currentCustomers.Remove(npc);
    }

    DeliveryBlock GetFreeTable()
    {
        foreach (DeliveryBlock table in _tables)
        {
            if (!table.IsOccupied)
            {
                table.IsOccupied = true;
                return table;
            }
        }

        return null;
    }
}
