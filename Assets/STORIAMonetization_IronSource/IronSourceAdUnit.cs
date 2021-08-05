using STORIAMonetization;
using STORIAMonetization.Advertisement;
using System;

namespace STORIAMonetization_IronSource
{
    public class IronSourceAdUnit : AdUnitBase
    {
        private bool m_isRewarded;
        private bool m_isRewardCloesd;

        public IronSourceAdUnit()
        {
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += OnRewardedVideoAvailabilityChanged;
            IronSourceEvents.onRewardedVideoAdClosedEvent += OnRewardedVideoAdClosedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += OnRewardedVideoAdRewarded;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += OnRewardedVideoAdShowFailedEvent;
            IronSourceEvents.onRewardedVideoAdLoadFailedDemandOnlyEvent += OnRewardedVideoAdLoadFailedDemandOnlyEvent;
            IronSourceEvents.onInterstitialAdReadyEvent += OnInterstitialAdReady;
            IronSourceEvents.onInterstitialAdClosedEvent += OnInterstitialAdClosed;

            IronSource.Agent.loadInterstitial();
            IronSource.Agent.loadISDemandOnlyRewardedVideo("0");

            GeneralCoroutineRunner.Instance.StartCoroutine(IECheck());
        }

        ~IronSourceAdUnit()
        {
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= OnRewardedVideoAvailabilityChanged;
            IronSourceEvents.onRewardedVideoAdClosedEvent -= OnRewardedVideoAdClosedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent -= OnRewardedVideoAdRewarded;
            IronSourceEvents.onInterstitialAdReadyEvent -= OnInterstitialAdReady;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent -= OnRewardedVideoAdShowFailedEvent;
            IronSourceEvents.onRewardedVideoAdLoadFailedDemandOnlyEvent -= OnRewardedVideoAdLoadFailedDemandOnlyEvent;
            IronSourceEvents.onInterstitialAdClosedEvent -= OnInterstitialAdClosed;
            IronSourceEvents.onInterstitialAdClosedEvent -= OnInterstitialAdClosed;

            // uncomment this if using firebase
            //IronSourceEvents.onImpressionSuccessEvent -= ImpressionSuccessEvent;
        }

        // will make sure all true in Initer
        private bool m_isRewardVideoAvailable = true;
        private bool m_isInterstitialAdReady = true;

        private Action m_onInterstitialShown;
        private Action m_onRewardShown;

        public override void ShowInterstitial(Action onShown, Action<AdvertisementManager.FailType> onFailed)
        {
            UnityEngine.Debug.Log("IronSource ShowInterstitial");
            UnityEngine.Debug.Log("IronSource m_isInterstitialAdReady=" + m_isInterstitialAdReady);
#if !UNITY_EDITOR
            if(!m_isInterstitialAdReady)
            {
                onFailed?.Invoke(AdvertisementManager.FailType.NotReadyYet);
                return;
            }

            m_isInterstitialAdReady = false;
            m_onInterstitialShown = onShown;

            IronSource.Agent.showInterstitial();
            return;
#endif
            onShown?.Invoke();
        }

        public override void ShowRewardVideo(Action onShown, Action<AdvertisementManager.FailType> onFailed)
        {
            UnityEngine.Debug.Log("IronSource ShowRewardVideo");
            UnityEngine.Debug.Log("IronSource m_isRewardVideoAvailable=" + m_isRewardVideoAvailable);
#if !UNITY_EDITOR
            if (!m_isRewardVideoAvailable)
            {
                onFailed?.Invoke(AdvertisementManager.FailType.NotReadyYet);
                return;
            }
            m_onRewardShown = onShown;

            IronSource.Agent.showRewardedVideo();
            return;
#endif
            onShown?.Invoke();
        }

        private void OnRewardedVideoAvailabilityChanged(bool available)
        {
            m_isRewardVideoAvailable = available;
        }

        private void OnInterstitialAdReady()
        {
            m_isInterstitialAdReady = true;
        }

        private void OnInterstitialAdClosed()
        {
            m_onInterstitialShown?.Invoke();
            m_onInterstitialShown = null;
        }

        private void OnRewardedVideoAdRewarded(IronSourcePlacement obj)
        {
            m_isRewarded = true;
            UnityEngine.Debug.Log("IronSource OnRewardedVideoAdRewarded");
        }

        private void OnRewardedVideoAdClosedEvent()
        {
            m_isRewardCloesd = true;
        }

        private System.Collections.IEnumerator IECheck()
        {
            if(m_isRewardCloesd && m_isRewarded)
            {
                yield return new UnityEngine.WaitForSeconds(0.1f);
                m_isRewardCloesd = m_isRewarded = false;
                m_onRewardShown?.Invoke();
            }
            yield return null;
            GeneralCoroutineRunner.Instance.StartCoroutine(IECheck());
        }

        private void OnRewardedVideoAdShowFailedEvent(IronSourceError obj)
        {
            UnityEngine.Debug.Log("OnRewardedVideoAdShowFailedEvent code=" + obj.getErrorCode() + " msg=" + obj.getDescription());
        }

        private void OnRewardedVideoAdLoadFailedDemandOnlyEvent(string arg1, IronSourceError arg2)
        {
            UnityEngine.Debug.Log("OnRewardedVideoAdLoadFailedDemandOnlyEvent code=" + arg2.getErrorCode() + " msg=" + arg2.getDescription());
        }

// uncomment this if using firebase
//
//        private void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
//        {
//            if (impressionData != null)
//            {
//                Firebase.Analytics.Parameter[] AdParameters = {
//                new Firebase.Analytics.Parameter("ad_platform", "ironSource"),
//                new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
//                new Firebase.Analytics.Parameter("ad_unit_name", impressionData.adUnit),
//                new Firebase.Analytics.Parameter("ad_format", impressionData.instanceName),
//                new Firebase.Analytics.Parameter("currency","USD"),
//                new Firebase.Analytics.Parameter("value", impressionData.revenue.ToString())
//                };
//                Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
//                {
//#if UNITY_EDITOR
//                    UnityEngine.Debug.Log("unity-script:  ImpressionSuccessEvent impressionData = " + impressionData);
//#else
//                    Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
//#endif
//                });

//            }
//        }
    }
}

