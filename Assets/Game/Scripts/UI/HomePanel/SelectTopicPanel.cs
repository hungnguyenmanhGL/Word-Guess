using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME;
using HAVIGAME.UI;
using TMPro;

public class SelectTopicPanel : UIFrame
{
    [Header("[Refs]")]
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI tmpLvl;
    [SerializeField] private List<TopicView> topicList;

    protected override void OnResume() {
        base.OnResume();
        UpdateTopicProgress();
    }

    public void SetTopicList(LevelData data) {
        tmpLvl.text = string.Format("LEVEL \n {0}", data.Id + 1);
        for (int i = 0; i < data.TopicList.Count; i++) {
            topicList[i].SetModel(data.TopicList[i]);
            topicList[i].Show();
        }
    }

    private void UpdateTopicProgress() {
        foreach (TopicView view in topicList) {
            view.UpdateProgress();
        }
    }

    public void SetBGColor(Color color) {
        background.color = color;
    }
}
