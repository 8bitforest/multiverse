using Multiverse.Common;
using TMPro;
using UnityEngine;
#if MULTIVERSE_TEST_SERVER_MIRROR_NOBLE
using kcp2k;
using Multiverse.MirrorNoble;

#elif MULTIVERSE_TEST_SERVER_PUN2
using Multiverse.PUN2;

#endif

namespace Tests.Scenes.Scripts
{
    public class TestServerLibraryAdder : MonoBehaviour
    {
        private void Awake()
        {
#if MULTIVERSE_TEST_SERVER_MIRROR_NOBLE
            gameObject.AddComponent<KcpTransport>();
            gameObject.AddComponent<MirrorNobleMvLibrary>();
            FindObjectOfType<TMP_Text>().text += " (Mirror/Noble)";
#elif MULTIVERSE_TEST_SERVER_PUN2
            gameObject.AddComponent<Pun2MvNetworkLibrary>();
            FindObjectOfType<TMP_Text>().text += " (PUN2)";
#endif
            
            gameObject.AddComponent<MvNetworkManager>();
        }
    }
}