using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME;
using HAVIGAME.UI;
using TMPro;

public class WinPanel : UIFrame
{
    [Header("[Topics]")]
    [SerializeField] private Image topicBg;
    [SerializeField] private TextMeshProUGUI tmpTopic;
    [SerializeField] private Image imgTopic;

    [Header("[Refs]")]
    [SerializeField] private Image background;
    [SerializeField] private GameButton btnBack;
    [SerializeField] private RectTransform emojiRect;
    [SerializeField] private List<Animator> animPrefabs;

    private Animator emoji;

    private void Start() {
        btnBack.onClick.AddListener(OnBackClick);
    }

    protected override void OnHideCompleted() {
        base.OnHideCompleted();
        emoji.Recycle();
        emoji = null;
    }

    public void SetTopic(TopicData data) {
        tmpTopic.text = data.Topic;
        imgTopic.sprite = data.TopicImg;
        tmpTopic.gameObject.SetActive(!data.IsImageTopic);
        imgTopic.gameObject.SetActive(data.IsImageTopic);
        SetRandomEmoji();
    }

    public void SetColor(ColorData colorData) {
        topicBg.color = colorData.Base;
        background.color = colorData.LightBg;
        btnBack.SetColor(colorData.Base);
    }

    private void SetRandomEmoji() {
        int index = Random.Range(0, animPrefabs.Count);
        emoji = animPrefabs[index].Spawn();
        RectTransform rect = emoji.GetComponent<RectTransform>();
        rect.SetParent(emojiRect);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one * 0.4f;
    }

    private void OnBackClick() {
        //UIManager.Instance.Push<SelectTopicPanel>();
        Hide();
    }
}
