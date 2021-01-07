//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;

namespace Meditation.Data
{
	[Serializable]
	public class ArgInfo
	{
		public String AssemblyName { get; set; }
		public String Namespace { get; set; }
		public String TypeName { get; set; }

		public ArgInfo(string assemblyName, string ns, string typeName)
		{
			AssemblyName = assemblyName;
			Namespace = ns;
			TypeName = typeName;
		}

		public override string ToString()
		{
			return $"{AssemblyName} {Namespace} {TypeName}";
		}

		public ArgInfo()
		{
		}

		protected bool Equals(ArgInfo other)
		{
			return string.Equals(AssemblyName, other.AssemblyName, StringComparison.InvariantCultureIgnoreCase) && string.Equals(Namespace, other.Namespace, StringComparison.InvariantCultureIgnoreCase) && string.Equals(TypeName, other.TypeName, StringComparison.InvariantCultureIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ArgInfo) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (AssemblyName != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(AssemblyName) : 0);
				hashCode = (hashCode * 397) ^ (Namespace != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Namespace) : 0);
				hashCode = (hashCode * 397) ^ (TypeName != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(TypeName) : 0);
				return hashCode;
			}
		}
	}
}
