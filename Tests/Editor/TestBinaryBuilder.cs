using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Multiverse.LibraryInterfaces;

namespace Multiverse.Tests
{
    public static class TestBinaryBuilder
    {
        [MenuItem("Multiverse/Build Test Binaries")]
        public static void BuildTestBinaries()
        {
            if (BuildPipeline.isBuildingPlayer)
                return;

            var libraries = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsInterface && typeof(IMvLibrary).IsAssignableFrom(t))
                .Select(t => t.Namespace.Split('.').Last())
                .ToArray();

            foreach (var lib in libraries)
                BuildTestBinary(lib, lib.ToUpper());

            foreach (var lib in libraries)
                Debug.Log($"Built test binary for library {lib} in {Application.temporaryCachePath}");
            
        }

        private static void BuildTestBinary(string backend, string define)
        {
            var path = Path.Combine(Application.temporaryCachePath, $"MultiverseTest{backend}");
            
            BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                locationPathName = path,
                options = BuildOptions.Development | BuildOptions.IncludeTestAssemblies,
                target = EditorUserBuildSettings.activeBuildTarget,
                extraScriptingDefines = new[]
                {
                    "MULTIVERSE_TEST_SERVER",
                    $"MULTIVERSE_TEST_SERVER_{define}",
                },
                scenes = new[]
                {
                    $"Packages/com.eightbitforest.multiverse/Tests/Runtime/Assets/MultiverseTest.unity"
                }
            });
        }
    }
}