using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.Burst;

public class TitleUI : MonoBehaviour
{
    [SerializeField] RectTransform opening;
    [SerializeField] RectTransform logo;
    [SerializeField] Image logoImage;
    [SerializeField] Text logoText;
    [SerializeField] Text startText;

    bool canStart = false;
    private void Start()
    {
        float prev = logo.anchoredPosition.y;
        logoImage.DOColor(new Color(1, 1, 1, 1), 2.0f);
        logoText.DOColor(new Color(1, 1, 1, 1), 2.0f);
        logo.DOAnchorPosY(0.0f, 2.0f).SetEase(Ease.OutElastic).OnComplete(() => {
            logoImage.DOColor(new Color(1, 1, 1, 0), 2.0f);
            logoText.DOColor(new Color(1, 1, 1, 0), 2.0f);
            opening.DOScaleX(0, 2.0f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                startText.rectTransform.DOAnchorPosY(200, 0.5f).OnComplete(() => canStart = true);
            });
        });
    }
    private void Update()
    {
        if (canStart && InputManager.IsTouchDown())
        {
            if (GlobalManager.Instance.save.introWatched) SceneSwitcher.SwitchScene("Menu");
            else SceneSwitcher.SwitchScene("Intro");
        }
    }
}
