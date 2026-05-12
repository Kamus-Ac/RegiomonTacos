using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlatesBlockVisual : MonoBehaviour
{
    [SerializeField] private Transform _counterToPoint;
    [SerializeField] private Transform _platePrefab;
    [SerializeField] private PlatesBlock _platesBlock;

    private List<GameObject> _plateList;

    private void Awake()
    {
        _plateList = new List<GameObject>();
    }

    private void Start()
    {
        _platesBlock.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        _platesBlock.OnPlateRemoved += PlatesCounter_OnPlateRemoved;

    }


    private void PlatesCounter_OnPlateRemoved(object sender, System.EventArgs e)
    {
        GameObject plateGameObject = _plateList[_plateList.Count - 1];
        _plateList.Remove(plateGameObject);
        Destroy(plateGameObject);
    }

    private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e)
    {
        Transform plateTransform = Instantiate(_platePrefab, _counterToPoint);
        plateTransform.localPosition = new Vector3(0, 0.01f * _plateList.Count, 0);
        _plateList.Add(plateTransform.gameObject);

    }



}
