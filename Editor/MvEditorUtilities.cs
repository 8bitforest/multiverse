using System.Linq;
using Multiverse.Utils;
using UnityEditor;
using UnityEngine;

namespace Multiverse
{
    public static class MvEditorUtilities
    {
        [MenuItem("Multiverse/Reset Current Scene & Prefab Ids")]
        public static void ResetCurrentScenePrefabIds()
        {
            var mvGameObjects = Resources.FindObjectsOfTypeAll<MvGameObject>()
                .Where(mvGo =>
                    mvGo.gameObject.hideFlags != HideFlags.NotEditable &&
                    mvGo.gameObject.hideFlags != HideFlags.HideAndDontSave &&
                    mvGo.gameObject.scene.name != "DontDestroyOnLoad").ToList();

            foreach (var mvGameObject in mvGameObjects)
                mvGameObject.ResetIds();
            MvIdManager.LoadCurrentIds();
            foreach (var mvGameObject in mvGameObjects)
                mvGameObject.OnValidate();
        }
    }
}