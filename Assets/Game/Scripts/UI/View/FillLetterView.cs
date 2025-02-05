using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FillLetterView : LetterView
{
    private static float revealFadeTime = 1f;

    private int usedLetterIndex;// match the index of the LetterView used to fill this
    private bool isFilled;
    private Tween revealTween;

    public bool IsFilled => isFilled;
    public int UsedLetterIndex => usedLetterIndex;

    public void SetFilledLetter(char letter, int usedLetterIndex) {
        base.SetLetter(letter);
        isFilled = true;
        this.usedLetterIndex = usedLetterIndex;
        ToggleShow(true);
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

    #region Reveal Hint
    public void RevealCorrectLetter(char letter)
    {
        base.SetLetter(letter);
        tmpLetter.gameObject.SetActive(true);
        SetCorrect();
        revealTween = DOTween.Sequence().Append(imgEmpty.DOFade(0, revealFadeTime))
                                        .Append(imgEmpty.DOFade(1, revealFadeTime))
                                        .SetLoops(-1, LoopType.Yoyo);
    }

    public void KillReveal()
    {
        revealTween?.Kill();
        imgEmpty.color = new Color(imgEmpty.color.r, imgEmpty.color.g, imgEmpty.color.b, 1);
        Reset();
    }
    #endregion
}
