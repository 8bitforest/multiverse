using UnityEngine;

namespace Multiverse.Messages
{
    internal struct MvInstantiatePrefabMessage : IMvMessage
    {
        public uint PrefabId { get; set; }
        public uint InstanceId { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public uint ClientInstanceId { get; set; }
        public MvPlayer Owner { get; set; }
    }
}