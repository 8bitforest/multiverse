using Mirror;
using Multiverse.Common;
using UnityEngine;

namespace Multiverse.MirrorNoble
{
    [RequireComponent(typeof(Transport), typeof(MirrorNobleMvMatchmaker))]
    public class MirrorNobleMvNetworkLibrary : MonoBehaviour, IMvNetworkLibrary
    {
        public IMvMatchmaker GetMatchmaker()
        {
            return GetComponent<MirrorNobleMvMatchmaker>();
        }
    }
}