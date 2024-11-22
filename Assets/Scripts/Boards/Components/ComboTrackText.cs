using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboTrackText : MonoBehaviour
{
    [SerializeField] Board trackingBoard;

    [SerializeField] Animator anim;
    [SerializeField] Text comboText;

    private void Awake()
    {
        trackingBoard.onTilePop += ComboUpdate;
    }
    int comboID = Animator.StringToHash("Combo");
    void ComboUpdate()
    {
        comboText.text = trackingBoard.combo.ToString();
        comboText.transform.localScale = new Vector2(Mathf.Min(100, trackingBoard.combo) * 0.01f + 1, Mathf.Min(100, trackingBoard.combo) * 0.01f + 1);
        anim.SetTrigger(comboID);
    }
}