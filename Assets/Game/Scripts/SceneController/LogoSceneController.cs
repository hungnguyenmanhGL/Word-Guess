using HAVIGAME;
using HAVIGAME.Scenes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using HAVIGAME.Services.Advertisings;

public class LogoSceneController : SceneController {
    [SerializeField] private LoadingView loadingView;
    [SerializeField] private PoolPreloader[] poolPreloaders;
    [SerializeField] private GameObject ingameConsolePrefab;

    private void Awake() {
        DOTween.SetTweensCapacity(512, 128);

        loadingView.Initialize();

        DontDestroyOnLoad(this.gameObject);
    }

    protected override void OnSceneStart() {
        GameManager.Instance.ManualInitialize();

        StartCoroutine(IEFakeLoading());
    }

    private IEnumerator IEFakeLoading() {
        loadingView.OnStart();
        yield return StartCoroutine(loadingView.FadeOut());
        loadingView.OnStartLoading();
        yield return StartCoroutine(loadingView.FadeIn());

        if (Log.LogLevel == LogLevel.Debug) {
            if (ingameConsolePrefab) Instantiate(ingameConsolePrefab);
        }

        int steps = 1;
        int totalSteps = 5;
        float elapsed = 0f;
        float duration = GameRemoteConfig.AppOpenLoadingDuration;
        float loadingProcessing = 0f;

        while (steps == 1) {
            elapsed += Time.deltaTime;

            if (!GameLauncher.Launching) {
                loadingProcessing = Mathf.Clamp(loadingProcessing + Time.deltaTime, 0, Mathf.Min((float)steps / totalSteps, elapsed / duration));
                loadingView.OnLoading(loadingProcessing);
                yield return null;
            }
            else {
                steps++;
            }
        }

        yield return null;

        int seasonCount = GameData.Player.SeasonCount;

        foreach (var item in poolPreloaders) {
            elapsed += Time.deltaTime;
            loadingProcessing = Mathf.Clamp(loadingProcessing + Time.deltaTime, 0, Mathf.Min((float)steps / totalSteps, elapsed / duration));
            loadingView.OnLoading(loadingProcessing);
            item.Preload();
            yield return null;
        }

        steps++;

        if (GameAdvertising.IsRemoveAds() || (!GameAdvertising.IsAppOpenIntertitialAdEnable && !GameAdvertising.IsAppOpenAdEnable)) {
            steps += 2;
        } else {
#if UNITY_EDITOR
            steps += 2;
#endif
        }

        while (steps == 3) {
            if (seasonCount > 0) elapsed += Time.deltaTime;

            if (!GameAdvertising.IsAllServiceInitialized()) {
                loadingProcessing = Mathf.Clamp(loadingProcessing + Time.deltaTime, 0, Mathf.Min((float)steps / totalSteps, elapsed / duration));
                loadingView.OnLoading(loadingProcessing);

                if (elapsed >= duration) {
                    steps++;
                }

                yield return null;
            } else {
                steps++;
            }
        }

        bool useAppOpenInterstitialAd = seasonCount <= 0 && GameAdvertising.IsAppOpenIntertitialAdEnable;

        if (steps == 4 && useAppOpenInterstitialAd) {
            InterstitialAd appOpenInterstitialAd = AdvertisingManager.GetInterstitialAd(GameAdvertising.appOpenInterstitialAdFilter);

            if (appOpenInterstitialAd != null && !appOpenInterstitialAd.AutoLoad) {
                appOpenInterstitialAd.SetAutoLoad(true);
                appOpenInterstitialAd.Load();
            }
        }

        while (steps == 4) {
            elapsed += Time.deltaTime;

            if (!GameAdvertising.IsInitialized() || (useAppOpenInterstitialAd ? !GameAdvertising.IsAppOpenInterstitialAdReady : !GameAdvertising.IsAppOpenAdReady)) {
                loadingProcessing = Mathf.Clamp(loadingProcessing + Time.deltaTime, 0, Mathf.Min((float)steps / totalSteps, elapsed / duration));
                loadingView.OnLoading(loadingProcessing);

                if (elapsed >= duration) {
                    steps++;
                }

                yield return null;
            }
            else {
                steps++;
            }
        }

        if (useAppOpenInterstitialAd) {
            InterstitialAd appOpenInterstitialAd = AdvertisingManager.GetInterstitialAd(GameAdvertising.appOpenInterstitialAdFilter);

            if (appOpenInterstitialAd != null && appOpenInterstitialAd.AutoLoad) {
                appOpenInterstitialAd.SetAutoLoad(false);
            }
        }

        bool completed = true;

#if UNITY_EDITOR

        //if (useAppOpenInterstitialAd) {
        //    completed = !GameAdvertising.TryShowAppOpenInterstitialAd(() => completed = true);
        //}
        //else {
        //    completed = !GameAdvertising.TryShowAppOpenAd(() => completed = true);
        //}

#else
        completed = true;
        //if (useAppOpenInterstitialAd) {
        //    GameAdvertising.TryShowAppOpenInterstitialAd();
        //}
        //else {
        //    GameAdvertising.TryShowAppOpenAd();
        //}
#endif


        while (!completed) {
            yield return null;
        }

        //MissionDatabase.Instance.StartMissions();

        GameData.Player.OnStartGameSeason();

        AsyncOperation operation = SceneManager.LoadSceneAsync(GameScene.ByIndex.Home, LoadSceneMode.Single);

        while (!operation.isDone) {

            elapsed += Time.deltaTime;

            loadingProcessing = Mathf.Clamp(loadingProcessing + Time.deltaTime, 0, Mathf.Min((float)steps / totalSteps, elapsed / duration));
            loadingView.OnLoading(loadingProcessing);

            if (operation.progress >= ScenesManager.maxProgress) {
                break;
            }
            else {
                yield return null;
            }
        }

        while (elapsed < duration) {
            elapsed += 10 * Time.deltaTime;
            loadingView.OnLoading(elapsed / duration);
            yield return null;
        }

        yield return StartCoroutine(loadingView.FadeOut());

        operation.allowSceneActivation = true;
        yield return new WaitUntil(() => operation.isDone);

        loadingView.OnFinishLoading();

        yield return StartCoroutine(loadingView.FadeIn());

        loadingView.OnFinish();

        Destroy(this.gameObject);
    }


    [System.Serializable]
    private class PoolPreloader {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int capacity = 4;

        public void Preload() {
            GameObjectPool.Instance.CreatePool(prefab, capacity);
        }
    }
}
