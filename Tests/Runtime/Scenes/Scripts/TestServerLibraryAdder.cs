using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Multiverse.Tests.Scenes.Scripts
{
    public class TestServerLibraryAdder : MonoBehaviour
    {
        private void Awake()
        {
            var libraryAdderType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .First(t => !t.IsInterface && typeof(IMvTestLibraryAdder).IsAssignableFrom(t));
            var libraryName = libraryAdderType.Namespace.Replace("Tests", "").Trim('.').Split('.').Last();
            var libraryAdder = (IMvTestLibraryAdder) Activator.CreateInstance(libraryAdderType);

            FindObjectOfType<TMP_Text>().text += $" ({libraryName})";
            libraryAdder.AddLibrary(gameObject);
            gameObject.AddComponent<MvNetworkManager>();
        }
    }
}