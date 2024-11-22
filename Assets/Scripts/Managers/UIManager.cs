using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Gems")]
    [SerializeField] Transform gemCounterScaler;
    [SerializeField] Text gemCount;
    [SerializeField] ParticleSystem gemObtainParticle;
    [SerializeField] Transform gemIconParent;
    private void Awake()
    {
        GameManager.Instance.onGemCountChange += OnGemCountChange;
        GameManager.Instance.onGemEarn += OnGemEarn;
    }
    void OnGemCountChange()
    {
        gemCount.text = $"{GameManager.Instance.gems}/{GameManager.Instance.maxGems}";
        int i;
        for(i = 0; i < GameManager.Instance.gems; i++)
        {
            gemIconParent.GetChild(i).gameObject.SetActive(true);
        }
        for(;i < GameManager.Instance.maxGems; i++)
        {
            gemIconParent.GetChild(i).gameObject.SetActive(false);
        }
    }
    void OnGemEarn()
    {
        gemObtainParticle.Play();
    }
    private void Update()
    {
        gemCounterScaler.localScale = new Vector2(GameManager.Instance.gemGenerationCounter / GameManager.Instance.gemGenerationTime, 1.0f);
    }
}