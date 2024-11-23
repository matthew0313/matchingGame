using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    [SerializeField] Button prevButton, nextButton, playButton;
    [SerializeField] Image[] stageAnchors;
    [SerializeField] Color completedColor, availableColor, notAvailableColor;
    int selectedStage = 1;

    Vector3 offSet = new Vector3(0, 0, -10.0f);
    private void Awake()
    {
        selectedStage = Mathf.Min(GlobalManager.Instance.save.lastCompleted + 1, stageAnchors.Length - 1);
        Camera.main.transform.position = stageAnchors[selectedStage].transform.position + offSet;
        for(int i = 0; i < stageAnchors.Length; i++)
        {
            if (GlobalManager.Instance.save.stageSaves[i].completed) stageAnchors[i].color = completedColor;
            else if (i == 0 || GlobalManager.Instance.save.stageSaves[i - 1].completed) stageAnchors[i].color = availableColor;
            else stageAnchors[i].color = notAvailableColor;
        }
        UpdateButtons();
    }
    bool moving = false;
    void UpdateButtons()
    {
        if (selectedStage <= 0) prevButton.interactable = false;
        else prevButton.interactable = true;

        if (selectedStage >= stageAnchors.Length - 1) nextButton.interactable = false;
        else nextButton.interactable = true;

        if (selectedStage > 0 && GlobalManager.Instance.save.stageSaves[selectedStage - 1].completed == false) playButton.interactable = false;
        else playButton.interactable = true;
    }
    public void SelectNext()
    {
        if (moving || selectedStage >= stageAnchors.Length - 1) return;
        selectedStage++;
        UpdateButtons();
        Camera.main.transform.DOMove(stageAnchors[selectedStage].transform.position + offSet, 0.5f).SetEase(Ease.OutCirc).OnComplete(() =>
        {

        });
    }
    public void SelectPrev()
    {
        if (moving || selectedStage <= 0) return;
        selectedStage--;
        UpdateButtons();
        Camera.main.transform.DOMove(stageAnchors[selectedStage].transform.position + offSet, 0.5f).SetEase(Ease.OutCirc).OnComplete(() =>
        {

        });
    }
    public void PlayStage()
    {
        if (selectedStage > 0 && GlobalManager.Instance.save.stageSaves[selectedStage - 1].completed == false) return;
        SceneSwitcher.SwitchScene("Stage" + selectedStage);
    }
}
