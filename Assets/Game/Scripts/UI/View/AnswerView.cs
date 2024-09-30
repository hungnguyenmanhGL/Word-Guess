using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME;
using HAVIGAME.UI;
using TMPro;
using DG.Tweening;
using System.Text;

public class AnswerView : View<AnswerData>
{
    [Header("[TMP]")]
    [SerializeField] private TextMeshProUGUI tmpFrontNum;
    [SerializeField] private TextMeshProUGUI tmpBackNum;
    [SerializeField] private TextMeshProUGUI tmpAnswer;

    [Header("[Refs]")]
    [SerializeField] private RectTransform rectTrans;
    [SerializeField] private Image imgHide;
    [SerializeField] private Image imgCircle;
    [SerializeField] private Button btn;

    private Tween revealTween;
    private bool isRevealed;

    public RectTransform RectTrans => rectTrans;
    public bool IsRevealed => isRevealed;

    private void Start() {
        btn.onClick.AddListener(ShowHint);
    }

    public override void Show() {
        StringBuilder strBuild = new StringBuilder();
        strBuild.Append(char.ToUpper(Model.Answer[0]));
        strBuild.Append(Model.Answer.Substring(1));
        tmpAnswer.text = strBuild.ToString();
    }

    public void Reveal(bool instant = false) {
        isRevealed = true;
        btn.enabled = false;
        if (instant) {
            imgHide.gameObject.SetActive(false);
            rectTrans.localEulerAngles = new Vector3(0, 180, 0);
            return;
        }
        revealTween = rectTrans.DOLocalRotate(new Vector3(0, 180, 0), 1f).OnUpdate(OnHalfReveal);
        SoundDatabase.Instance.CorrectAudio.Play();
    }

    public void OnHalfReveal() {
        if (revealTween.ElapsedPercentage() >= 0.35f && !revealTween.IsComplete()) {
            imgHide.gameObject.SetActive(false);
            revealTween.OnUpdate(null);
        }
    }

    public void Hide() {
        isRevealed = false;
        rectTrans.localEulerAngles = Vector3.zero;
        imgHide.gameObject.SetActive(true);
        btn.enabled = true;
    }

    private void ShowHint() {
        HintPanel hintPanel = UIManager.Instance.Push<HintPanel>();
        hintPanel.SetAnswer(Model.Answer);
    }

    #region Visuals
    public void SetColor(ColorData data) {
        imgHide.color = data.Base;
        tmpBackNum.color = data.Base;
        tmpAnswer.color = data.Base;
        imgCircle.color = data.Base;
    }

    public void SetNumber(int num) {
        tmpBackNum.text = num.ToString();
        tmpFrontNum.text = num.ToString();
    }
    #endregion
}
