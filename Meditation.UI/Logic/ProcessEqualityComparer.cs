//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UI.Logic
{
	internal class ProcessEqualityComparer : IEqualityComparer<Process>
	{
		public bool Equals(Process x, Process y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			if (x.GetType() != y.GetType()) return false;
			return x.Id == y.Id && string.Equals(x.ProcessName, y.ProcessName, StringComparison.InvariantCultureIgnoreCase);
		}

		public int GetHashCode(Process obj)
		{
			unchecked
			{
				return (obj.Id * 397) ^ StringComparer.InvariantCultureIgnoreCase.GetHashCode(obj.ProcessName);
			}
		}
	}
}
