using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeLevelView : LevelView
{
    public override void Show() {
        base.Show();
        ThemeLevelData data = (ThemeLevelData)Model;
        SetTheme(data.BgSprite, data.StarSprite);
    }

    public void SetTheme(Sprite bgSprite, Sprite starSprite) {
        background.sprite = bgSprite;
        foreach (StarSlot star in starList) {
            star.ChangeSprite(starSprite);
        }
    }
}
