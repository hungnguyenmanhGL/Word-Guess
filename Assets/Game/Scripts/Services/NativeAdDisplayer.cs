using HAVIGAME;
using HAVIGAME.Services.Advertisings;
using System.Collections.Generic;
using UnityEngine;

public abstract class NativeAdDisplayer : MonoBehaviour {
    protected static readonly Dictionary<string, NativeAdDisplayer> displayers = new Dictionary<string, NativeAdDisplayer>();

    public static bool Showing => displayers.Count > 0;

    public static bool HideAll() {
        foreach (var displayer in displayers.Values) {
            displayer.Hide();
        }

        return displayers.Count > 0;
    }

    [SerializeField] protected string viewId;
    [SerializeField] protected NativeAdElementView[] elementViews;
    [SerializeField] protected Transform root;
    [SerializeField] protected bool autoRefresh = true;
    [SerializeField] protected IntProperty refreshTime = IntProperty.Create();

    protected NativeAdView view;

    public bool IsShowing => view != null && view.IsShowing;
    public int RefreshTime => refreshTime.Get();

    protected virtual void Awake() {
        viewId += GetHashCode();

        displayers[viewId] = this;
        view = new NativeAdView(viewId, elementViews);
    }

    public abstract void Show();
    public abstract void Hide();
}
