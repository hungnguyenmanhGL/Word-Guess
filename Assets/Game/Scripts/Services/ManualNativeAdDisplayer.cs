using HAVIGAME.Services.Advertisings;
using HAVIGAME;
using System.Collections;
using UnityEngine;
using System;

public class ManualNativeAdDisplayer : NativeAdDisplayer {
    private NativeAd nativeAd;
    private Coroutine createCoroutine;
    private Coroutine requestCoroutine;
    private Coroutine refreshCoroutine;

    public bool IsCreating => createCoroutine != null;
    public bool IsRequesting => requestCoroutine != null;
    public bool IsRefreshing => refreshCoroutine != null;

    public bool NativeAdReady {
        get {
#if UNITY_EDITOR || CHEAT
            return true;
#elif AD_DISABLE
            return false;
#else
            return nativeAd != null && nativeAd.IsReady;
#endif
        }
    }

    private void OnDisable() {
        Hide();
    }

    private void OnDestroy() {
        Hide();
    }

    public override void Show() {
        if (nativeAd == null) {
            createCoroutine = StartCoroutine(IECreateAd(RequestAd));
        } else {
            RequestAd();
        }
    }

    public override void Hide() {
        StopLoadAd();
        StopRequestAd();
        StopRefreshAd();

        if (root) root.gameObject.SetActive(false);
        if (view != null && view.IsShowing) view.Hide();
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
            
            if (GameAdvertising.TryShowNativeAd(view, nativeAd)) {
                nativeAd = null;

                if (root) root.gameObject.SetActive(true);

                if (autoRefresh) {
                    refreshCoroutine = StartCoroutine(IERefreshAd());
                }
            }
        }
        else {
            if (root) root.gameObject.SetActive(false);
            requestCoroutine = StartCoroutine(IERequestAd());
        }
#endif
    }

    private void StopLoadAd() {
        if (IsRequesting) StopCoroutine(requestCoroutine);
    }

    private void StopRequestAd() {
        if (IsRequesting) StopCoroutine(requestCoroutine);
    }

    private void StopRefreshAd() {
        if (IsRefreshing) StopCoroutine(refreshCoroutine);
    }

    private IEnumerator IECreateAd(Action onCompleted) {
        WaitForSeconds wait = Executor.Instance.WaitForSeconds(1);

        while (!GameAdvertising.IsInitialized()) {
            yield return wait;
        }

        if (nativeAd != null) {
            AdvertisingManager.DestroyManualNativeAd(nativeAd);
            nativeAd = null;
        }

        if (GameAdvertising.TryCreateManualNativeAd(out nativeAd)) {
            nativeAd.Load();
            onCompleted?.Invoke();
        }

        createCoroutine = null;
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

        if (nativeAd != null) {
            AdvertisingManager.DestroyManualNativeAd(nativeAd);
            nativeAd = null;
        }

        if (GameAdvertising.TryCreateManualNativeAd(out nativeAd)) {
            nativeAd.Load();

            WaitForSeconds wait = Executor.Instance.WaitForSeconds(1);

            while (!NativeAdReady) {
                yield return wait;
            }

            RequestAd();
        }

        refreshCoroutine = null;
    }
}
