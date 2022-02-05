using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor;
#endif
using IdDict = System.Collections.Generic.Dictionary<uint, Multiverse.MvGameObject>;

namespace Multiverse.Utils
{
    internal static class MvIdManager
    {
        private static readonly Dictionary<int, byte> SceneIds = new Dictionary<int, byte>();
        private static readonly Dictionary<byte, int> Scenes = new Dictionary<byte, int>();

        private static readonly Dictionary<byte, IdDict> SceneObjects = new Dictionary<byte, IdDict>();
        private static readonly IdDict Prefabs = new IdDict();
        private static readonly IdDict Instances = new IdDict();

        private static uint _lastMatchId;
        private static uint _lastPlayerId;
        private static uint _lastInstanceId;
        private static MvPrefabs _mvPrefabs;

        private static void ClearIds()
        {
            SceneIds.Clear();
            Scenes.Clear();
            SceneObjects.Clear();
            Prefabs.Clear();
            Instances.Clear();

            if (!Application.isPlaying)
                _mvPrefabs = null;
        }

        internal static void LoadMvPrefabs()
        {
            _mvPrefabs = Resources.Load<MvPrefabs>(MvPrefabs.ResourceName);
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

            // In play mode, load prefabs from _mvPrefabs, since FindObjectsOfTypeAll doesn't return prefabs
            if (_mvPrefabs)
                foreach (var prefab in _mvPrefabs.Prefabs)
                    AddIdVerifyUnique(Prefabs, prefab.PrefabId, prefab);

            foreach (var mvGameObject in mvGameObjects)
            {
                if (mvGameObject.IsSceneObject)
                {
                    var sceneId = GetSceneId(mvGameObject.gameObject.scene.buildIndex);
                    if (!SceneObjects.ContainsKey(sceneId))
                        SceneObjects[sceneId] = new IdDict();
                    AddIdVerifyUnique(SceneObjects[sceneId], mvGameObject.SceneObjectId, mvGameObject);
                }

#if UNITY_EDITOR
                if (!EditorApplication.isPlaying && mvGameObject.IsPrefab)
                    AddIdVerifyUnique(Prefabs, mvGameObject.PrefabId, mvGameObject);
                else if (EditorApplication.isPlaying && !mvGameObject.IsPrefab)
                {
                    Debug.LogError($"{mvGameObject.name} has no Multiverse Id! It needs to be resaved!");
                    EditorApplication.isPlaying = false;
                }
#endif
            }
        }


#if UNITY_EDITOR
        [PostProcessScene(0)]
        public static void OnPostProcessScene()
        {
            LoadCurrentIds();
        }

        public static uint GenerateNextSceneObjectId(MvGameObject gameObject)
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

        public static uint AllocateInstanceId(MvGameObject gameObject)
        {
            // Just use an incrementing uint for this instead of checking the dictionary
            // It's faster, and won't ever re-use ids that might get desynced on client/server
            _lastInstanceId++;
            Instances[_lastInstanceId] = gameObject;
            return _lastInstanceId;
        }

        public static void RegisterInstanceId(MvGameObject gameObject)
        {
            Instances[gameObject.Id] = gameObject;
        }

        public static void UnregisterInstanceId(MvGameObject gameObject)
        {
            Instances.Remove(gameObject.Id);
        }

        public static uint AllocateMatchId()
        {
            _lastMatchId++;
            return _lastMatchId;
        }
        
        public static uint AllocatePlayerId()
        {
            _lastPlayerId++;
            return _lastPlayerId;
        }

        public static MvGameObject GetSceneObject(byte sceneId, uint id)
        {
            return SceneObjects[sceneId][id];
        }

        public static MvGameObject GetPrefab(uint id)
        {
            return Prefabs[id];
        }

        public static IEnumerable<MvGameObject> GetPrefabs()
        {
            return Prefabs.Values;
        }

        public static MvGameObject GetInstance(uint id)
        {
            return Instances[id];
        }

        public static IEnumerable<MvGameObject> GetInstances()
        {
            return Instances.Values;
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

            existingIds[id] = gameObject;
        }

        private static uint GenerateNextId(IdDict existingIds, MvGameObject gameObject)
        {
            var nextId = 1u;
            foreach (var id in existingIds.Keys.OrderBy(i => i))
            {
                if (id == nextId)
                    nextId++;
                else
                    break;
            }

            existingIds[nextId] = gameObject;
            return nextId;
        }
    }
}