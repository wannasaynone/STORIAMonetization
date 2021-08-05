using UnityEngine;

namespace STORIAMonetization_IronSource
{
    [CreateAssetMenu(menuName = "Mini Vault/IconSource App Key Storer")]
	public class IconSourceAppKeyStorer : ScriptableObject
	{
        public string iOSAppKey = "";
        public string androidAppKey = "";
    }
}
