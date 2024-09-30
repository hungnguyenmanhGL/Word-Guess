using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME;
using HAVIGAME.UI;
using TMPro;

public class HintPanel : UIFrame
{
    [Header("[Refs]")]
    [SerializeField] private GridLayoutGroup wordFillGrid;
    [SerializeField] private GridLayoutGroup letterGrid;
    [SerializeField] private FillLetterView fillPrefab;
    [SerializeField] private LetterView letterPrefab;

    [SerializeField] private List<FillLetterView> slotList;
    [SerializeField] private List<LetterView> availableList;
    private HashSet<int> usedIndex;

    private int currentIndex;
    private string answer;

    private void Awake() {
        if (slotList == null) slotList = new List<FillLetterView>();
        if (availableList == null) availableList = new List<LetterView>();
        if (usedIndex == null) usedIndex = new HashSet<int>();
    }

    protected override void OnShow(bool instant = false) {
        base.OnShow(instant);
        SpawnAvailableLetters();
    }

    #region OnShow Set
    private void SpawnAvailableLetters() {
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

        //Randomize positions for every letter of the answer
        usedIndex.Clear();
        for (int i = 1; i < answer.Length; i++) {//start from 1 because answer[0] is always shown
            int index = Random.Range(0, availableList.Count);
            while (usedIndex.Contains(index)) index = Random.Range(0, availableList.Count);
            availableList[index].SetLetter(answer[i]);
            availableList[index].ToggleShow(true);
            usedIndex.Add(index);
        }
        //Randomize letter for unused indexes
        for (int i = 0; i < availableList.Count; i++) {
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
            slotList.Add(view);
            //always show the first letter of the answer
            if (i == 0) {
                view.SetLetter(answer[0]);
                view.ToggleShow(true);
                view.SetCorrect();
            }//but hide other slots 
            else view.ToggleShow(false);
        }
        currentIndex = 1;
    }
    #endregion

    #region Fill - Undo
    public void FillSlot(LetterView selected) {

    }

    public void ClearSlot(FillLetterView selected) {

    }
    #endregion
}
