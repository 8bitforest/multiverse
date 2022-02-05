using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;

#endif

namespace Multiverse.Utils
{
    // Serializes all Multiverse Prefabs so that they can be found at runtime
    internal class MvPrefabs : ScriptableObject
    {
        public const string ResourceName = "MvPrefabs";

        [field: SerializeField] public List<MvGameObject> Prefabs { get; private set; }

#if UNITY_EDITOR
        [PostProcessScene]
        public static void OnPostProcessScene()
        {
            if (Application.isPlaying)
                return;

            var prefabs = CreateInstance<MvPrefabs>();
            prefabs.Prefabs = MvIdManager.GetPrefabs().ToList();
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            AssetDatabase.CreateAsset(prefabs, $"Assets/Resources/{ResourceName}.asset");
            AssetDatabase.SaveAssets();
        }
#endif
    }
}