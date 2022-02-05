namespace Multiverse.Tests.Assets.Scripts
{
    public class TestMatchData : IMvMatchData
    {
        public string StringData;
        public int IntData;
        public float FloatData;

        public static TestMatchData Create()
        {
            return new TestMatchData
            {
                StringData = "TestMatchData CustomData",
                IntData = 123456789,
                FloatData = 3.14159f
            };
        }
    }
}