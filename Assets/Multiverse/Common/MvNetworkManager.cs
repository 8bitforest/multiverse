using UnityEngine;

namespace Multiverse.Common
{
    [RequireComponent(typeof(IMvLibrary))]
    public class MvNetworkManager : Singleton<MvNetworkManager>
    {
        public string Backend => _library.Name;
        public MvMatchmaker Matchmaker { get; private set; }

        private IMvLibrary _library;

        private void Awake()
        {
            _library = GetComponent<IMvLibrary>();
            Matchmaker = new MvMatchmaker(_library.GetMatchmaker());
        }

        private void OnDestroy()
        {
            Matchmaker.Disconnect();
        }
    }
}