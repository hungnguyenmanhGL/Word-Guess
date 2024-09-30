using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillLetterView : LetterView
{
    [SerializeField] private Image imgEmpty;

    private int slotIndex;// match the index of the LetterView used to fill this

    public override void ToggleShow(bool toShow) {
        base.ToggleShow(toShow);
        imgEmpty.gameObject.SetActive(!toShow);
    }

    protected override void OnClick() {
        base.OnClick();
        slotIndex = -1;
        panel.ClearSlot(this);
    }

    public void SetCorrect() {
        btn.enabled = false;
        tmpLetter.color = Color.green;
    }
}
