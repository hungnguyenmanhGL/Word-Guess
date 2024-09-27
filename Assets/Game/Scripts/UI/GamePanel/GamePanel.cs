using HAVIGAME.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class GamePanel : UIFrame {
    [Header("[References]")]
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private HideButton btnHide;
    [SerializeField] private Image background;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private TMP_InputField inputField;

    [Header("[Topic]")]
    [SerializeField] private TextMeshProUGUI tmpTopic;
    [SerializeField] private Image imgTopic;

    [Header("[Answers]")]
    [SerializeField] private AnswerView answerPrefab;
    [SerializeField] private RectTransform answerPanel;
    [SerializeField] private List<AnswerView> answerList;

    private TopicData topicData;
    private ColorData colorData;

    private int answeredCount;

    private Tween revealTween;
    private Tween wrongTween;

    private void Start() {
    }

    protected override void OnShow(bool instant = false) {
        base.OnShow(instant);
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        inputField.ActivateInputField();
    }

    protected override void OnBack() { }

    #region Gameplay
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            OnInputSubmit();
        }
    }

    // Call this method to scroll to a specific child index
    public void ScrollToAnswer(int index) {
        revealTween?.Kill();
        // Get the RectTransform of the child we want to scroll to
        RectTransform target = answerList[index].RectTrans;

        // Calculate the position we want to scroll to
        float targetPosition = target.anchoredPosition.x + (scroll.viewport.rect.width / 2) - (target.rect.width/2);

        // Use DOTween to smoothly scroll to the new position
        revealTween = scroll.content.DOLocalMoveX(-targetPosition - target.rect.width, .2f).SetEase(Ease.Linear);
    }

    #region Answer
    private void OnInputSubmit() {
        if (inputField.text.Equals(string.Empty)) {
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            inputField.ActivateInputField();
            return;
        }
        string input = inputField.text;
        inputField.text = string.Empty;
        input = input.ToLower();
        int num = IsAnswer(input);
        if (num >= 0) {
            ScrollToAnswer(num);
            answerList[num].Reveal();
            //save upon player answered correctly
            GameData.Level.SaveTopic(topicData.Id, num);
            answeredCount++;
            CheckWin();
        }
        else {
            wrongTween?.Kill();
            wrongTween = inputField.transform.DOShakeRotation(0.5f, 10);
        }
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        inputField.ActivateInputField();
    }

    private int IsAnswer(string input) {
        int ansCount = topicData.AnswerList.Count;
        for (int i=0; i < ansCount; i++) {
            if (answerList[i].IsRevealed) continue;
            AnswerData ans = answerList[i].Model;
            if (CompareAnswer(input, ans.Answer)) return i;
            foreach (string alter in ans.Allowed) {
                if (CompareAnswer(input, alter)) return i;
            }
        }
        return -1;
    }

    private bool CompareAnswer(string input, string ans) {
        string singular = Depluralize(input);
        string lowerAns = ans.ToLower();
        return input.Equals(ans) || input.Equals(lowerAns) || singular.Equals(ans) || singular.Equals(lowerAns);
    }

    public static string Depluralize(string word) {
        // Dictionary of irregular singulars
        var irregulars = new Dictionary<string, string>
        {
            { "children", "child" },
            { "feet", "foot" },
            { "teeth", "tooth" },
            { "people", "person" },
            { "mice", "mouse" },
            { "geese", "goose" },
            { "men", "man" },
            { "women", "woman" },
            { "cacti", "cactus" },
            { "bases", "basis" },
            { "axes", "axis" },
            { "fungi", "fungus" },
            { "viruses", "virus" }
        };

        // Check for irregulars first
        if (irregulars.ContainsKey(word.ToLower())) {
            return irregulars[word.ToLower()];
        }

        // Regular depurarlization rules
        if (word.EndsWith("ies") && word.Length > 3) {
            return word.Substring(0, word.Length - 3) + "y"; // e.g., "cities" -> "city"
        }

        if (word.EndsWith("es") && word.Length > 2) {
            // Handle cases like "buses", "boxes"
            if (word.EndsWith("s") || word.EndsWith("x")) {
                return word.Substring(0, word.Length - 2); // e.g., "buses" -> "bus"
            }
        }

        // Default case: remove "s"
        if (word.EndsWith("s") && word.Length > 1) {
            return word.Substring(0, word.Length - 1); // e.g., "cats" -> "cat"
        }

        // If no changes are made, return the original word
        return word;
    }
    #endregion
    #endregion

    #region Win
    private void CheckWin() {
        if (answeredCount == topicData.AnswerList.Count) {
            GameData.Level.OnTopicComplete(topicData.Id);
            Debug.LogError("Win!");
            StartCoroutine(IEWin());
        }
    }

    private IEnumerator IEWin() {
        btnHide.enabled = false;
        yield return new WaitForSeconds(2f);
        btnHide.enabled = true;
        WinPanel winPanel = UIManager.Instance.Push<WinPanel>();
        winPanel.SetColor(colorData);
        winPanel.SetTopic(topicData);
    }
    #endregion

    #region Set Topic
    public void Reset() {
        answeredCount = 0;
        scroll.content.localPosition = new Vector3(0, scroll.content.localPosition.y, 0);
        foreach (AnswerView view in answerList) {
            view.Hide();
        }
        LoadSave();
    }

    public void SetTopic(TopicData topicData) {
        this.topicData = topicData;
        imgTopic.sprite = topicData.TopicImg;
        tmpTopic.text = topicData.Topic;
        imgTopic.gameObject.SetActive(topicData.IsImageTopic);

        int ansNum = topicData.AnswerList.Count;
        int count = answerList.Count;
        int dif = ansNum - count;
        for (int i = 0; i < dif; i++) {
            AnswerView view = Instantiate(answerPrefab, answerPanel);
            view.transform.localScale = Vector3.one;
            answerList.Add(view);
        }
        if (dif < 0) {
            dif = Mathf.Abs(dif);
            for (int i = 0; i < dif; i++) {
                answerList[answerList.Count - i - 1].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < ansNum; i++) {
            answerList[i].gameObject.SetActive(true);
            answerList[i].SetNumber(i + 1);

            answerList[i].SetModel(topicData.AnswerList[i]);
            answerList[i].Show();
        }
    }

    public void LoadSave() {
        if (GameData.Level.IsTopicCompleted(topicData.Id)) {
            answeredCount = topicData.AnswerList.Count;
            foreach (AnswerView view in answerList) view.Reveal(true);
            return;
        }
        TopicSaveData data = GameData.Level.GetTopicSave(topicData.Id);
        if (data != null) {
            foreach (int index in data.Answered) {
                answeredCount++;
                answerList[index].Reveal(true);
            }
        }
    }
    #endregion

    public void SetColor(int colorId = ColorId.blue1) {
        colorData = ColorDatabase.Instance.GetColorDataById(colorId);
        background.color = colorData.LightBg;
        foreach (AnswerView view in answerList) view.SetColor(colorData);
    }
}
