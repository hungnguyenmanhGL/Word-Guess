using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HAVIGAME;

[CreateAssetMenu(fileName = "Level 0", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject, IIdentify<int>
{
    [SerializeField] private int id;
    [SerializeField] private List<TopicData> topicList;

    public int Id => id;
    public List<TopicData> TopicList => topicList;

    public TopicData GetTopicByOrder(int i) {
        if (i < topicList.Count) return topicList[i];
        return topicList[0];
    }
}
