using System;
using Multiverse.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Multiverse
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Multiverse/MvGameObject")]
    public class MvGameObject : MonoBehaviour
    {
        [SerializeField] private bool allowClientSpawn;

        [SerializeField] [ReadOnlyField] private byte sceneId;
        public byte SceneId => sceneId;

        [SerializeField] [ReadOnlyField] private uint sceneObjectId;
        public uint SceneObjectId => sceneObjectId;
        public bool IsSceneObject => SceneObjectId > 0;

        [SerializeField] [ReadOnlyField] private uint prefabId;
        public uint PrefabId => prefabId;
        public bool IsPrefab => PrefabId > 0 && SceneObjectId == 0;

        public uint Id { get; private set; }
        public bool IsInstance => Id > 0;

#if UNITY_EDITOR
        private void OnValidate()
        {
            GenerateIds();
        }

        internal void GenerateIds()
        {
            // Have to check the GlobalObjectId to make sure this game object is actually new, not
            // just a duplicate of an existing object
            var myId = GlobalObjectId.GetGlobalObjectIdSlow(this);
            if (myId.identifierType == 0)
                return;

            if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                if (prefabId == 0 || !myId.Equals(MvIdManager.GetPrefabReference(prefabId).ObjectId))
                    GeneratePrefabId();
            }
            else
            {
                if (sceneObjectId == 0 ||
                    !myId.Equals(MvIdManager.GetSceneObjectReference(sceneId, sceneObjectId).ObjectId))
                    GenerateSceneObjectId();
            }
        }

        internal void ResetIds()
        {
            sceneId = 0;
            sceneObjectId = 0;
            prefabId = 0;
        }

        private void GenerateSceneObjectId()
        {
            if (Application.isPlaying)
                return;

            if (BuildPipeline.isBuildingPlayer)
                throw new InvalidOperationException(
                    $"{name} has no valid sceneId yet! Cannot build scene {gameObject.scene.name}!");

            if (gameObject.gameObject.scene.buildIndex == -1)
                throw new InvalidOperationException(
                    $"{gameObject.name} is in a scene not enabled in build settings. This is not supported!");

            Undo.RecordObject(this, "Generate SceneId");
            sceneId = MvIdManager.GetSceneId(gameObject.scene.buildIndex);
            sceneObjectId = MvIdManager.GenerateNextSceneId(this);
        }

        private void GeneratePrefabId()
        {
            if (Application.isPlaying)
                return;

            if (BuildPipeline.isBuildingPlayer)
                throw new InvalidOperationException(
                    $"{name} has no valid prefabId yet! Cannot build scene {gameObject.scene.name}!");

            prefabId = MvIdManager.GenerateNextPrefabId(this);
        }
#endif
    }
}