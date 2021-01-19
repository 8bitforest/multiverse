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
            return new WaitForTask(e.Wait(60));
        }

        public static async Task ThrowsAsync<T>(Task asyncMethod) where T : Exception
        {
            await ThrowsAsync<T>(asyncMethod, "");
        }

        public static async Task ThrowsAsync<T>(Task asyncMethod, string message) where T : Exception
        {
            try
            {
                await asyncMethod;
            }
            catch (T)
            {
                return;
            }
            catch (Exception e)
            {
                Assert.That(e, Is.TypeOf<T>(), message != "" ? $"{message} {e}" : e.ToString());
                throw;
            }

            Assert.Fail("Expected an exception of type " + typeof(T).FullName + " but no exception was thrown.");
        }
    }
}