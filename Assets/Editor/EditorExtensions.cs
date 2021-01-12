using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class EditorExtensions
    {
        [MenuItem("Multiverse/Build Test Servers")]
        public static void BuildTestServer()
        {
            if (BuildPipeline.isBuildingPlayer)
                return;

            var prevDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(';');
            try
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone,
                    string.Join(";", prevDefines.Where(d => !d.Contains("MULTIVERSE_TEST_SERVER"))));
                BuildTestServer("MirrorNoble", "MIRROR_NOBLE");
                BuildTestServer("Pun2", "PUN2");
            }
            finally
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone,
                    string.Join(";", prevDefines));
            }

            Debug.Log("Done building test servers!");
        }

        private static void BuildTestServer(string backend, string define)
        {
            var path = Path.Combine(Application.temporaryCachePath, $"TestServer{backend}");
            BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                locationPathName = path,
                options = BuildOptions.Development,
                target = EditorUserBuildSettings.activeBuildTarget,
                extraScriptingDefines = new[]
                {
                    "MULTIVERSE_TEST_SERVER",
                    $"MULTIVERSE_TEST_SERVER_{define}",
                },
                scenes = new[]
                {
                    "Assets/Tests/Scenes/TestServer.unity"
                }
            });
        }
    }
}