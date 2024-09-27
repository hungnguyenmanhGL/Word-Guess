using HAVIGAME;
using UnityEngine;
using SimpleJSON;

public abstract class Mission : ScriptableObject, IIdentify<string> {
    [SerializeField] private string fullName;
    [SerializeField] private string shortName;
    [SerializeField, SpriteField] private Sprite icon;
    [SerializeField] private string id;
    [SerializeField] private MissionTypes type;
    [SerializeField, TextArea(3, 5)] private string description;
    [SerializeField] private int requireProgress = 1;
    [SerializeField] private ItemStack[] rewards;

    public string Id => id;
    public MissionTypes Type => type;
    public Sprite Icon => icon;
    public string FullName => fullName;
    public string ShortName => shortName;
    public string Description => description;
    public int CurrentProgress => currentProgress;
    public int RequireProgress => requireProgress;
    public MissionStates State => state;
    public ItemStack[] Rewards => rewards;


    protected MissionStates state = MissionStates.None;
    protected int currentProgress = 0;


    public void Reset() {
        if (state == MissionStates.None) {
            OnReset();
        } else {
            Log.Warning("[Mission] Reset mission failed! Mission is running.");
        }
    }

    public void Start() {
        if (state == MissionStates.None) {
            state = MissionStates.Running;
            OnAwake();
            OnStarted();
        } else {
            Log.Warning("[Mission] Start mission failed! Mission is running.");
        }
    }

    public void Cancel(bool forceCompleted = false) {
        if (state == MissionStates.Running) {
            if (forceCompleted) {
                Complete();
            } else {
                Fail();
            }
        } else {
            Log.Warning("[Mission] Cancel mission failed! Mission is not running.");
        }
    }

    protected void Complete() {
        state = MissionStates.Completed;

        OnCompleted();
        OnKill();
    }

    protected void Fail() {
        state = MissionStates.Failed;

        OnFailed();
        OnKill();
    }

    protected void AddProgress(int value) {
        if (state == MissionStates.Running) {
            currentProgress = Mathf.Min(currentProgress + value, RequireProgress);

            if (currentProgress >= RequireProgress) {
                Complete();
            }
        }
    }

    protected virtual void OnAwake() { }
    protected virtual void OnReset() {
        currentProgress = 0;
    }
    protected virtual void OnStarted() { }
    protected virtual void OnCompleted() { }
    protected virtual void OnFailed() { }
    protected virtual void OnKill() { }

    public virtual JSONNode ToJSON() {
        JSONNode node = new JSONObject();
        node.Add("id", id);
        node.Add("progress", currentProgress);
        node.Add("state", (int)state);
        return node;
    }
    public virtual void FromJSON(JSONNode node) {
        currentProgress = node["progress"];
        state = (MissionStates)node["state"].AsInt;
    }
}

public abstract class Mission<T> : Mission where T : IEventArgs {

    protected override void OnAwake() {
        EventDispatcher.AddListener<T>(OnUpdate);
    }

    protected override void OnKill() {
        EventDispatcher.RemoveListener<T>(OnUpdate);
    }

    protected abstract void OnUpdate(T args);
}

[System.Serializable]
public enum MissionStates {
    None,
    Running,
    Completed,
    Failed,
}

[System.Serializable]
public enum MissionTypes {
    Daily,
    Weekly,
    Achievement,
}