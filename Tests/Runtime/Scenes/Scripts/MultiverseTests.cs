using System;
using System.Linq;
using UnityEngine;

namespace Multiverse.Tests.Scenes.Scripts
{
    public class MultiverseTests : MonoBehaviour
    {
        private void Awake()
        {
            if (Environment.GetCommandLineArgs().Contains("-client"))
                gameObject.AddComponent<TestClient>();
            else if (Environment.GetCommandLineArgs().Contains("-server"))
                gameObject.AddComponent<TestServer>();
        }
    }
}