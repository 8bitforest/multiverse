using Mirror;
using Multiverse.Common;
using UnityEngine;

namespace Multiverse.MirrorNoble
{
    [RequireComponent(typeof(Transport), typeof(MirrorNobleMvLibraryMatchmaker))]
    public class MirrorNobleMvLibrary : MonoBehaviour, IMvLibrary
    {
        public string Name => "MirrorNoble";

        public IMvLibraryMatchmaker GetMatchmaker()
        {
            return GetComponent<MirrorNobleMvLibraryMatchmaker>();
        }
    }
}