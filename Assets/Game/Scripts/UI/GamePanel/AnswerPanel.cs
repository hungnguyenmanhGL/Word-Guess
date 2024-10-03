using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerPanel : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private List<AnswerView> answerList;

    public RectTransform RectTrans => rectTransform;
    public List<AnswerView> AnswerList => answerList;

    private void Awake() {
        if (!rectTransform) rectTransform = (RectTransform)transform;
    }

    public void SetAnswerList(TopicData topicData) {
        for (int i=0; i<topicData.AnswerList.Count; i++) {
            answerList[i].SetModel(topicData.AnswerList[i]);
            answerList[i].SetNumber(i + 1);
            answerList[i].Show();
        }
    }

    public void RevealByIndex(int index, bool instant = false) {
        answerList[index].Reveal(instant);
    }

    public void RevealAll() {
        foreach (AnswerView view in answerList) {
            view.Reveal(true);
        }
    }

    public void HideAll() {
        foreach (AnswerView view in answerList) {
            view.Hide();
        }
    }
}
