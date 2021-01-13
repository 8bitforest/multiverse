using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Multiverse.Tests.Extensions
{
    // https://forum.unity.com/threads/can-i-replace-upgrade-unitys-nunit.488580/
    public static class AssertExtensions
    {
        public static async Task ThrowsAsync<T>(Task asyncMethod) where T : Exception
        {
            await ThrowsAsync<T>(asyncMethod, "");
        }

        public static async Task ThrowsAsync<T>(Task asyncMethod, string message) where T : Exception
        {
            try
            {
                await asyncMethod; //Should throw..
            }
            catch (T)
            {
                //Ok! Swallow the exception.
                return;
            }
            catch (Exception e)
            {
                if (message != "")
                {
                    Assert.That(e, Is.TypeOf<T>(),
                        message + " " + e.ToString()); //of course this fail because it goes through the first catch..
                }
                else
                {
                    Assert.That(e, Is.TypeOf<T>(), e.ToString());
                }

                throw; //probably unreachable
            }

            Assert.Fail("Expected an exception of type " + typeof(T).FullName + " but no exception was thrown.");
        }
    }
}