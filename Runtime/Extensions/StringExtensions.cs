namespace Multiverse.Extensions
{
    public static class StringExtensions
    {
        public static int GetDeterministicHashCode(this string str)
        {
            unchecked
            {
                var hash = 23;
                foreach (var c in str)
                    hash = hash * 31 + c;
                return hash;
            }
        }
    }
}