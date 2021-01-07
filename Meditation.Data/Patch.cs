//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Linq;

namespace Meditation.Data
{
	public class CPatch
	{
		public Guid Id { get; set; }
		public EPatchType Type { get; set; }
		public String AssemblyName { get; set; }
		public String Namespace { get; set; }
		public String TypeName { get; set; }
		public String MethodName { get; set; }
		public ArgInfo[] Args { get; set; }

		//public ArgInfo[] Generics { get; set; } //TODO add generics args support
		public String CustomMessage { get; set; }

		public CPatch()
		{

		}

		public CPatch(Guid id,EPatchType type, string assemblyName, String nameSpace, string typeName, string methodName, ArgInfo[] args, string customMessage)
		{
			Id = id;
			Type = type;
			AssemblyName = assemblyName;
			Namespace = nameSpace;
			TypeName = typeName;
			MethodName = methodName;
			Args = args;
			CustomMessage = customMessage;
		}

		public override String ToString()
		{
			return $"\nPATCH DATA: \n{Id}  \n{Type} \n{AssemblyName} \n{Namespace} \n{TypeName} \n{MethodName} {String.Join(", ",Args.Select(a=> a.ToString()))} \n{CustomMessage} \n ";
		}
	}
}
