using UnityEngine;
using System.Collections;
using HAVIGAME.Services.Advertisings;
using HAVIGAME;

public class AutoNativeAdDisplayer : NativeAdDisplayer {
    private Coroutine requestCoroutine;
    private Coroutine refreshCoroutine;

    public bool IsRequesting => requestCoroutine != null;
    public bool IsRefreshing => refreshCoroutine != null;

    public bool NativeAdReady {
        get {
#if UNITY_EDITOR || CHEAT
            return true;
#elif AD_DISABLE
            return false;
#else
            return GameAdvertising.IsNativeAdReady;
#endif
        }
    }


    private void OnEnable() {
        bool isPremium = GameAdvertising.IsRemoveAds();

        if (isPremium) {
            Hide();
        } else {
            Show();
        }
    }

    private void OnDisable() {
        Hide();
    }

    public override void Show() {
        if (view != null && !view.IsShowing) {
            RequestAd();
        }
    }

    public override void Hide() {
        StopRequestAd();
        StopRefreshAd();

        if (root) root.gameObject.SetActive(false);
        if (view != null && view.IsShowing) view.Hide();
    }

    public void Refresh() {
        Hide();

        bool isPremium = GameAdvertising.IsRemoveAds();

        if (isPremium) {
            return;
        }

        RequestAd();
    }

    private void RequestAd() {
        if (root) root.gameObject.SetActive(false);

#if UNITY_EDITOR
        if (root) root.gameObject.SetActive(true);

        if (autoRefresh) {
            refreshCoroutine = StartCoroutine(IERefreshAd());
        }
#else
        if (NativeAdReady) {
        
            if (view != null && view.IsShowing) view.Hide();

            GameAdvertising.TryShowNativeAd(view);

            if (root) root.gameObject.SetActive(true);

            if (autoRefresh) {
                refreshCoroutine = StartCoroutine(IERefreshAd());
            }
        }
        else {
            if (root) root.gameObject.SetActive(false);
            requestCoroutine = StartCoroutine(IERequestAd());
        }
#endif
    }

    private void StopRequestAd() {
        if (IsRequesting) StopCoroutine(requestCoroutine);
    }

    private void StopRefreshAd() {
        if (IsRefreshing) StopCoroutine(refreshCoroutine);
    }

    private IEnumerator IERequestAd() {
        WaitForSeconds wait = Executor.Instance.WaitForSeconds(1);

        while (!NativeAdReady) {
            yield return wait;
        }

        RequestAd();

        requestCoroutine = null;
    }

    private IEnumerator IERefreshAd() {
        yield return Executor.Instance.WaitForSeconds(RefreshTime);

        WaitForSeconds wait = Executor.Instance.WaitForSeconds(1);

        while (!NativeAdReady) {
            yield return wait;
        }

        refreshCoroutine = null;

        RequestAd();
    }
}
