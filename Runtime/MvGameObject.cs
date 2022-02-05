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
        [SerializeField] private bool allowClientInstantiate;
        internal bool AllowClientInstantiate => allowClientInstantiate;
        [SerializeField] private bool allowClientDestroy;
        internal bool AllowClientDestroy => allowClientDestroy;

        [SerializeField] [ReadOnlyField] private byte sceneId;
        internal byte SceneId => sceneId;

        [SerializeField] [ReadOnlyField] private uint sceneObjectId;
        internal uint SceneObjectId => sceneObjectId;
        internal bool IsSceneObject => SceneObjectId > 0;

        [SerializeField] [ReadOnlyField] private uint prefabId;
        internal uint PrefabId => prefabId;
        internal bool IsPrefab => PrefabId > 0 && SceneObjectId == 0;

        // By serializing this we can check to see if this object was duplicated
        [SerializeField] [ReadOnlyField] private string objectId;

        public uint Id { get; private set; }
        internal bool IsInstance => Id > 0;

        public MvPlayer Owner { get; private set; }
        public bool HasOwnership { get; private set; }
        public bool HasControl { get; private set; }

        private void OnDestroy()
        {
            MvIdManager.UnregisterInstanceId(this);
        }

#if UNITY_EDITOR
        internal void OnValidate()
        {
            if (Application.isPlaying)
                return;

            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (BuildPipeline.isBuildingPlayer)
            {
                if (!IsPrefab && !IsSceneObject)
                    throw new InvalidOperationException(
                        $"{name} has no valid prefabId or sceneId yet! Cannot build scene {gameObject.scene.name}!");
                return;
            }

            // I timed this and it doesn't seem slow at all for this purpose...
            // 500k in 80ms on i7 10700k
            var myId = GlobalObjectId.GetGlobalObjectIdSlow(this);
            if ((string.IsNullOrEmpty(objectId) || myId.ToString() == new GlobalObjectId().ToString())
                && (IsPrefab || IsSceneObject))
            {
                // WTF?!?!?! THIS SHOULD NOT *FUCKING* HAPPEN
                Debug.Log("FFS");
                return;
            }

            if (myId.ToString() == new GlobalObjectId().ToString())
                return;

            if (myId.ToString() != objectId)
            {
                // This object is either new or duplicated
                Undo.RecordObject(gameObject, "Generate Ids");
                ResetIds();
                MvIdManager.LoadCurrentIds();
                GenerateIds();
                objectId = myId.ToString();
            }
        }

        private void GenerateIds()
        {
            if (PrefabUtility.IsPartOfPrefabAsset(gameObject) && prefabId == 0)
                GeneratePrefabId();
            else if (!PrefabUtility.IsPartOfPrefabAsset(gameObject) && sceneObjectId == 0)
                GenerateSceneObjectId();
        }

        internal void ResetIds()
        {
            objectId = "reset";
            sceneId = 0;
            sceneObjectId = 0;
            prefabId = 0;
        }

        private void GenerateSceneObjectId()
        {
            // If in prefab edit mode
            if (string.IsNullOrEmpty(gameObject.scene.path))
                return;

            if (BuildPipeline.isBuildingPlayer)
                throw new InvalidOperationException(
                    $"{name} has no valid sceneId yet! Cannot build scene {gameObject.scene.name}!");

            if (gameObject.scene.buildIndex == -1)
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

        internal void GenerateInstanceId()
        {
            Id = MvIdManager.AllocateInstanceId(this);
        }

        internal void AssignInstanceId(uint id)
        {
            Id = id;
            MvIdManager.RegisterInstanceId(this);
        }

        internal void AssignOwner(MvPlayer owner)
        {
            Owner = owner;
            HasOwnership = Owner?.IsLocal ?? MvNetworkManager.I.HasServer;
            HasControl = MvNetworkManager.I.HasServer || Owner!.IsLocal;
        }
    }
}