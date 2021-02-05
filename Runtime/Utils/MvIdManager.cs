using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor;
#endif
using IdDict = System.Collections.Generic.Dictionary<uint, Multiverse.Utils.PersistentGameObjectReference>;

namespace Multiverse.Utils
{
    internal readonly struct PersistentGameObjectReference
    {
        public MvGameObject MvGameObject { get; }
#if UNITY_EDITOR
        public GlobalObjectId ObjectId { get; }
#endif

        public PersistentGameObjectReference(MvGameObject mvGameObject)
        {
            MvGameObject = mvGameObject;

#if UNITY_EDITOR
            ObjectId = GlobalObjectId.GetGlobalObjectIdSlow(mvGameObject);
#endif
        }
    }

    internal static class MvIdManager
    {
        private static readonly Dictionary<int, byte> SceneIds = new Dictionary<int, byte>();
        private static readonly Dictionary<byte, int> Scenes = new Dictionary<byte, int>();

        private static readonly Dictionary<byte, IdDict> SceneObjects = new Dictionary<byte, IdDict>();
        private static readonly IdDict Prefabs = new IdDict();
        private static readonly IdDict Instances = new IdDict();

        private static void ClearIds()
        {
            SceneIds.Clear();
            Scenes.Clear();
            SceneObjects.Clear();
            Prefabs.Clear();
            Instances.Clear();
        }

        internal static void LoadCurrentIds()
        {
            ClearIds();

            // Load scene ids
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                Scenes[(byte) i] = i;
                SceneIds[i] = (byte) i;
            }

            // Load object ids
            var mvGameObjects = Resources.FindObjectsOfTypeAll<MvGameObject>()
                .Where(mvGo =>
                    mvGo.gameObject.hideFlags != HideFlags.NotEditable &&
                    mvGo.gameObject.hideFlags != HideFlags.HideAndDontSave &&
                    mvGo.gameObject.scene.name != "DontDestroyOnLoad");

            foreach (var mvGameObject in mvGameObjects)
            {
                if (mvGameObject.IsSceneObject)
                {
                    var sceneId = GetSceneId(mvGameObject.gameObject.scene.buildIndex);
                    if (!SceneObjects.ContainsKey(sceneId))
                        SceneObjects[sceneId] = new IdDict();
                    AddIdVerifyUnique(SceneObjects[sceneId], mvGameObject.SceneObjectId, mvGameObject);
                }
                else if (mvGameObject.IsPrefab)
                    AddIdVerifyUnique(Prefabs, mvGameObject.PrefabId, mvGameObject);
                else if (mvGameObject.IsInstance)
                    AddIdVerifyUnique(Instances, mvGameObject.Id, mvGameObject);
#if UNITY_EDITOR
                else if (EditorApplication.isPlaying)
                {
                    Debug.LogError($"{mvGameObject.name} has no Multiverse Id! It needs to be resaved!");
                    EditorApplication.isPlaying = false;
#endif
                }
            }
        }


#if UNITY_EDITOR
        [PostProcessScene]
        public static void OnPostProcessScene()
        {
            // Run this mainly just to check for duplicate ids
            LoadCurrentIds();
        }

        public static uint GenerateNextSceneId(MvGameObject gameObject)
        {
            var sceneId = GetSceneId(gameObject.gameObject.scene.buildIndex);

            if (sceneId == -1)
            {
                Debug.LogError(
                    $"{gameObject.name} is in a scene not enabled in build settings. This is not supported!");
                return 0;
            }

            // Make sure we load all scene objects in the current scene first
            if (!SceneObjects.ContainsKey(sceneId))
            {
                LoadCurrentIds();

                // If there aren't any MvGameObjects in this scene yet
                if (!SceneObjects.ContainsKey(sceneId))
                    SceneObjects[sceneId] = new IdDict();
            }

            return GenerateNextId(SceneObjects[sceneId], gameObject);
        }

        public static uint GenerateNextPrefabId(MvGameObject gameObject)
        {
            return GenerateNextId(Prefabs, gameObject);
        }
#endif

        public static uint GenerateNextInstanceId(MvGameObject gameObject)
        {
            return GenerateNextId(Instances, gameObject);
        }

        public static MvGameObject GetSceneObject(byte sceneId, uint id)
        {
            if (!SceneObjects.ContainsKey(sceneId))
                LoadCurrentIds();

            return SceneObjects[sceneId][id].MvGameObject;
        }

        internal static PersistentGameObjectReference GetSceneObjectReference(byte sceneId, uint id)
        {
            if (!SceneObjects.ContainsKey(sceneId))
                LoadCurrentIds();

            return SceneObjects[sceneId][id];
        }

        public static MvGameObject GetPrefab(uint id)
        {
            return Prefabs[id].MvGameObject;
        }

        internal static PersistentGameObjectReference GetPrefabReference(uint id)
        {
            return Prefabs[id];
        }

        public static byte GetSceneId(int buildIndex)
        {
            return SceneIds[buildIndex];
        }

        private static void AddIdVerifyUnique(IdDict existingIds, uint id, MvGameObject gameObject)
        {
            if (existingIds.ContainsKey(id))
            {
                Debug.LogError($"{gameObject.name} has a duplicated id! This should not happen!");
                return;
            }

            existingIds[id] = new PersistentGameObjectReference(gameObject);
        }

        private static uint GenerateNextId(IdDict existingIds, MvGameObject gameObject)
        {
            var nextId = 1u;
            foreach (var id in existingIds.Keys)
            {
                if (id == nextId)
                    nextId++;
                else
                    break;
            }

            existingIds[nextId] = new PersistentGameObjectReference(gameObject);
            return nextId;
        }
    }
}