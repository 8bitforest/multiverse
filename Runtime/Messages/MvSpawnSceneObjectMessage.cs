namespace Multiverse.Messages
{
    internal struct MvSpawnSceneObjectMessage : IMvMessage
    {
        public byte SceneId { get; set; }
        public uint SceneObjectId { get; set; }
    }
}