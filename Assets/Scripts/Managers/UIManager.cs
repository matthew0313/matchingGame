using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("Gems")]
    [SerializeField] Transform gemCounterScaler;
    [SerializeField] Text gemCount;
    [SerializeField] ParticleSystem gemObtainParticle;
    [SerializeField] Transform gemIconParent;

    [Header("GameEnd")]
    [SerializeField] GameObject endScene;
    [SerializeField] RectTransform backBlack;
    [SerializeField] RectTransform victoryText, defeatText;
    private void Awake()
    {
        GameManager.Instance.onGemCountChange += OnGemCountChange;
        GameManager.Instance.onGemEarn += OnGemEarn;
        GameManager.Instance.onGameDefeat += OnGameDefeat;
        GameManager.Instance.onGameWin += OnGameWin;
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
    void OnGameDefeat()
    {
        endScene.SetActive(true);
        backBlack.DOScaleY(1.0f, 0.5f).SetEase(Ease.InCirc).OnComplete(() =>
        {
            defeatText.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutElastic);
        });
    }
    void OnGameWin()
    {
        endScene.SetActive(true);
        backBlack.DOScaleY(1.0f, 0.5f).SetEase(Ease.InCirc).OnComplete(() =>
        {
            victoryText.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutElastic);
        });
    }
    void Wait(float time, Action afterWait) => StartCoroutine(Waiting(time, afterWait));
    IEnumerator Waiting(float time, Action afterWait)
    {
        yield return new WaitForSeconds(time);
        afterWait?.Invoke();
    }
}