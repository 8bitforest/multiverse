using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class MultiverseTestFixture
    {
        protected MvNetworkManager NetworkManager { get; private set; }
        protected MultiverseTestManager TestManager { get; private set; }

        private bool _setup;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestManager = Object.FindObjectOfType<MultiverseTestManager>();
            TestManager.DisconnectAndReset();
            NetworkManager = MvNetworkManager.I;
        }

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            if (!_setup)
                yield return UnityOneTimeSetUp();
            _setup = true;
        }

        protected virtual IEnumerator UnityOneTimeSetUp()
        {
            yield break;
        }
    }
}