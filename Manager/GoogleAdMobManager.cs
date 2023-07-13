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
        // Google ���o�C���L�� SDK �����������܂��B
        MobileAds.Initialize(initStatus => { initalizeComplete = true; });

        yield return new WaitUntil(() => initalizeComplete); //��true�ɂȂ�����i��

        StartCoroutine(RequestInterstitial());
    }

    private InterstitialAd interstitialAd;
    IEnumerator RequestInterstitial()
    {

        //��ŃA�v���p��ID����͂���K�v������
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-4370716390400329/7180943250"/*"ca-app-pub-3940256099942544/4411468910"*/; //���{�Ԃ̓e�X�gID�ł͂Ȃ��A���ۂ�ID���g��
        //                      �{��                                      �e�X�g
        #else
        string adUnitId = "unexpected_platform";
        #endif

        /*��������
         * 
        // Initialize an InterstitialAd.
        // �C���^�[�X�e�B�V�����L�������������܂��B
        this.interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        // �L�����N�G�X�g������ɓǂݍ��܂ꂽ�Ƃ��ɌĂяo����܂��B
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        // �L�����N�G�X�g�̓ǂݍ��݂Ɏ��s�����Ƃ��ɌĂяo����܂��B
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        // �L�����\�������Ƃ��ɌĂяo����܂��B
        this.interstitial.OnAdOpening += HandleOnAdOpening;
        // Called when the ad is closed.
        // �L��������ꂽ�Ƃ��ɌĂяo����܂��B
        this.interstitial.OnAdClosed += HandleOnAdClosed;


        // Create an empty ad request.
        // ��̍L�����N�G�X�g���쐬���܂��B
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        // ���N�G�X�g�ŃC���^�[�X�e�B�V������ǂݍ��݂܂��B
        this.interstitial.LoadAd(request);

        �����܂ŋ��^*/

        /// <summary>
        /// Loads the interstitial ad.
        /// �C���^�[�X�e�B�V�����L����ǂݍ��݂܂��B
        /// </summary>
        // Clean up the old ad before loading a new one.
        //�V�����L����ǂݍ��ޑO�ɁA�Â��L�����N���[���A�b�v���܂��B
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        //�L���̓ǂݍ��݂Ɏg�p���郊�N�G�X�g���쐬���܂��B
        var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

        bool loadComplete = false;
        // send the request to load the ad.
        // �L����ǂݍ��ރ��N�G�X�g�𑗐M���܂��B
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

        yield return new WaitUntil(() => loadComplete); //��true�ɂȂ�����i��
        RegisterEventHandlers(interstitialAd);
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        //�L�������v���グ���Ɛ��肳���Ƃ��ɔ������܂��B
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        //�L���̃C���v���b�V�������L�^���ꂽ�Ƃ��ɔ������܂��B
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        // �L���̃N���b�N���L�^���ꂽ�Ƃ��ɔ������܂��B
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        // �L�����S��ʕ\���̃R���e���c���J�����Ƃ��ɔ������܂��B
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("�L�����S��ʕ\���̃R���e���c���J�����Ƃ��ɔ������܂�.");
            //�L���\���L��bool
            InterstitialShowing = true;
        };
        // Raised when the ad closed full screen content.
        // �L�����S��ʕ\���R���e���c������Ƃ��ɔ������܂��B
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("�L�����S��ʕ\���R���e���c������Ƃ��ɔ������܂�");

            //�L���\���L��bool
            InterstitialShowing = false;
            //���������[�N�j�~
            this.interstitialAd.Destroy();

            Debug.Log("�C���^�[�X�e�B�V�����L���I��");
            StartCoroutine(RequestInterstitial());
        };
        // Raised when the ad failed to open full screen content.
        // �L�����S��ʕ\���̃R���e���c���J�����Ƃ��ł��Ȃ������Ƃ��ɔ������܂��B
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
    /*
     * ��������

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
        Debug.Log("�C���^�[�X�e�B�V�����L������������");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
        Debug.Log("�C���^�[�X�e�B�V�����ǂݎ�莸�s");
    }

    public void HandleOnAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpening event received");
        //�L���\���L��bool
        InterstitialShowing = true;
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");

        //�L���\���L��bool
        InterstitialShowing = false;
        //���������[�N�j�~
        this.interstitialAd.Destroy();

        Debug.Log("�C���^�[�X�e�B�V�����L���I��");
        RequestInterstitial();
    }

    �����܂ŋ��^
    */

    public void InterStitialStart()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("�C���^�[�X�e�B�V�����L���J�n");
            interstitialAd.Show();
        }
        else //�ǂݎ��Ɏ��s���Ă����ꍇ
        {

        }
    }
   
}
