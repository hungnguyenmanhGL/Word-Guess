using HAVIGAME.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;
using HAVIGAME;
using System.Text;

public class GamePanel : UIFrame {
    [Header("[References]")]
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private HideButton btnHide;
    [SerializeField] private Image background;
    [SerializeField] private Image inputBg;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private SubmitInputField inputField;

    [Header("[Topic]")]
    [SerializeField] private TextMeshProUGUI tmpTopic;
    [SerializeField] private Image imgTopic;

    [Header("[Answers]")]
    [SerializeField] private AnswerPanel answerPanel;
    [SerializeField] private List<AnswerView> answerList;

    private TopicData topicData;
    private ColorData colorData;

    private int answeredCount;
    private int lastHintIndex;

    private Tween revealTween;
    private Tween wrongTween;

    private void Start() {
        inputField.onKeyboardDone.AddListener(OnInputSubmit);
    }

    protected override void OnShow(bool instant = false) {
        base.OnShow(instant);
        int width = Camera.main.pixelWidth;
        if (Application.platform == RuntimePlatform.WindowsEditor && width >= 1920) {
            width = 1080 * 3;
        }
        tmpTopic.rectTransform.sizeDelta = new Vector2(width, tmpTopic.rectTransform.sizeDelta.y);
        LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);
        ToggleInput(true);
        SelectInput();
        inputField.text = string.Empty;
    }

    protected override void OnHide(bool instant = false) {
        base.OnHide(instant);
        ToggleInput(false);
    }

    protected override void OnBack() { }

    protected override void OnPause() {
        base.OnPause();
        ToggleInput(false);
    }

    protected override void OnResume() {
        base.OnResume();
        ToggleInput(true);

    }

    #region Gameplay
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            OnInputSubmit();
        }
    }

    public void SelectInput() {
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        inputField.ActivateInputField();
        //inputBg.rectTransform.anchoredPosition = new Vector2(0, KeyboardUtility.GetKeyboardHeight(false));
    }

    public void ToggleInput(bool enable) {
        if (!enable) inputField.DeactivateInputField();
        inputField.gameObject.SetActive(enable);
    }

    // Call this method to scroll to a specific child index
    public void ScrollToAnswer(int index) {
        revealTween?.Kill();
        // Get the RectTransform of the child we want to scroll to
        RectTransform target = answerList[index].RectTrans;

        // Calculate the position we want to scroll to
        float targetPosition = target.anchoredPosition.x + (scroll.viewport.rect.width / 2);
        if (target.rect.width <= 240) targetPosition += target.rect.width;
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
        input = RemoveEndSpace(input);
        input = input.ToLower();
        int num = IsAnswer(input);
        if (num >= 0) {
            RevealAnswer(num, true);
        }
        else {
            if (wrongTween == null) {
                wrongTween?.Kill();
                wrongTween = inputField.transform.DOShakeRotation(0.5f, 10).OnComplete(() => { wrongTween = null; });
            }
        }
        SelectInput();
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
        SoundDatabase.Instance.WrongAudio.Play();
        return -1;
    }

    private bool CompareAnswer(string input, string ans) {
        string singular = Depluralize(input);
        string lowerAns = ans.ToLower();

        if (Mathf.Abs(input.Length - ans.Length) >= 3 || Mathf.Abs(input.Length - lowerAns.Length) >= 3
            || Mathf.Abs(singular.Length - ans.Length) >= 3 || Mathf.Abs(singular.Length - lowerAns.Length) >= 3)
            return false;
        if (input.Equals(ans) || input.Equals(lowerAns) || singular.Equals(ans) || singular.Equals(lowerAns)) 
            return true;
        if (ans.Length >= ConfigDatabase.Instance.GrammarCheckLength) {
            if (CheckAdmissibleAnswer(input, ans) || CheckAdmissibleAnswer(input, lowerAns)
                || CheckAdmissibleAnswer(singular, ans) || CheckAdmissibleAnswer(singular, lowerAns)) 
                return true;
        }
        return false;
    }

    private void RevealAnswer(int index, bool scrollTo = true) {
        if (scrollTo) ScrollToAnswer(index);
        answerList[index].Reveal();
        answeredCount++;
        //save upon player answered correctly
        GameData.Level.SaveTopic(topicData.Id, index);
        CheckWin();
    }
    #endregion

    public void OnHintShow(AnswerView selected, HintPanel hintPanel) {
        lastHintIndex = answerList.IndexOf(selected);
        hintPanel.SetTopic(topicData);
    }

    public void OnAnswerByHint() {
        RevealAnswer(lastHintIndex, false);
    }
    #endregion

    #region String Formatting
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

    private string RemoveEndSpace(string word) {
        int end = word.Length - 1;
        int count = 0;
        while (word[end] == ' ') {
            end--;
            count++;
        }
        if (count > 0) return word.Remove(end + 1, count);
        return word;
    }

    private bool CheckAdmissibleAnswer(string a, string b) {
        if (Mathf.Abs(a.Length - b.Length) > 2) // If the length difference is more than 2, they can't be similar within two mistakes
            return false;

        int mistakes = 0;
        int i = 0, j = 0;

        while (i < a.Length && j < b.Length) {
            if (a[i] != b[j]) {
                mistakes++;
                if (mistakes > ConfigDatabase.Instance.MaxMistakeCount)
                    return false;

                if (a.Length > b.Length)
                    i++;
                else if (a.Length < b.Length)
                    j++;
                else {
                    i++;
                    j++;
                }
            } else {
                i++;
                j++;
            }
        }

        mistakes += Mathf.Abs(a.Length - i) + Mathf.Abs(b.Length - j);

        return mistakes <= ConfigDatabase.Instance.MaxMistakeCount;
    }
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
        SoundDatabase.Instance.WinAudio.Play();
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
            view.GamePanel = this;
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
        if (answerPanel) answerPanel.Recycle();
        answerPanel = PrefabDatabase.Instance.GetAnswerPanel(ansNum).Spawn();
        answerPanel.RectTrans.SetParent(scroll.content);
        answerPanel.RectTrans.localScale = Vector3.one;
        answerPanel.SetAnswerList(topicData);
        answerList = answerPanel.AnswerList;

        LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);
    }

    public void LoadSave() {
        if (GameData.Level.IsTopicCompleted(topicData.Id)) {
            answeredCount = topicData.AnswerList.Count;
            answerPanel.RevealAll();
            return;
        }
        TopicSaveData data = GameData.Level.GetTopicSave(topicData.Id);
        if (data != null) {
            foreach (int index in data.Answered) {
                answeredCount++;
                answerPanel.RevealByIndex(index, true);
            }
        }
    }
    #endregion

    public void SetColor(int colorId = ColorId.blue1) {
        colorData = ColorDatabase.Instance.GetColorDataById(colorId);
        background.color = colorData.LightBg;
        inputBg.color = colorData.Base;
        foreach (AnswerView view in answerList) view.SetColor(colorData);
    }
}
