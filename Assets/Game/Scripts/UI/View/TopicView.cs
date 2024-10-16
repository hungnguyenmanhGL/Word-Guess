using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME;
using HAVIGAME.UI;
using TMPro;
using HAVIGAME.Scenes;

public class TopicView : View<TopicData>
{
    [Header("[Refs]")]
    [SerializeField] private Image background;
    [SerializeField] private Button btn;
    [SerializeField] private Image imgStar;

    [Header("[Data]")]
    [SerializeField] private TextMeshProUGUI tmpTopic;
    [SerializeField] private TextMeshProUGUI tmpProgress;
    [SerializeField] private Image imgTopic;

    private int colorId;
    private bool isThemeTopic;

    private void Start() {
        btn.onClick.AddListener(PlayTopic);
    }

    public override void Show() {
        bool isImg = Model.IsImageTopic;
        tmpTopic.gameObject.SetActive(!Model.IsImageTopic);
        imgTopic.gameObject.SetActive(Model.IsImageTopic);

        tmpTopic.text = Model.Topic;
        imgTopic.sprite = Model.TopicImg;
        UpdateProgress();
    }

    public void UpdateProgress() {
        TopicSaveData save = GameData.Level.GetTopicSave(Model.Id, isThemeTopic);
        if (save == null) {
            tmpProgress.text = string.Format("0/{0}", Model.AnswerList.Count);
        } else tmpProgress.text = string.Format("{0}/{1}", save.Answered.Count, Model.AnswerList.Count);

        bool isDone = GameData.Level.IsTopicCompleted(Model.Id, isThemeTopic);
        imgStar.gameObject.SetActive(isDone);
        tmpProgress.gameObject.SetActive(!isDone);
    }

    private void PlayTopic() {
        GamePanel panel = UIManager.Instance.Push<GamePanel>();
        panel.SetTopic(Model, isThemeTopic);
        panel.SetColor(colorId);
        panel.Reset();
    }

    public void SetColor(ColorData data) {
        this.colorId = data.Id;
        background.color = data.Base;
    }

    public void SetThemeTopic(bool isTheme) {
        isThemeTopic = isTheme;
    } 
}
