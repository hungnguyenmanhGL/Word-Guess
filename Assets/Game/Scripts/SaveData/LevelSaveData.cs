using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HAVIGAME;
using HAVIGAME.SaveLoad;

public class LevelSaveData : SaveData
{
    [SerializeField] private List<TopicSaveData> topicList;
    [SerializeField] private List<int> completedTopicList;

    public LevelSaveData() {
        topicList = new List<TopicSaveData>();
        completedTopicList = new List<int>();
    }

    public void SaveTopic(int topicId, int ansIndex) {
        //if save already exist -> add new answered to that save
        foreach (TopicSaveData data in topicList) {
            if (data.Id == topicId) {
                data.Answered.Add(ansIndex);
                SetChanged();
                return;
            }
        }
        //if save is new -> init new save data
        TopicSaveData save = new TopicSaveData(topicId);
        save.Answered.Add(ansIndex);
        topicList.Add(save);
        SetChanged();
    }

    public void OnTopicComplete(int topicId) {
        int index = -1;
        for (int i=0; i<topicList.Count; i++) {
            if (topicList[i].Id == topicId) {
                index = i;
                break;
            }
        }
        if (index >= 0) {
            topicList.RemoveAt(index);
            Debug.LogWarning(string.Format("[LevelSaveData] Found save data id {0} completed. Removed!", topicId));
        }

        completedTopicList.Add(topicId);
        SetChanged();
    }

    public TopicSaveData GetTopicSave(int topicId) {
        foreach (TopicSaveData data in topicList) {
            if (data.Id == topicId) return data;
        }
        return null;
    }

    public bool IsTopicCompleted(int topicId) {
        return completedTopicList.Contains(topicId);
    }
}

[System.Serializable]
public class TopicSaveData {
    [SerializeField] private int id;
    [SerializeField] private List<int> answered;// save answered by index in answer list in TopicData.cs

    public int Id => id;
    public List<int> Answered => answered;

    public TopicSaveData(int id) {
        this.id = id;
        answered = new List<int>();
    }

    public TopicSaveData(int id, List<int> ans) {
        this.id = id;
        this.answered = new List<int>(ans);
    }
}
