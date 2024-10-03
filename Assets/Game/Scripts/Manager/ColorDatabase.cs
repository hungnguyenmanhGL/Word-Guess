using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HAVIGAME;

[CreateAssetMenu(fileName = "ColorDatabase", menuName = "Database/ColorDatabase")]
public class ColorDatabase : Database<ColorDatabase>
{
    [SerializeField] private List<ColorData> dataList;

    public List<ColorData> DataList => dataList;

    public ColorData GetColorDataById(int id) {
        foreach (ColorData data in dataList) {
            if (data.Id == id) return data;
        }
        return null;
    }
}

[System.Serializable]
public class ColorData {
    [SerializeField, ConstantField(typeof(ColorId))] private int id;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color bgColor;
    [SerializeField] private Color lightBgColor;

    public int Id => id;
    public Color Base => baseColor;
    public Color Bg => bgColor;
    public Color LightBg => lightBgColor;
}

public static class ColorId {
    public const int blue1 = 0;
    public const int blue2 = 1;
    public const int blue3 = 2;
    public const int green1 = 3;
    public const int green2 = 4;
    public const int green3 = 5;
    public const int green4 = 6;
    public const int green5 = 7;

    public static int MaxId => green5;
}
