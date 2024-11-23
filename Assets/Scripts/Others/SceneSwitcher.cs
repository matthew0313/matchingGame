using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] RectTransform back;
    [SerializeField] GameObject[] tips;

    static SceneSwitcher instance;
    bool switching = false;
    public static void SwitchScene(string sceneName, bool tip = false)
    {
        if (instance == null) instance = Resources.Load<SceneSwitcher>("SceneSwitcher");
        if (instance.switching) return;
        instance.switching = true;
        instance.back.pivot = new Vector2(1.0f, 0.5f);
        instance.back.DOScaleX(1, 0.5f).SetEase(Ease.InCirc).OnComplete(() =>
        {
            Action next = () =>
            {
                SceneManager.LoadScene(sceneName);
                instance.switching = false;
                instance.back.pivot = new Vector2(0.0f, 0.0f);
                instance.back.DOScaleX(0, 0.5f).SetEase(Ease.InCirc);
            };
            if (tip) instance.StartCoroutine(instance.Tipping(next));
            else next.Invoke();
        });
    }
    IEnumerator Tipping(Action onTap)
    {
        int tipIndex = UnityEngine.Random.Range(0, tips.Length);
        tips[tipIndex].SetActive(true);
        while (!InputManager.IsTouchDown()) yield return null;
        tips[tipIndex].SetActive(false);
    }
}