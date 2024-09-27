using HAVIGAME.Services.IAP;
using System;

public static class GameIAP {
    public static bool IsInitialized => IAPManager.IsInitialized;
    public static bool IsTransacting => IAPManager.IsTransacting;

    public static bool IsRemoveAds => false;

    public static bool IsOwned(string productId) {
        return IAPManager.IsOwned(productId);
    }

    public static bool IsSubscribed(string productId) {
        return IAPManager.IsSubscribed(productId);
    }

    public static IAPProduct GetProduct(string productId) {
        return IAPSettings.Instance.GetProduct(productId);
    }

    public static bool Purchase(string productId, Action onCompleted = null, Action<string> onFailed = null) {
#if CHEAT
        onCompleted?.Invoke();
        return true;
#else
        if (IAPManager.Purchase(productId, () => {
            int level = GameData.Classic.LevelUnlocked;
            decimal value = GetLocalizedPrice(productId);
            string currency = GetIsoCurrencyCode(productId);

            GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("iap_sdk")
                .Add("level", level.ToString())
                .Add("level_mode", "classic")
                .Add("value", (double)value)
                .Add("currency", currency)
                .Add("product_id", productId));

            GameAdvertising.FixAppOpenAdCooldown();
            onCompleted?.Invoke();
        }, onFailed)) {
            GameAdvertising.FixAppOpenAdCooldown();
            return true;
        } else {
            return false;
        }
#endif
    }

    public static void RestorePurchases(Action<string[]> onCompleted = null, Action<string> onFailed = null) {
        IAPManager.RestorePurchases(onCompleted, onFailed);
    }

    public static decimal GetLocalizedPrice(string productId) {
        return IAPManager.GetLocalizedPrice(productId);
    }

    public static string GetIsoCurrencyCode(string productId) {
        return IAPManager.GetIsoCurrencyCode(productId);
    }
}
