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
        Vector2 prev = logo.anchoredPosition;
        logoImage.DOColor(new Color(1, 1, 1, 1), 2.0f).OnComplete(() => logoImage.DOColor(new Color(1, 1, 1, 0), 3.0f));
        logoText.DOColor(new Color(1, 1, 1, 1), 2.0f).OnComplete(() => logoText.DOColor(new Color(1, 1, 1, 0), 3.0f));
        logo.DOAnchorPos(Vector2.zero, 2.0f).SetEase(Ease.OutElastic).OnComplete(() => {
            logo.DORewind();
            opening.DOScaleX(0, 2.0f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                startText.rectTransform.DOAnchorPosY(200, 0.5f).OnComplete(() => canStart = true);
            });
        });
    }
    private void Update()
    {
        if (canStart && InputManager.IsTouchDown()) SceneSwitcher.SwitchScene("Menu");
    }
}
