using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME.UI;

public class ThemePanel : HomePanel
{
    private List<ThemeLevelView> themeLvlList;

    protected override void OnShow(bool instant = false) {
        if (themeLvlList == null) themeLvlList = new List<ThemeLevelView>();
        base.OnShow(instant);
    }

    protected override void UpdateLevel() {
        int lvlCount = ThemeLevelDatabase.Instance.GetCount();
        int toSpawn = lvlCount - themeLvlList.Count;
        for (int i = 0; i < toSpawn; i++) {
            ThemeLevelView view = (ThemeLevelView)Instantiate(levelPrefab);
            view.transform.SetParent(levelScroll.content);
            view.transform.localScale = Vector3.one;
            themeLvlList.Add(view);
        }

        for (int i = 0; i < themeLvlList.Count; i++) {
            themeLvlList[i].SetModel(ThemeLevelDatabase.Instance.GetDataById(i));
            themeLvlList[i].Show();
        }
    }
}
