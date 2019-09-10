using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.DotNet.PlatformAbstractions
{
	public struct HashCodeCombiner
	{
		private long _combinedHash64;

		public int CombinedHash
		{
			get
			{
				return _combinedHash64.GetHashCode();
			}
		}

		private HashCodeCombiner(long seed)
		{
			_combinedHash64 = seed;
		}

		public void Add(int i)
		{
			_combinedHash64 = (((_combinedHash64 << 5) + _combinedHash64) ^ i);
		}

		public void Add(string s)
		{
			int i = s?.GetHashCode() ?? 0;
			Add(i);
		}

		public void Add(object o)
		{
			int i = o?.GetHashCode() ?? 0;
			Add(i);
		}

		public void Add<TValue>(TValue value, IEqualityComparer<TValue> comparer)
		{
			int i = (value != null) ? comparer.GetHashCode(value) : 0;
			Add(i);
		}

		public static HashCodeCombiner Start()
		{
			return new HashCodeCombiner(5381L);
		}
	}
}
