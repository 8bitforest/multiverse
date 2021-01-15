using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using Reaction;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Multiverse.Tests.Extensions
{
    public static class AssertExtensions
    {
        public static IEnumerator EventCalled(RxnEvent e)
        {
            var go = new GameObject("Assert.EventCalled");
            var eventCalled = false;
            e.OnInvoked(go, () => eventCalled = true);

            return new WaitUntilTimeout(() =>
            {
                if (eventCalled)
                    Object.Destroy(go);
                return eventCalled;
            }, 60);
        }

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