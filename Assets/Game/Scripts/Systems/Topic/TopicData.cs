using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HAVIGAME;

[CreateAssetMenu(fileName = "TopicData 0", menuName = "Data/TopicData")]
public class TopicData : ScriptableObject, IIdentify<int>
{
    [SerializeField] private int id;

    [Header("[Topic]")]
    [SerializeField, TextArea(2,2)] private string topic;
    [SerializeField, SpriteField] private Sprite topicImg;

    [Header("[Answers]")]
    [SerializeField] private bool isNoun;
    [SerializeField] private List<AnswerData> answerList;

    public int Id => id;
    public string Topic => topic;
    public Sprite TopicImg => topicImg;
    public bool IsNoun => isNoun;
    public List<AnswerData> AnswerList => answerList;

    public bool IsImageTopic => topicImg != null;
}

[System.Serializable]
public class AnswerData {
    [SerializeField] private string answer;
    [SerializeField] private List<string> allowed;

    public string Answer => answer;
    public List<string> Allowed => allowed;
}
