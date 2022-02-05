using UnityEngine;

namespace Multiverse.Tests.Assets.Scripts
{
    [CreateAssetMenu(menuName = "Multiverse Tests Objects")]
    public class MultiverseTestObjects : ScriptableObject
    {
        [field: SerializeField] public GameObject ServerOnlyPrefab { get; private set; }
        [field: SerializeField] public GameObject ClientServerPrefab { get; private set; }
        [field: SerializeField] public GameObject NotMultiversePrefab { get; private set; }
    }
}