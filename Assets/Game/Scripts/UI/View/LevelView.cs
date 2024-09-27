using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME;
using HAVIGAME.UI;
using TMPro;

public class LevelView : View<LevelData>
{
    [SerializeField] private Button btnLevel;
    [SerializeField] private Image background;
    [Header("[Visuals]")]
    [SerializeField] private TextMeshProUGUI tmpLevel;
    [SerializeField] private List<StarSlot> starList;

    private int colorId;

    private void Start() {
        btnLevel.onClick.AddListener(OpenLevel);    
    }

    public override void Show() {
        tmpLevel.text = Model.Id.ToString();
        for (int i=0; i<3; i++) {
            starList[i].Toggle(GameData.Level.IsTopicCompleted(Model.GetTopicByOrder(i).Id));
        }
    }

    public void OpenLevel() {
        SelectTopicPanel selectPanel = UIManager.Instance.Push<SelectTopicPanel>();
        selectPanel.SetTopicList(Model);
        selectPanel.SetBGColor(ColorDatabase.Instance.GetColorDataById(colorId).Bg);
    }

    public void SetColor(int colorId = ColorId.blue1) {
        this.colorId = colorId;
        ColorData data = ColorDatabase.Instance.GetColorDataById(colorId);
        background.color = data.Base;
    }
}
