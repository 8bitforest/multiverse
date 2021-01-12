using UnityEngine;

namespace Multiverse.Common
{
    [RequireComponent(typeof(IMvNetworkLibrary))]
    public class MvNetworkManager : Singleton<MvNetworkManager>
    {
        public IMvMatchmaker Matchmaker { get; private set; }

        private IMvNetworkLibrary _library;

        private void Awake()
        {
            _library = GetComponent<IMvNetworkLibrary>();
            Matchmaker = _library.GetMatchmaker();
        }
    }
}