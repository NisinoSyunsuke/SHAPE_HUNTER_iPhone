using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleAdMobManager : MonoBehaviour
{
 
    public bool InterstitialShowing;
    public IEnumerator StartProcess()
    {
        bool initalizeComplete = false;
        // Initialize the Google Mobile Ads SDK.
        // Google モバイル広告 SDK を初期化します。
        MobileAds.Initialize(initStatus => { initalizeComplete = true; });

        yield return new WaitUntil(() => initalizeComplete); //←trueになったら進む

        StartCoroutine(RequestInterstitial());
    }

    private InterstitialAd interstitialAd;
    IEnumerator RequestInterstitial()
    {

        //後でアプリ用のIDを入力する必要がある
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-4370716390400329/7180943250"/*"ca-app-pub-3940256099942544/4411468910"*/; //←本番はテストIDではなく、実際のIDを使う
        //                      本番                                      テスト
        #else
        string adUnitId = "unexpected_platform";
        #endif

        /*ここから
         * 
        // Initialize an InterstitialAd.
        // インタースティシャル広告を初期化します。
        this.interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        // 広告リクエストが正常に読み込まれたときに呼び出されます。
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        // 広告リクエストの読み込みに失敗したときに呼び出されます。
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        // 広告が表示されるときに呼び出されます。
        this.interstitial.OnAdOpening += HandleOnAdOpening;
        // Called when the ad is closed.
        // 広告が閉じられたときに呼び出されます。
        this.interstitial.OnAdClosed += HandleOnAdClosed;


        // Create an empty ad request.
        // 空の広告リクエストを作成します。
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        // リクエストでインタースティシャルを読み込みます。
        this.interstitial.LoadAd(request);

        ここまで旧型*/

        /// <summary>
        /// Loads the interstitial ad.
        /// インタースティシャル広告を読み込みます。
        /// </summary>
        // Clean up the old ad before loading a new one.
        //新しい広告を読み込む前に、古い広告をクリーンアップします。
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        //広告の読み込みに使用するリクエストを作成します。
        var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

        bool loadComplete = false;
        // send the request to load the ad.
        // 広告を読み込むリクエストを送信します。
        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitialAd = ad;
                loadComplete = true;
            });

        yield return new WaitUntil(() => loadComplete); //←trueになったら進む
        RegisterEventHandlers(interstitialAd);
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        //広告が収益を上げたと推定されるときに発生します。
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        //広告のインプレッションが記録されたときに発生します。
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        // 広告のクリックが記録されたときに発生します。
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        // 広告が全画面表示のコンテンツを開いたときに発生します。
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("広告が全画面表示のコンテンツを開いたときに発生します.");
            //広告表示有無bool
            InterstitialShowing = true;
        };
        // Raised when the ad closed full screen content.
        // 広告が全画面表示コンテンツを閉じたときに発生します。
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("広告が全画面表示コンテンツを閉じたときに発生します");

            //広告表示有無bool
            InterstitialShowing = false;
            //メモリリーク阻止
            this.interstitialAd.Destroy();

            Debug.Log("インタースティシャル広告終了");
            StartCoroutine(RequestInterstitial());
        };
        // Raised when the ad failed to open full screen content.
        // 広告が全画面表示のコンテンツを開くことができなかったときに発生します。
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
    /*
     * ここから

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
        Debug.Log("インタースティシャル広告初期化完了");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
        Debug.Log("インタースティシャル読み取り失敗");
    }

    public void HandleOnAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpening event received");
        //広告表示有無bool
        InterstitialShowing = true;
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");

        //広告表示有無bool
        InterstitialShowing = false;
        //メモリリーク阻止
        this.interstitialAd.Destroy();

        Debug.Log("インタースティシャル広告終了");
        RequestInterstitial();
    }

    ここまで旧型
    */

    public void InterStitialStart()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("インタースティシャル広告開始");
            interstitialAd.Show();
        }
        else //読み取りに失敗していた場合
        {

        }
    }
   
}
