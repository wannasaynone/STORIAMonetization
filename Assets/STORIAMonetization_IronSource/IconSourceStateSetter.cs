using UnityEngine;

namespace STORIAMonetization_IronSource
{
    public class IconSourceStateSetter : MonoBehaviour
    {
        private void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }
    }
}

