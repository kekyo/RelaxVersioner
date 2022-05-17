////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

namespace RelaxVersioner
{
    internal sealed class Rule
    {
        public readonly string Name;
        public readonly string Key;
        public readonly string Format;

        public Rule(string name, string key, string format)
        {
            this.Name = name;
            this.Key = key;
            this.Format = format;
        }
    }
}
