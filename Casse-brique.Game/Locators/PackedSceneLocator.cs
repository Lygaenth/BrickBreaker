using Godot;
using System.Collections.Generic;

namespace Cassebrique.Locators
{
    public static class PackedSceneLocator
    {
        private const string DefaultKey = "Default";

        private static class PackedSceneLocatorEntry<T> where T : Node
        {
            public static Dictionary<string, PackedScene> Instance;
        }

        public static void Register<T>(string uriName, string key = DefaultKey) where T : Node
        {
            PackedSceneLocatorEntry<T>.Instance[key] = ResourceLoader.Load<PackedScene>(uriName);
        }

        public static T GetScene<T>(string key = DefaultKey) where T : Node
        {
            return PackedSceneLocatorEntry<T>.Instance[key].Instantiate<T>();
        }
    }
}
