using HAVIGAME.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HAVIGAME.Scenes;
using System.Collections.Generic;

public class HomePanel : UITab {
    [Header("[References]")]
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnPlay;

    [Header("[Levels]")]
    [SerializeField] private ScrollRect levelScroll;
    [SerializeField] private LevelView levelPrefab;

    private List<LevelView> lvlList;
    private int currentLevel;

    private void Start() {
        btnSetting.onClick.AddListener(OpenSettings);
    }

    protected override void OnShow(bool instant = false) {
        base.OnShow(instant);
        canvasGroup.interactable = true;
        if (lvlList == null) lvlList = new List<LevelView>();
        UpdateLevel();
    }

    protected override void OnBack() {
        OpenSettings();
    }

    protected override void OnResume() {
        base.OnResume();
        UpdateProgress();
    }

    private void PlayGame() {
        int levelToPlay = currentLevel;
        GameSceneController.pendingLoadLevelOption = LoadLevelOption.Create(levelToPlay);
        ScenesManager.Instance.LoadSceneAsyn(GameScene.ByIndex.Game);
    }

    private void UpdateLevel() {
        int lvlCount = LevelDatabase.Instance.GetCount();
        int toSpawn = lvlCount - lvlList.Count;
        for (int i=0; i<toSpawn; i++) {
            LevelView view = Instantiate(levelPrefab);
            view.transform.SetParent(levelScroll.content);
            view.transform.localScale = Vector3.one;
            lvlList.Add(view);
        }

        int colorCount = 0;
        for (int i=0; i<lvlList.Count; i++) {
            if (colorCount > ColorId.MaxId) colorCount = 0;
            lvlList[i].SetModel(LevelDatabase.Instance.GetDataById(i));
            lvlList[i].SetColor(colorCount);
            lvlList[i].Show();
            colorCount++;
        }
    }

    private void UpdateProgress() {
        foreach (LevelView view in lvlList)
            view.Show();
    }

    private void OpenSettings() {
        UIManager.Instance.Push<SettingsPanel>();
    }
}
