using System;
using Multiverse.Common;
using Photon.Pun;
using UnityEngine;

namespace Multiverse.PUN2
{
    [RequireComponent(typeof(Pun2MvMatchmaker))]
    public class Pun2MvNetworkLibrary : MonoBehaviour, IMvNetworkLibrary
    {
        private void Awake()
        {
            PhotonNetwork.Disconnect();
        }

        public IMvMatchmaker GetMatchmaker()
        {
            return GetComponent<Pun2MvMatchmaker>();
        }
    }
}