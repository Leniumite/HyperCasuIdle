using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
{
    public static AdsManager instance;

    [SerializeField] private bool testMode = true;
    [SerializeField] private BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

#if UNITY_IOS
    private const string GAME_ID = "5270504";
    private const string PLACEMENT_ID = "Interstitial_iOS";
#elif UNITY_ANDROID
    private const string GAME_ID = "5289755";
    private const string PLACEMENT_ID = "Interstitial_Android";
    private const string PLACEMENTBANNER_ID = "Banner_Android";
    private const string PLACEMENTREWARDED_ID = "Rewarded_Android";
#else
    private const string GAME_ID = "";
    private const string PLACEMENT_ID = "";
#endif

    public Action OnShowAdsStart;
    public Action OnShowAdsComplete;
    public Action OnShowAdsRewardedComplete;

    private void Awake()
    {
        if (instance != null)
            DestroyImmediate(instance);
        else
            instance = this;
    }

    private void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        Advertisement.Banner.SetPosition(bannerPosition);
        Advertisement.Initialize(GAME_ID, testMode, this);
#endif
}

    #region Public Methods

    #region Initialization

    public void OnInitializationComplete()
    {
        LoadBanner();
        LoadAdRewarded();
    }
    
    public void OnInitializationFailed(UnityAdsInitializationError error, string message) { }

    #endregion

    #region Interstitial
    
    public void LoadAdInterstitial()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + PLACEMENT_ID);
        Advertisement.Load(PLACEMENT_ID, this);
    }
    
    public void LoadAdRewarded()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + PLACEMENTREWARDED_ID);
        Advertisement.Load(PLACEMENTREWARDED_ID, this);
    }
    
    public void PlayAdsInterstitial()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Advertisement.isInitialized)
        {
            Advertisement.Show(PLACEMENT_ID, this);
        }
#endif
    } 
    
    public void PlayAdsRewarded()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Advertisement.isInitialized)
        {
            Advertisement.Show(PLACEMENTREWARDED_ID, this);
        }
#endif
    }
    
    public void OnUnityAdsAdLoaded(string placementId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
    }
    
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {PLACEMENT_ID} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }
    
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {PLACEMENT_ID}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Ads started");
        OnShowAdsStart?.Invoke();
    }
    
    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Ads completed");

        if (placementId.Equals(PLACEMENTREWARDED_ID))
        {
            LoadAdRewarded();
            if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                OnShowAdsRewardedComplete?.Invoke();
            }
        }
        else
        {
            Advertisement.Load(PLACEMENT_ID, this);
            OnShowAdsComplete?.Invoke();
        }
    }
    
    #endregion

    #region Banner

    // Implement a method to call when the Load Banner button is clicked:
    public void LoadBanner()
    {
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
 
        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(PLACEMENTBANNER_ID, options);
    }
 
    // Implement code to execute when the loadCallback event triggers:
    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
        ShowBannerAd();
    }
 
    // Implement code to execute when the load errorCallback event triggers:
    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
        // Optionally execute additional code, such as attempting to load another ad.
    }
 
    // Implement a method to call when the Show Banner button is clicked:
    public void ShowBannerAd()
    {
        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };
 
        // Show the loaded Banner Ad Unit:
        Advertisement.Banner.Show(PLACEMENTBANNER_ID, options);
    }
 
    // Implement a method to call when the Hide Banner button is clicked:
    void HideBannerAd()
    {
        // Hide the banner:
        Advertisement.Banner.Hide();
    }
 
    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }

    #endregion
    
    #endregion
}
