using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME;
using HAVIGAME.UI;
using TMPro;

public class LevelView : View<LevelData>
{
    [SerializeField] protected Button btnLevel;
    [SerializeField] protected Image background;
    [Header("[Visuals]")]
    [SerializeField] protected TextMeshProUGUI tmpLevel;
    [SerializeField] protected List<StarSlot> starList;

    protected int colorId;

    protected void Start() {
        btnLevel.onClick.AddListener(OpenLevel);    
    }

    public override void Show() {
        tmpLevel.text = (Model.Id + 1).ToString();
        for (int i=0; i<3; i++) {
            starList[i].Toggle(GameData.Level.IsTopicCompleted(Model.GetTopicByOrder(i).Id, false));
        }
    }

    public void OpenLevel() {
        SelectTopicPanel selectPanel = UIManager.Instance.Push<SelectTopicPanel>();
        selectPanel.SetTopicList(Model);
        selectPanel.SetColor(ColorDatabase.Instance.GetColorDataById(colorId));
    }

    public virtual void SetColor(int colorId = ColorId.blue1) {
        this.colorId = colorId;
        ColorData data = ColorDatabase.Instance.GetColorDataById(colorId);
        background.color = data.Base;
    }
}
