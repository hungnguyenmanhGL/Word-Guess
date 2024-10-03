using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME;
using HAVIGAME.UI;
using DG.Tweening;
using TMPro;

public class HintPanel : UIFrame
{
    [Header("[Topic]")]
    [SerializeField] private TextMeshProUGUI tmpTopic;
    [SerializeField] private Image imgTopic;

    [Header("[Refs]")]
    [SerializeField] private GridLayoutGroup wordFillGrid;
    [SerializeField] private GridLayoutGroup letterGrid;
    [SerializeField] private FillLetterView fillPrefab;
    [SerializeField] private LetterView letterPrefab;

    [SerializeField] private List<FillLetterView> slotList;
    [SerializeField] private List<LetterView> availableList;
    private HashSet<int> usedIndex;

    private Tween wrongTween;

    private int currentIndex;
    private string answer;
    private bool isAnswered;

    private void Awake() {
        if (slotList == null) slotList = new List<FillLetterView>();
        if (availableList == null) availableList = new List<LetterView>();
        if (usedIndex == null) usedIndex = new HashSet<int>();
    }

    protected override void OnShow(bool instant = false) {
        base.OnShow(instant);
        SpawnAvailableLetterViews();
    }

    protected override void OnHideCompleted() {
        base.OnHideCompleted();
        if (isAnswered) {
            GamePanel gamePanel = (GamePanel)UIManager.Instance.Peek();
            gamePanel.OnAnswerByHint();
            isAnswered = false;
        }
    }

    #region On Complete
    private void OnFill() {
        //move to the smallest empty index to keep filling
        currentIndex = GetSmallestEmptyIndex();
        //if no empty index found -> word completed -> compare all slots to answer 
        if (currentIndex > slotList.Count) {
            CheckAnswer();
        }
    }

    private void CheckAnswer() {
        for (int i=0; i<slotList.Count; i++) {
            if (slotList[i].Letter != answer[i]) {
                //wrong tween -> break
                wrongTween?.Kill();
                wrongTween = wordFillGrid.transform.DOShakePosition(0.25f, 100);
                return;
            }
        }
        //if no wrong letter -> correct
        OnCorrectFill();
    }

    private void OnCorrectFill() {
        Debug.LogError(answer);
        isAnswered = true;
        Hide();
    }

    #endregion

    #region OnShow Set
    private void SpawnAvailableLetterViews() {
        if (availableList.Count > 0) return;
        for (int i = 0; i < 20; i++) {
            LetterView view = letterPrefab.Spawn();
            view.transform.SetParent(letterGrid.transform);
            view.transform.localScale = Vector3.one;
            availableList.Add(view);
        }
    }

    public void SetAnswer(string word) {
        if (word.Equals(answer)) return;
        answer = word;
        currentIndex = 1;

        //Randomize positions for every letter of the answer
        usedIndex.Clear();
        for (int i = 1; i < answer.Length; i++) {//start from 1 because answer[0] is always shown
            if (answer[i] == ' ') continue;
            int index = Random.Range(0, availableList.Count);
            while (usedIndex.Contains(index)) index = Random.Range(0, availableList.Count);
            availableList[index].SetLetter(answer[i]);
            availableList[index].ToggleShow(true);
            usedIndex.Add(index);
        }
        //Randomize letter for unused indexes
        for (int i = 0; i < availableList.Count; i++) {
            availableList[i].Panel = this;
            if (usedIndex.Contains(i)) continue;
            availableList[i].SetLetter((char)Random.Range(97, 122));
            availableList[i].ToggleShow(true);
        }

        //clear previous fill slot to reset
        foreach (FillLetterView view in slotList) view.Recycle();
        slotList.Clear();

        for (int i=0; i<answer.Length; i++) {
            FillLetterView view = fillPrefab.Spawn();
            view.transform.SetParent(wordFillGrid.transform);
            view.transform.localScale = Vector3.one;
            view.Panel = this;
            slotList.Add(view);
            //always show the first letter of the answer
            if (i == 0 || answer[i] == ' ') {
                view.SetFilledLetter(answer[i], -1);
                view.SetCorrect();
            }//but hide other slots 
            else view.Reset();
        }
        currentIndex = 1;
    }

    public void SetTopic(TopicData data) {
        imgTopic.gameObject.SetActive(data.IsImageTopic);
        imgTopic.sprite = data.TopicImg;
        tmpTopic.gameObject.SetActive(!data.IsImageTopic);
        tmpTopic.text = data.Topic;
    }
    #endregion

    #region Fill - Undo
    public void FillSlot(LetterView selected) {
        if (currentIndex > slotList.Count) return;

        selected.ToggleShow(false);
        //fill the selected letter to the current index to fill (when first opened, index = 1)
        FillLetterView fillView = slotList[currentIndex];
        fillView.SetFilledLetter(selected.Letter, availableList.IndexOf(selected));
        int lastIndex = currentIndex;
        //hide the selected letter view
        selected.ToggleShow(false);
        //check if the word is complete and if it is right/wrong
        OnFill();
        Log.Info(string.Format("Used {0} for {1}, next fill {2}", availableList.IndexOf(selected), lastIndex, currentIndex));
    }

    public void ClearSlot(FillLetterView selected) {
        selected.ToggleShow(false);
        LetterView usedView = availableList[selected.UsedLetterIndex];
        usedView.ToggleShow(true);
        selected.ToggleShow(false);

        int emptyIndex = slotList.IndexOf(selected);
        if (emptyIndex < currentIndex) {
            currentIndex = emptyIndex;
        }
        Log.Info(string.Format("Clear {0} for {1}, next fill {2}", selected.UsedLetterIndex, emptyIndex, currentIndex));
    }

    private int GetSmallestEmptyIndex() {
        for (int i = 0; i < slotList.Count; i++) {
            if (!slotList[i].IsFilled) {
                return i;
            }
        }
        return slotList.Count + 1;
    }
    #endregion
}
