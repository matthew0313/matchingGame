using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayerUnitSpawnButton : MonoBehaviour
{
    [SerializeField] Unit spawnedUnit;
    [SerializeField] int spawnCost;
    Button origin;
    private void Awake()
    {
        origin = GetComponent<Button>();
        origin.onClick.AddListener(TrySpawn);
    }
    private void Update()
    {
        if (GameManager.Instance.gems < spawnCost) origin.interactable = false;
        else origin.interactable = true;
    }
    void TrySpawn()
    {
        if(GameManager.Instance.gems >= spawnCost)
        {
            GameManager.Instance.gems -= spawnCost;
            GameManager.Instance.playerBase.SpawnUnit(spawnedUnit);
        }
    }
}
