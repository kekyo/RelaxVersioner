////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System.Reflection;

namespace RelaxVersioner
{
    internal static class Utilities
    {
        public static object? GetField(this object instance, string name)
        {
            var type = instance.GetType();
            var fi = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return fi?.GetValue(instance);
        }

        public static object? GetProperty(this object instance, string name)
        {
            var type = instance.GetType();
            var pi = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return pi?.GetValue(instance, new object[0]); 
        }
    }
}
