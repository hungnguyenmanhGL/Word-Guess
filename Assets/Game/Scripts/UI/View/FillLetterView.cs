using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillLetterView : LetterView
{
    [SerializeField] private Image imgEmpty;

    private int usedLetterIndex;// match the index of the LetterView used to fill this
    private bool isFilled;

    public bool IsFilled => isFilled;
    public int UsedLetterIndex => usedLetterIndex;

    public void SetFilledLetter(char letter, int usedLetterIndex) {
        base.SetLetter(letter);
        isFilled = true;
        this.usedLetterIndex = usedLetterIndex;
        ToggleShow(true);
    }

    public override void ToggleShow(bool toShow) {
        base.ToggleShow(toShow);
        imgEmpty.gameObject.SetActive(!toShow);
    }

    protected override void OnClick() {
        panel.ClearSlot(this);
        usedLetterIndex = -1;
        isFilled = false;
    }

    public void SetCorrect() {
        btn.enabled = false;
        tmpLetter.color = Color.green;
    }

    public void Reset() {
        tmpLetter.color = Color.black;
        usedLetterIndex = -1;
        isFilled = false;
        ToggleShow(false);
    }
}
