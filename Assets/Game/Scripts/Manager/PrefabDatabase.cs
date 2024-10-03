using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME;

[CreateAssetMenu(fileName = "PrefabDatabase", menuName = "Database/PrefabDatabase")]
public class PrefabDatabase : Database<PrefabDatabase>
{
    [Header("[AnswerPanels]")]
    [SerializeField] private List<AnswerPanel> x4List;
    [SerializeField] private List<AnswerPanel> x5List;
    [SerializeField] private List<AnswerPanel> x6List;
    [SerializeField] private List<AnswerPanel> x7List;
    [SerializeField] private List<AnswerPanel> x8List;
    [SerializeField] private List<AnswerPanel> x9List;
    [SerializeField] private List<AnswerPanel> x10List;

    public AnswerPanel GetAnswerPanel(int numOfAns) {
        switch (numOfAns) {
            case 4:
                return x4List[Random.Range(0, x4List.Count)];
            case 5:
                return x5List[Random.Range(0, x5List.Count)];
            case 6:
                return x6List[Random.Range(0, x6List.Count)];
            case 7:
                return x7List[Random.Range(0, x7List.Count)];
            case 8:
                return x8List[Random.Range(0, x8List.Count)];
            case 9:
                return x9List[Random.Range(0, x9List.Count)];
            case 10:
                return x10List[Random.Range(0, x10List.Count)];
        }
        return null;
    }
}
