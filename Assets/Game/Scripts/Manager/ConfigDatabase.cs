using UnityEngine;
using HAVIGAME;
using HAVIGAME.Audios;

[CreateAssetMenu(fileName = "ConfigDatabase", menuName = "Database/ConfigDatabase")]
public class ConfigDatabase : Database<ConfigDatabase> {
    [SerializeField] private Audio defaultButtonPressAudio;
    [SerializeField] private int maxLevel;
    [SerializeField] private ItemStack[] defaultInventory;
    [SerializeField] private string defaultValueFormat = "#,##0";

    [Header("[Gameplay]")]
    [SerializeField] private int grammarCheckLength = 6;
    [SerializeField] private int maxMistakeCount = 2;
    [SerializeField] private bool removeExtraInHint;

    public Audio DefaultButtonPressAudio => defaultButtonPressAudio;

    public int MaxLevel => maxLevel;

    public ItemStack[] DefaultInventory => defaultInventory;

    public string DefaultValueFormat => defaultValueFormat;

    public int GrammarCheckLength => grammarCheckLength;
    public int MaxMistakeCount => maxMistakeCount;
    public bool RemoveExtraInHint => removeExtraInHint;
}
