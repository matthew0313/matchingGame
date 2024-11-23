using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayerUnitSpawnButton : MonoBehaviour
{
    [SerializeField] Unit spawnedUnit;
    [SerializeField] int spawnCost;
    [SerializeField] float spawnCooldown;
    [SerializeField] Text cooldownText;
    [SerializeField] Image cooldownImage;
    float counter = 0.0f;
    Button origin;
    private void Awake()
    {
        origin = GetComponent<Button>();
        origin.onClick.AddListener(TrySpawn);
    }
    private void Update()
    {
        if(counter > 0.0f)
        {
            counter = Mathf.Max(0, counter - Time.deltaTime);
            if(counter <= 0.0f)
            {
                cooldownText.gameObject.SetActive(false);
                cooldownImage.gameObject.SetActive(false);
            }
            else
            {
                cooldownText.text = (Mathf.Floor(counter * 10.0f) / 10.0f).ToString();
                cooldownImage.fillAmount = counter / spawnCooldown;
            }
        }
    }
    void TrySpawn()
    {
        if(GameManager.Instance.gems >= spawnCost && counter <= 0.0f)
        {
            counter = spawnCooldown;
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = (Mathf.Floor(counter * 10.0f) / 10.0f).ToString();
            cooldownImage.gameObject.SetActive(true);
            cooldownImage.fillAmount = counter / spawnCooldown;
            GameManager.Instance.gems -= spawnCost;
            GameManager.Instance.playerBase.SpawnUnit(spawnedUnit);
        }
    }
}
