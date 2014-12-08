using System;
using System.Collections;
using Microsoft.Build.Framework;

namespace ILMerge.Tools.MSBuildToolTask.TestHelpers
{
	public class TestBuildEngine : IBuildEngine
	{
		public TestBuildEngine()
		{
			ContinueOnError = false;
		}

		public void LogErrorEvent(BuildErrorEventArgs e)
		{
			ErrorCount++;
			Console.WriteLine("ERROR LOG: " + e.Message);
		}

		public void LogWarningEvent(BuildWarningEventArgs e)
		{
			Console.WriteLine(" WARN LOG: " + e.Message);
		}

		public void LogMessageEvent(BuildMessageEventArgs e)
		{
			Console.WriteLine(" INFO LOG: " + e.Message);
		}

		public void LogCustomEvent(CustomBuildEventArgs e)
		{
			Console.WriteLine(" CUST LOG: " + e.Message);
		}

		public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
		{
			throw new NotImplementedException();
		}

		public bool ContinueOnError { get; set; }
		public int LineNumberOfTaskNode { get; set; }
		public int ColumnNumberOfTaskNode { get; set; }
		public string ProjectFileOfTaskNode { get; set; }

		public int ErrorCount { get; private set; }
		public bool ErrorOccurred { get { return ErrorCount > 0; } }
	}
}