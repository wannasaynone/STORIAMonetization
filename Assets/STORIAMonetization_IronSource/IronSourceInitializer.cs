using STORIAMonetization;
using STORIAMonetization.Init;
using System;
using UnityEngine;

namespace STORIAMonetization_IronSource
{
    public class IronSourceInitializer : InitializerBase
    {
        private const float TIME_OUT = 10f;

        private bool m_isRewardReady = false;
        private bool m_isIntertutualReady = false;

        private Action m_onDone;

        public override string GetPluginName()
        {
            return "IronSource";
        }

        public override void Start(Action onDone)
        {
            m_onDone = onDone;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChanged;

            IronSourceEvents.onInterstitialAdReadyEvent += OnIntertitialAdReady;

            UnityEngine.Object.Instantiate(Resources.Load<IconSourceStateSetter>("IconSourceStateSetter"));
            IconSourceAppKeyStorer _keys = Resources.Load<IconSourceAppKeyStorer>("IconSourceAppKeyStorer");

#if UNITY_IOS
            IronSource.Agent.init(_keys.iOSAppKey);
#elif UNITY_ANDROID
            IronSource.Agent.init(_keys.androidAppKey);
#endif

#if UNITY_EDITOR
            Debug.Log("[IronSource] Is in editor, will let Initializer finish anyway. MUST test in device after all.");
            OnAllLoaded();
#else
            //IronSource.Agent.loadInterstitial();
            //IronSource.Agent.loadISDemandOnlyRewardedVideo("0");
            //GeneralCoroutineRunner.Instance.StartCoroutine(IEWaitTimeOut());
            OnAllLoaded();
#endif
        }

        private void OnIntertitialAdReady()
        {
            Debug.Log("OnIntertitialAdReady");
            m_isIntertutualReady = true;
            if(m_isRewardReady)
            {
                OnAllLoaded();
            }
        }

        private void RewardedVideoAvailabilityChanged(bool availiable)
        {
            Debug.Log("RewardedVideoAvailabilityChanged availiable=" + availiable);
            if(availiable)
            {
                m_isRewardReady = true;
                if(m_isIntertutualReady)
                {
                    OnAllLoaded();
                }
            }
        }

        private void OnAllLoaded()
        {
            Debug.Log("IronSource OnAllLoaded");
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= RewardedVideoAvailabilityChanged;
            IronSourceEvents.onInterstitialAdReadyEvent -= OnIntertitialAdReady;

            MonetizeCenter.Instance.AdManager.SetAdUnit(new IronSourceAdUnit());
            m_onDone?.Invoke();
        }

        private System.Collections.IEnumerator IEWaitTimeOut()
        {
            yield return new WaitForSeconds(TIME_OUT);
            if (!m_isIntertutualReady || !m_isRewardReady)
            {
                Panic(new FailedInfo
                {
                    pluginName = GetPluginName(),
                    errorMessage = "Time out."
                });

                IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= RewardedVideoAvailabilityChanged;
                IronSourceEvents.onInterstitialAdReadyEvent -= OnIntertitialAdReady;

                m_onDone?.Invoke();
            }
        }
    }
}
