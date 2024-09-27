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
}
