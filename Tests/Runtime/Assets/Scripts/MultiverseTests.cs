using System;
using System.Linq;
using UnityEngine;

namespace Multiverse.Tests.Assets.Scripts
{
    public class MultiverseTests : MonoBehaviour
    {
        private void Awake()
        {
            if (Environment.GetCommandLineArgs().Contains("-client"))
                gameObject.AddComponent<TestClient>();
            else
                gameObject.AddComponent<TestServer>();
        }
    }
}