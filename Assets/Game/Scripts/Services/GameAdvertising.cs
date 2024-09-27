using System;
using System.Collections.Generic;
using UnityEngine;
using HAVIGAME;
using HAVIGAME.UI;
using HAVIGAME.Services.Advertisings;
using System.Collections;
using HAVIGAME.Services.IAP;

public class GameAdvertising : Singleton<GameAdvertising> {
    public static readonly AdFilter defaultAdFilter = new AdFilter(AdNetwork.All, AdGroup.One);
    public static readonly AdFilter appOpenInterstitialAdFilter = new AdFilter(AdNetwork.All, AdGroup.Two);


    public static bool IsInitialized() => AdvertisingManager.IsInitialized;
    public static bool IsAllServiceInitialized() => AdvertisingManager.IsInitialized && AdvertisingManager.IsAllServiceInitialized;


    public static bool IsAppOpenAdEnable => GameRemoteConfig.AppOpenAdEnable;
    public static bool IsIntertitialAdEnable => GameRemoteConfig.InterstitialAdEnable;
    public static bool IsAppOpenIntertitialAdEnable => GameRemoteConfig.AppOpenInterstitialAdEnable;
    public static bool IsRewaredAdEnable => GameRemoteConfig.RewardedAdEnable;
    public static bool IsBannerAdEnable => GameRemoteConfig.BannerAdEnable;
    public static bool IsMRectAdEnable => GameRemoteConfig.MRectAdEnable;
    public static bool IsNativeAdEnable => GameRemoteConfig.NativeAdEnable;


    public static float BannerAdRefreshTime => GameRemoteConfig.BannerAdRefreshTime;


    public static bool IsAppOpenAdReady => AdvertisingManager.IsAppOpenAdReady(defaultAdFilter);
    public static bool IsInterstitialAdReady => AdvertisingManager.IsInterstitialAdReady(defaultAdFilter);
    public static bool IsAppOpenInterstitialAdReady => AdvertisingManager.IsInterstitialAdReady(appOpenInterstitialAdFilter);
    public static bool IsRewardedAdReady => AdvertisingManager.IsRewardedAdReady(defaultAdFilter);
    public static bool IsBannerAdReady => AdvertisingManager.IsBannerAdReady(defaultAdFilter);
    public static bool IsMRectAdReady => AdvertisingManager.IsMediumRectangleAdReady(defaultAdFilter);
    public static bool IsNativeAdReady => AdvertisingManager.IsNativeAdReady(defaultAdFilter);


    public static bool IsShowingAppOpenAd => AdvertisingManager.IsAppOpenAdShowing(defaultAdFilter);
    public static bool IsShowingInterstitialAd => AdvertisingManager.IsInterstitialAdShowing(defaultAdFilter);
    public static bool IsShowingAppOpenInterstitialAd => AdvertisingManager.IsInterstitialAdShowing(appOpenInterstitialAdFilter);
    public static bool IsShowingRewardedAd => AdvertisingManager.IsRewardedAdShowing(defaultAdFilter);
    public static bool IsShowingBannerAd => AdvertisingManager.IsBannerAdShowing(defaultAdFilter);
    public static bool IsShowingMRectAd => AdvertisingManager.IsMediumRectangleAdShowing(defaultAdFilter);
    public static bool IsShowingNativeAd => AdvertisingManager.IsNativeAdShowing(defaultAdFilter);


    private static float lastTimeInterstitialAdShowed = float.MinValue;

    private static float lastTimeAppOpenAdShowed = float.MinValue;

    private static Coroutine bannerAdRefreshCoroutine;

    public static bool IsAppOpenAdCooldown => Time.time < lastTimeAppOpenAdShowed + GameRemoteConfig.AppOpenAdCooldownTime;
    public static bool IsInterstitialAdCooldown => Time.time < lastTimeInterstitialAdShowed + GameRemoteConfig.InterstitialAdCooldownTime;

    public static BannerPosition GetBannerAdPosition() {
        return ToBannerPosition(AdvertisingManager.GetBannerAdPosition(defaultAdFilter));
    }

    public static float GetBannerHeight() {
        if (IsShowingBannerAd) {
#if UNITY_EDITOR
            switch (GetBannerAdPosition()) {
                case BannerPosition.TopCenter:
                    return 100;
                case BannerPosition.TopLeft:
                    return 100;
                case BannerPosition.TopRight:
                    return 100;
                case BannerPosition.BottomCenter:
                    return 168;
                case BannerPosition.BottomLeft:
                    return 168;
                case BannerPosition.BottomRight:
                    return 168;
                default:
                    return 0;
            }
#else
            return AdvertisingManager.GetBannerHeight(defaultAdFilter);
#endif
        }

        return 0;
    }

    public static MRectPosition GetMRectAdPosition() {
        return ToMRectPosition(AdvertisingManager.GetMediumRectangleAdPosition(defaultAdFilter));
    }

    public static float GetMRectHeight() {
        if (IsShowingMRectAd) {
#if UNITY_EDITOR
            switch (GetMRectAdPosition()) {
                case MRectPosition.TopCenter:
                    return 250;
                case MRectPosition.TopLeft:
                    return 250;
                case MRectPosition.TopRight:
                    return 250;
                case MRectPosition.BottomCenter:
                    return 250;
                case MRectPosition.BottomLeft:
                    return 250;
                case MRectPosition.BottomRight:
                    return 250;
                default:
                    return 0;
            }
#else
            return AdvertisingManager.MediumRectangleHeight(defaultAdFilter);
#endif
        }

        return 0;
    }

    protected override void OnAwake() {
        RegisterAdEvents();
    }

    protected override void OnDestroy() {
        base.OnDestroy();

        UnregisterAdEvents();
    }

    //private void OnApplicationPause(bool pause) {
    //    if (!pause) {
    //        if (GameIAP.IsTransacting) {
    //            Log.Warning("[GameAds] IAP is transacting.");
    //            return;
    //        }


    //        if (IsShowingInterstitialAd) {
    //            Log.Warning("[GameAds] Interstitial ad is showing.");
    //            return;
    //        }


    //        if (IsShowingRewardedAd) {
    //            Log.Warning("[GameAds] Rewarded ad is showing.");
    //            return;
    //        }


    //        if (IsShowingAppOpenInterstitialAd) {
    //            Log.Warning("[GameAds] App open interstitial ad is showing.");
    //            return;
    //        }

    //        bool bannerIsShowing = IsShowingBannerAd;
    //        BannerPosition bannerAdPosition = GetBannerAdPosition();

    //        if (TryShowAppOpenAd(() => {
    //            if (bannerIsShowing) TryShowBannerAd(bannerAdPosition);
    //        })) {
    //            if (bannerIsShowing) TryHideBannerAd();
    //        }
    //    }
    //}

    #region Ad Events
    private void UnregisterAdEvents() {
        AdvertisingManager.onAdRevenuePaid -= OnAdRevenuePaid;

        AdvertisingManager.onAdLoad -= OnAdLoad;
        AdvertisingManager.onAdLoaded -= OnAdLoaded;
        AdvertisingManager.onAdLoadFailed -= OnAdLoadFailed;
        AdvertisingManager.onAdDisplay -= OnAdDisplay;
        AdvertisingManager.onAdDisplayed -= OnAdDisplayed;
        AdvertisingManager.onAdDisplayFailed -= OnAdDisplayFailed;
        AdvertisingManager.onAdClicked -= OnAdClicked;
        AdvertisingManager.onAdClosed -= OnAdClosed;
    }

    private void RegisterAdEvents() {
        AdvertisingManager.onAdRevenuePaid += OnAdRevenuePaid;

        AdvertisingManager.onAdLoad += OnAdLoad;
        AdvertisingManager.onAdLoaded += OnAdLoaded;
        AdvertisingManager.onAdLoadFailed += OnAdLoadFailed;
        AdvertisingManager.onAdDisplay += OnAdDisplay;
        AdvertisingManager.onAdDisplayed += OnAdDisplayed;
        AdvertisingManager.onAdDisplayFailed += OnAdDisplayFailed;
        AdvertisingManager.onAdClicked += OnAdClicked;
        AdvertisingManager.onAdClosed += OnAdClosed;

        IAPManager.initializeEvent.AddListener(OnIAPInitialize);
    }

    public static bool IsRemoveAds() {
        return GameData.Player.IsPremium;
    }

    private void OnIAPInitialize(bool isInitialized) {
        GameData.Player.SetPremium(GameIAP.IsRemoveAds);
    }

    private void OnAdRevenuePaid(AdService client, AdRevenuePaid adRevenuePaid) {
        if (Log.DebugEnabled) {
            Log.Debug($"[GameAdvertising][{client.Network}] ad revenue paid: {adRevenuePaid}");
        }

        GameAnalytics.LogRevenue(new GameAnalytics.GameRevenue(adRevenuePaid.adPlatform, adRevenuePaid.adSource, adRevenuePaid.adUnitName, adRevenuePaid.adFormat, adRevenuePaid.value, "USD"));

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("ad_revenue_sdk")
            .Add("level", GameData.Classic.LevelUnlocked.ToString())
            .Add("level_mode", "classic")
            .Add("ad_network", adRevenuePaid.adPlatform)
            .Add("ad_format", GetFirebaseV2AdFormatName(adRevenuePaid.adFormat))
            .Add("value", adRevenuePaid.value)
            .Add("currency", adRevenuePaid.currency));
    }

    private string GetFirebaseV2AdFormatName(string adFormat) {
        switch (adFormat) {
            case "InterstitialAd": return "interstitial";
            case "RewardedAd": return "rewarded";
            case "BannerAd": return "banner";
            case "AppOpenAd": return "appopen";
            case "RewardedInterstitialAd": return "rewardedinterstitial";
            case "MediumRectangle": return "mrec";
            case "NativeAd": return "native";
            default: return adFormat;
        }
    }

    private void OnAdLoad(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }
    private void OnAdLoaded(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }
    private void OnAdLoadFailed(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Warning);
    }
    private void OnAdDisplay(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }
    private void OnAdDisplayed(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }
    private void OnAdDisplayFailed(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Warning);
    }
    private void OnAdClicked(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }
    private void OnAdClosed(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }

    private void LogAdEvent(AdService client, AdUnit unit, AdEventArgs args, LogLevel logLevel) {
        if (Log.DebugEnabled) {
            Log.Debug($"[GameAdvertising][{client.Network}-{unit}] {args.Name}, id = {args.Get(AdEventArgs.id)}, placement = {args.Get(AdEventArgs.placement)}, info = {args.Get(AdEventArgs.info)}, error = {args.Get(AdEventArgs.error)}");
        }

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create(Utility.Text.Format("{0}_{1}", GetAdUnitName(unit), args.Name))
            .Add("network", client.Network)
            .Add("unit", unit)
            .Add("id", args.Get(AdEventArgs.id))
            .Add("placement", args.Get(AdEventArgs.placement))
            .Add("info", args.Get(AdEventArgs.info))
            .Add("error", args.Get(AdEventArgs.error)));
    }

    private string GetAdUnitName(AdUnit adUnit) {
        switch (adUnit) {
            case AdUnit.AppOpenAd: return "app_open";
            case AdUnit.BannerAd: return "banner";
            case AdUnit.RewardedAd: return "rewarded";
            case AdUnit.InterstitialAd: return "interstitial";
            case AdUnit.RewardedInterstitialAd: return "rewarded_interstitial";
            case AdUnit.MediumRectangle: return "medium_rectangle";
            case AdUnit.NativeAd: return "native";
            default: return "unknow";
        }
    }
    #endregion

    public static void RemoveAds() {
        TryHideBannerAd();
        TryHideMediumRectangleAd();
        TryHideNativeAd();
    }

    public static bool TryShowAppOpenAd(Action onCompleted = null, bool ignoreCooldownTime = false) {
        if (IsRemoveAds()) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("app_open_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        onCompleted?.Invoke();
        return true;
#endif

        if (!IsAppOpenAdEnable) {
            Log.Warning("[GameAds] App open ad is disabled");
            return false;
        }

        if (IsShowingAppOpenAd) {
            Log.Warning("[GameAds] App open ad is showing");
            return false;
        }

        if (!ignoreCooldownTime && IsAppOpenAdCooldown) {
            Log.Warning("[GameAds] App open ad is cooldown");
            return false;
        }

        if (AdvertisingManager.ShowAppOpenAd(onCompleted, GetCurrentPlacement(), defaultAdFilter)) {
            lastTimeAppOpenAdShowed = Time.time;
            return true;
        }
        else {
            Log.Warning("[GameAds] App open ad is not ready.");
            return false;
        }
    }

    public static bool TryShowInterstitialAd(Action onCompleted = null, bool ignoreCooldownTime = false) {
        if (IsRemoveAds()) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("interstitial_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        onCompleted?.Invoke();
        return true;
#endif


        if (!IsIntertitialAdEnable) {
            Log.Warning("[GameAds] Interstitial ad is disabled");
            return false;
        }

        if (IsShowingInterstitialAd) {
            Log.Warning("[GameAds] Interstitial ad is showing");
            return false;
        }

        if (!ignoreCooldownTime && IsInterstitialAdCooldown) {
            Log.Warning("[GameAds] Interstitial ad is showing");
            return false;
        }


        if (AdvertisingManager.ShowInterstitialAd(onCompleted, GetCurrentPlacement(), defaultAdFilter)) {
            lastTimeInterstitialAdShowed = Time.time;
            FixAppOpenAdCooldown();
            return true;
        }
        else {
            Log.Warning("[GameAds] Intertitial ad is not ready.");
            return false;
        }
    }

    public static bool TryShowAppOpenInterstitialAd(Action onCompleted = null, bool ignoreCooldownTime = false) {
        if (IsRemoveAds()) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("app_open_interstitial_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        onCompleted?.Invoke();
        return true;
#endif

        if (!IsAppOpenIntertitialAdEnable) {
            Log.Warning("[GameAds] App Open Interstitial ad is disabled");
            return false;
        }

        if (IsShowingAppOpenInterstitialAd) {
            Log.Warning("[GameAds] App Open Interstitial ad is showing");
            return false;
        }

        if (!ignoreCooldownTime && IsInterstitialAdCooldown) {
            Log.Warning("[GameAds] App Open Interstitial ad is showing");
            return false;
        }

        if (AdvertisingManager.ShowInterstitialAd(onCompleted, GetCurrentPlacement(), appOpenInterstitialAdFilter)) {
            lastTimeInterstitialAdShowed = Time.time;
            FixAppOpenAdCooldown();
            return true;
        }
        else {
            Log.Warning("[GameAds] App Open Intertitial ad is not ready.");
            return false;
        }
    }

    public static bool TryShowRewardedAd(Action onCompleted = null, Action onSkipped = null, bool showWarningPopup = true) {
        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("rewarded_ad_request"));


#if AD_DISABLE
        return false;
#endif

#if CHEAT
        onCompleted?.Invoke();
        return true;
#endif

        if (!IsRewaredAdEnable) {
            Log.Warning("[GameAds] Reward ad is disabled");
            return false;
        }

        if (IsShowingRewardedAd) {
            Log.Warning("[GameAds] Reward Ad is showing");
            return false;
        }


        if (AdvertisingManager.ShowRewardedAd(onCompleted, onSkipped, GetCurrentPlacement(), defaultAdFilter)) {
            FixAppOpenAdCooldown();
            return true;
        }
        else {
            if (showWarningPopup) ShowPopupAdFailed("Reward ad no ready. Please try again later.");
            Log.Warning("[GameAds] Reward ad is not ready.");
            return false;
        }
    }

    public static bool TryShowBannerAd(BannerPosition position) {
        return TryShowBannerAd(position, Vector2Int.zero);
    }

    public static bool TryShowBannerAd(BannerPosition position, Vector2Int offset) {
        if (IsRemoveAds()) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("banner_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        return true;
#endif


        if (!IsBannerAdEnable) {
            Log.Warning("[GameAds] Banner ad is disabled");
            return false;
        }

        if (IsShowingBannerAd) {
            Log.Warning("[GameAds] Banner Ad is showing");
            return false;
        }

        if (AdvertisingManager.ShowBannerAd(FromBannerPosition(position), offset, GetCurrentPlacement(), defaultAdFilter)) {
            float bannerAdRefreshTime = BannerAdRefreshTime;
            if (bannerAdRefreshTime > 0) {
                if (bannerAdRefreshCoroutine != null) Executor.Instance.Stop(bannerAdRefreshCoroutine);
                bannerAdRefreshCoroutine = Executor.Instance.Run(() => TryShowBannerAd(position, offset), bannerAdRefreshTime, false);
            }
            return true;
        }
        else {
            Log.Warning("[GameAds] Banner ad is not ready.");
            return false;
        }
    }

    public static bool TryHideBannerAd() {
        if (bannerAdRefreshCoroutine != null) Executor.Instance.Stop(bannerAdRefreshCoroutine);
        return AdvertisingManager.HideBannerAd(defaultAdFilter);
    }

    public static bool TryDestroyBannerAd() {
        if (bannerAdRefreshCoroutine != null) Executor.Instance.Stop(bannerAdRefreshCoroutine);
        return AdvertisingManager.DestroyBannerAd(defaultAdFilter);
    }

    public static bool TryShowMediumRectangleAd(MRectPosition position) {
        return TryShowMediumRectangleAd(position, Vector2Int.zero);
    }

    public static bool TryShowMediumRectangleAd(MRectPosition position, Vector2Int offset) {
        if (IsRemoveAds()) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("mrect_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        return true;
#endif


        if (!IsMRectAdEnable) {
            Log.Warning("[GameAds] MRect ad is disabled");
            return false;
        }

        if (IsShowingMRectAd) {
            Log.Warning("[GameAds] MRect Ad is showing");
            return false;
        }

        if (AdvertisingManager.ShowMediumRectangleAd(FromMRectPosition(position), offset, GetCurrentPlacement(), defaultAdFilter)) {
            return true;
        }
        else {
            Log.Warning("[GameAds] MRect ad is not ready.");
            return false;
        }
    }

    public static bool TryHideMediumRectangleAd() {
        return AdvertisingManager.HideMediumRectangleAd(defaultAdFilter);
    }

    public static bool TryShowNativeAd(NativeAdView view) {
        if (IsRemoveAds()) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("native_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        return true;
#endif

        if (!IsNativeAdEnable) {
            Log.Warning("[GameAds] Native ad is disabled");
            return false;
        }

        if (AdvertisingManager.ShowNativeAd(view, GetCurrentPlacement(), defaultAdFilter)) {
            return true;
        }
        else {
            Log.Warning("[GameAds] Native ad is not ready.");
            return false;
        }
    }

    public static bool TryShowNativeAd(NativeAdView view, NativeAd nativeAd) {
        if (IsRemoveAds()) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("native_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        return true;
#endif

        if (!IsNativeAdEnable) {
            Log.Warning("[GameAds] Native ad is disabled");
            return false;
        }

        if (nativeAd != null) {
            nativeAd.Show(view, GetCurrentPlacement());
            return true;
        } else {
            Log.Warning("[GameAds] Native ad is not ready.");
            return false;
        }
    }

    public static bool TryCreateManualNativeAd(out NativeAd nativeAd) {
        nativeAd = null;

        if (IsRemoveAds()) return false;

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        return true;
#endif

        if (!IsNativeAdEnable) {
            Log.Warning("[GameAds] Native ad is disabled");
            return false;
        }

        nativeAd = AdvertisingManager.CreateManualNativeAd(defaultAdFilter);

        if (nativeAd != null) {
            return true;
        }

        Log.Warning("[GameAds] Create manual native ad failed.");
        return false;
    }

    public static bool TryHideNativeAd() {
        return NativeAdDisplayer.HideAll();
    }

    public static void FixAppOpenAdCooldown() {
        float newLastTimeAppOpenAdShowed = Time.time - GameRemoteConfig.AppOpenAdCooldownTime + 1;

        if (lastTimeAppOpenAdShowed < newLastTimeAppOpenAdShowed) {
            lastTimeAppOpenAdShowed = newLastTimeAppOpenAdShowed;
        }
    }

    public static void ResetInterstitialAdCooldown() {
        lastTimeInterstitialAdShowed = float.MinValue;
    }

    public static void IncreaseInterstitialAdCooldown(float increaseSeconds = 15) {
        lastTimeInterstitialAdShowed += increaseSeconds;
    }

    private static BannerPosition ToBannerPosition(BannerAdPosition position) {
        switch (position) {
            case BannerAdPosition.TopCenter: return BannerPosition.TopCenter;
            case BannerAdPosition.TopLeft: return BannerPosition.TopLeft;
            case BannerAdPosition.TopRight: return BannerPosition.TopRight;
            case BannerAdPosition.Centered: return BannerPosition.Centered;
            case BannerAdPosition.CenterLeft: return BannerPosition.CenterLeft;
            case BannerAdPosition.CenterRight: return BannerPosition.CenterRight;
            case BannerAdPosition.BottomCenter: return BannerPosition.BottomCenter;
            case BannerAdPosition.BottomLeft: return BannerPosition.BottomLeft;
            case BannerAdPosition.BottomRight: return BannerPosition.BottomRight;
            default: return BannerPosition.Centered;
        }
    }

    private static BannerAdPosition FromBannerPosition(BannerPosition position) {
        switch (position) {
            case BannerPosition.TopCenter: return BannerAdPosition.TopCenter;
            case BannerPosition.TopLeft: return BannerAdPosition.TopLeft;
            case BannerPosition.TopRight: return BannerAdPosition.TopRight;
            case BannerPosition.Centered: return BannerAdPosition.Centered;
            case BannerPosition.CenterLeft: return BannerAdPosition.CenterLeft;
            case BannerPosition.CenterRight: return BannerAdPosition.CenterRight;
            case BannerPosition.BottomCenter: return BannerAdPosition.BottomCenter;
            case BannerPosition.BottomLeft: return BannerAdPosition.BottomLeft;
            case BannerPosition.BottomRight: return BannerAdPosition.BottomRight;
            default: return BannerAdPosition.Centered;
        }
    }


    private static MRectPosition ToMRectPosition(MediumRectangleAdPosition position) {
        switch (position) {
            case MediumRectangleAdPosition.TopCenter: return MRectPosition.TopCenter;
            case MediumRectangleAdPosition.TopLeft: return MRectPosition.TopLeft;
            case MediumRectangleAdPosition.TopRight: return MRectPosition.TopRight;
            case MediumRectangleAdPosition.Centered: return MRectPosition.Centered;
            case MediumRectangleAdPosition.CenterLeft: return MRectPosition.CenterLeft;
            case MediumRectangleAdPosition.CenterRight: return MRectPosition.CenterRight;
            case MediumRectangleAdPosition.BottomCenter: return MRectPosition.BottomCenter;
            case MediumRectangleAdPosition.BottomLeft: return MRectPosition.BottomLeft;
            case MediumRectangleAdPosition.BottomRight: return MRectPosition.BottomRight;
            default: return MRectPosition.Centered;
        }
    }

    private static MediumRectangleAdPosition FromMRectPosition(MRectPosition position) {
        switch (position) {
            case MRectPosition.TopCenter: return MediumRectangleAdPosition.TopCenter;
            case MRectPosition.TopLeft: return MediumRectangleAdPosition.TopLeft;
            case MRectPosition.TopRight: return MediumRectangleAdPosition.TopRight;
            case MRectPosition.Centered: return MediumRectangleAdPosition.Centered;
            case MRectPosition.CenterLeft: return MediumRectangleAdPosition.CenterLeft;
            case MRectPosition.CenterRight: return MediumRectangleAdPosition.CenterRight;
            case MRectPosition.BottomCenter: return MediumRectangleAdPosition.BottomCenter;
            case MRectPosition.BottomLeft: return MediumRectangleAdPosition.BottomLeft;
            case MRectPosition.BottomRight: return MediumRectangleAdPosition.BottomRight;
            default: return MediumRectangleAdPosition.Centered;
        }
    }


    public enum BannerPosition {
        TopCenter,
        TopLeft,
        TopRight,
        Centered,
        CenterLeft,
        CenterRight,
        BottomCenter,
        BottomLeft,
        BottomRight,
    }


    public enum MRectPosition {
        TopCenter,
        TopLeft,
        TopRight,
        Centered,
        CenterLeft,
        CenterRight,
        BottomCenter,
        BottomLeft,
        BottomRight
    }

    private static string GetCurrentPlacement() {
        if (UIManager.HasInstance) {
            UIFrame frame = UIManager.Instance.Peek();

            if (frame) {
                return frame.GetType().Name;
            }
        }

        return "NULL";
    }

    private static void ShowPopupAdFailed(string message) {
        if (UIManager.HasInstance) {
            UIManager.Instance.Push<DialogPanel>().Dialog("Failed!", message);
        }
    }
}

