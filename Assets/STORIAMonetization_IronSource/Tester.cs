using System;
using UnityEngine;

public class Tester : MonoBehaviour
{
    private const string REWARD_TIMES_STRING_FORMAT = "Reward: {0}";
    private const string INTERSTITIAL_TIMES_STRING_FORMAT = "Interstitial: {0}";

    [SerializeField] private GameObject m_uiRoot;
    [SerializeField] private UnityEngine.UI.Text m_rewardTimesText;
    [SerializeField] private UnityEngine.UI.Text m_interstitialText;

    private int m_rewardTimes = 0;
    private int m_interstitialTimes = 0;

    private void Start()
    {
        STORIAMonetization.MonetizeCenter.Instance.Initialize(
            new STORIAMonetization.Init.InitializerBase[]
            {
                new STORIAMonetization_IronSource.IronSourceInitializer()
            }, OnInitialized);
    }

    private void OnInitialized()
    {
        m_rewardTimes = 0;
        m_interstitialTimes = 0;
        UpdateText();
        m_uiRoot.SetActive(true);
    }

    public void AssignReward_rewardVid()
    {
        m_rewardTimes++;
        UpdateText();
    }

    public void AssignReward_interstitial()
    {
        m_interstitialTimes++;
        UpdateText();
    }

    private void UpdateText()
    {
        m_rewardTimesText.text = string.Format(REWARD_TIMES_STRING_FORMAT, m_rewardTimes);
        m_interstitialText.text = string.Format(INTERSTITIAL_TIMES_STRING_FORMAT, m_interstitialTimes);
    }
}
