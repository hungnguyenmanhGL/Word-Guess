using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HAVIGAME;

[CreateAssetMenu(fileName = "ThemeLevelData", menuName = "Data/ThemeLevelData")]
public class ThemeLevelData : LevelData
{
    [SerializeField, SpriteField] private Sprite starSprite;
    [SerializeField, SpriteField] private Sprite bgSprite;

    [SerializeField, ConstantField(typeof(ColorId))] private int colorId;

    public Sprite StarSprite => starSprite;
    public Sprite BgSprite => bgSprite;
    public int ColorId => colorId;
}
