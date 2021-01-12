using Multiverse.Common;
using Photon.Pun;
using UnityEngine;

namespace Multiverse.PUN2
{
    [RequireComponent(typeof(Pun2MvLibraryMatchmaker))]
    public class Pun2MvLibrary : MonoBehaviour, IMvLibrary
    {
        public string Name => "Pun2";
        
        private void Awake()
        {
            PhotonNetwork.Disconnect();
        }

        public IMvLibraryMatchmaker GetMatchmaker()
        {
            return GetComponent<Pun2MvLibraryMatchmaker>();
        }
    }
}