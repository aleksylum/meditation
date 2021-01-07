//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Meditation.Data;
using System;
using System.Linq;
using System.Reflection;
using UI.Logic;

namespace UI.Model
{
	[Serializable]
	public class Method
	{
		protected bool Equals(Method other)
		{
			return string.Equals(SimpleName, other.SimpleName, StringComparison.OrdinalIgnoreCase) &&
			       string.Equals(Type, other.Type, StringComparison.OrdinalIgnoreCase) &&
			       string.Equals(Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase) &&
			       Enumerable.SequenceEqual(Args, other.Args);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Method)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (SimpleName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(SimpleName) : 0);
				hashCode = (hashCode * 397) ^ (Type != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Type) : 0);
				hashCode = (hashCode * 397) ^
				           (Namespace != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Namespace) : 0);
				hashCode = (hashCode * 397) ^ (Args != null ? Args.GetHashCode() : 0);
				return hashCode;
			}
		}

		public Method(String assemblyName, MethodInfo methodInfo, ArgInfo[] args, String retValue)
		{
			SimpleName = methodInfo.Name;
			Type = methodInfo.DeclaringType.Name;
			Namespace = methodInfo.DeclaringType.Namespace;
			AssemblyName = assemblyName;
			Args = args;
			RetValue = retValue;
		}

		public String SimpleName;

		public String AssemblyName;
		public String Name => $"{Type}.{SimpleName}";
		public String Namespace { get; }
		public String Type;
		public ArgInfo[] Args { get; }
		public String RetValue { get; }

		public Byte[] GetHash512(Int32 pid, String processName)
		{
			return HashSha512.CalcSha512($"{pid}{processName}{Namespace}{Type}{Name}{String.Join("", Args.Select(a => a.ToString()))}");
		}
	}
}
