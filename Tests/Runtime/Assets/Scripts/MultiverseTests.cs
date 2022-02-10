using System;
using System.Linq;
using Multiverse.Tests.Backend;
using TMPro;
using UnityEngine;

namespace Multiverse.Tests.Assets.Scripts
{
    public class MultiverseTests : MonoBehaviour
    {
        [SerializeField] private MultiverseTestObjects testObjects;
        public MultiverseTestObjects TestObjects => testObjects;

        private void Awake()
        {
            // TODO: Pass backend name in via cli arg, find via IMvLibraryTestSuite. Don't build multiple backend bins
            var testSuiteType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .First(t => !t.IsInterface && typeof(IMvLibraryTestSuite).IsAssignableFrom(t));
            var testSuite = (IMvLibraryTestSuite) Activator.CreateInstance(testSuiteType);

            FindObjectOfType<TMP_Text>().text += $" ({testSuite.Name})";
            testSuite.AddLibrary(gameObject);
            var nm = gameObject.AddComponent<MvNetworkManager>();
            nm.SetTimeout(5);
            
            if (Environment.GetCommandLineArgs().Contains("-client"))
                gameObject.AddComponent<TestClient>();
            else
                gameObject.AddComponent<TestServer>();
        }
    }
}