//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Diagnostics;

namespace UI.Model
{
	public class ProcessModel
	{
		public Process Process { get; }
		public string Name => Process.ProcessName;
		public Int32 PID => Process.Id;

		public ProcessModel(Process process)
		{
			Process = process;
		}
	}
}
