using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeLevelView : LevelView
{

    public override void Show() {
        tmpLevel.text = (Model.Id + 1).ToString();
        for (int i = 0; i < 3; i++) {
            starList[i].Toggle(GameData.Level.IsTopicCompleted(Model.GetTopicByOrder(i).Id, true));
        }
        ThemeLevelData data = (ThemeLevelData)Model;
        SetTheme(data.BgSprite, data.StarSprite);
    }

    public void SetTheme(Sprite bgSprite, Sprite starSprite) {
        background.sprite = bgSprite;
        foreach (StarSlot star in starList) {
            star.ChangeSprite(starSprite);
        }
    }

    public override void SetColor(int colorId = 0) {
        this.colorId = colorId;
    }
}
