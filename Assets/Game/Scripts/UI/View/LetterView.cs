using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterView : MonoBehaviour
{
    [Header("[Refs]")]
    [SerializeField] protected Button btn;
    [SerializeField] protected TextMeshProUGUI tmpLetter;
    [SerializeField] protected Image background;
    [SerializeField] protected Image imgEmpty;

    protected char letter;
    protected HintPanel panel;

    public char Letter => letter;

    public HintPanel Panel {
        set => panel = value;
    }

    protected virtual void Start() {
        btn.onClick.AddListener(OnClick);
    }

    public void SetLetter(char letter) {
        char up = System.Char.ToUpper(letter);
        this.letter = letter;
        tmpLetter.text = up.ToString();
    }

    public virtual void ToggleShow(bool toShow) {
        if (background) background.enabled = toShow;
        tmpLetter.gameObject.SetActive(toShow);
        imgEmpty.gameObject.SetActive(!toShow);
        btn.enabled = toShow;
    }

    protected virtual void OnClick() {
        //call fill function in HintPanel here...
        panel.FillSlot(this);
    }
}
