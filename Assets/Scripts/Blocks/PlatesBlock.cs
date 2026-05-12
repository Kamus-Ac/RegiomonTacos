using System;
using UnityEngine;

public class PlatesBlock : BaseBlock
{
    [SerializeField] private KitchenObjectSO _plateKitchenObjectSO;
    private float _spawnPlateTimer;
    private float _spawnPlateTimerMax = 4f;
    private int _platesSpawnAmount = 0;
    private int _plateSpawnAmountMax = 4;

    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    private void Update()
    {
        _spawnPlateTimer += Time.deltaTime;

        if (_spawnPlateTimer >= _spawnPlateTimerMax)
        {
            _spawnPlateTimer = 0;
            if (_platesSpawnAmount < _plateSpawnAmountMax)
            {
                _platesSpawnAmount++;
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }


    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //player is empty handed
            if (_platesSpawnAmount > 0)
            {
                //at least one plate is available
                KitchenObject.SpawnKitchenObject(_plateKitchenObjectSO, player);
                _platesSpawnAmount--;
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }


}
