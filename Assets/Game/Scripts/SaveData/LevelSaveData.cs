using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HAVIGAME;
using HAVIGAME.SaveLoad;

public class LevelSaveData : SaveData
{
    [SerializeField] private List<TopicSaveData> topicList;
    [SerializeField] private List<TopicSaveData> themeTopicList;
    [SerializeField] private List<int> completedTopicList;
    [SerializeField] private List<int> completedThemeList;

    public LevelSaveData() {
        topicList = new List<TopicSaveData>();
        themeTopicList = new List<TopicSaveData>();
        completedTopicList = new List<int>();
        completedThemeList = new List<int>();
    }

    public void SaveTopic(int topicId, int ansIndex, bool isTheme = false) {
        if (!isTheme) {
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
        }
        else {
            foreach (TopicSaveData data in themeTopicList) {
                if (data.Id == topicId) {
                    data.Answered.Add(ansIndex);
                    SetChanged();
                    return;
                }
            }
            TopicSaveData save = new TopicSaveData(topicId);
            save.Answered.Add(ansIndex);
            themeTopicList.Add(save);
        }
        SetChanged();
    }

    public void OnTopicComplete(int topicId, bool isTheme = false) {
        int index = -1;
        if (!isTheme) {
            for (int i = 0; i < topicList.Count; i++) {
                if (topicList[i].Id == topicId) {
                    index = i;
                    break;
                }
            }
            if (index >= 0) {
                topicList.RemoveAt(index);
                Debug.LogWarning(string.Format("[LevelSaveData] Found save data id {0} in normal, now completed. Removed!", topicId));
            }
            completedTopicList.Add(topicId);
        }
        else {
            for (int i = 0; i < themeTopicList.Count; i++) {
                if (themeTopicList[i].Id == topicId) {
                    index = i;
                    break;
                }
            }
            if (index >= 0) {
                themeTopicList.RemoveAt(index);
                Debug.LogWarning(string.Format("[LevelSaveData] Found save data id {0} in theme, now completed. Removed!", topicId));
            }
            completedThemeList.Add(topicId);
        }
        SetChanged();
    }

    public TopicSaveData GetTopicSave(int topicId, bool isTheme = false) {
        if (!isTheme) {
            foreach (TopicSaveData data in topicList) {
                if (data.Id == topicId) return data;
            }
        }
        else {
            foreach (TopicSaveData data in themeTopicList) {
                if (data.Id == topicId) return data;
            }
        }
        return null;
    }

    public bool IsTopicCompleted(int topicId, bool isTheme = false) {
        if (!isTheme) return completedTopicList.Contains(topicId);
        else return completedThemeList.Contains(topicId);
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
