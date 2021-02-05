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

        // By serializing this we can check to see if this object was duplicated
        [SerializeField] [ReadOnlyField] private string objectId;

        public uint Id { get; private set; }
        public bool IsInstance => Id > 0;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            // I timed this and it doesn't seem slow at all for this purpose...
            // 500k in 80ms on i7 10700k
            var myId = GlobalObjectId.GetGlobalObjectIdSlow(this);
            if (myId.ToString() != objectId)
            {
                // This object is either new or duplicated
                ResetIds();
                MvIdManager.LoadCurrentIds();
                GenerateIds();
                objectId = myId.ToString();
                Undo.RecordObject(this, "Generate Ids");
            }
        }

        internal void GenerateIds()
        {
            if (PrefabUtility.IsPartOfPrefabAsset(gameObject) && prefabId == 0)
                GeneratePrefabId();
            else if (!PrefabUtility.IsPartOfPrefabAsset(gameObject) && sceneObjectId == 0)
                GenerateSceneObjectId();
        }

        internal void ResetIds()
        {
            objectId = null;
            sceneId = 0;
            sceneObjectId = 0;
            prefabId = 0;
        }

        private void GenerateSceneObjectId()
        {
            if (BuildPipeline.isBuildingPlayer)
                throw new InvalidOperationException(
                    $"{name} has no valid sceneId yet! Cannot build scene {gameObject.scene.name}!");

            if (gameObject.gameObject.scene.buildIndex == -1)
                throw new InvalidOperationException(
                    $"{gameObject.name} is in a scene not enabled in build settings. This is not supported!");

            sceneId = MvIdManager.GetSceneId(gameObject.scene.buildIndex);
            sceneObjectId = MvIdManager.GenerateNextSceneObjectId(this);
        }

        private void GeneratePrefabId()
        {
            if (BuildPipeline.isBuildingPlayer)
                throw new InvalidOperationException(
                    $"{name} has no valid prefabId yet! Cannot build scene {gameObject.scene.name}!");

            prefabId = MvIdManager.GenerateNextPrefabId(this);
        }
#endif
    }
}