using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ILMerge.Tools.MSBuildToolTask.Tests.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ILMerge.Tools.MSBuildToolTask.TestHelpers
{
	public class ILMergeToolTestBase
	{
		protected ILMergeTool CreateILMergeToolAndInitializeOutputFile()
		{
			var callingMethodName = GetCallingMethodName();
			var dir = GetTestExecutionDirectoryPath();

			var outputFileName = string.Format("{0}_MergedTestAssembly.dll", callingMethodName);
			var outputFilePath = Path.Combine(dir, outputFileName);

			var pdbFileName = string.Format("{0}.pdb", Path.GetFileNameWithoutExtension(outputFileName));
			var pdbFilePath = Path.Combine(dir, pdbFileName);

			if (File.Exists(outputFilePath))
				File.Delete(outputFilePath);

			if (File.Exists(pdbFilePath))
				File.Delete(pdbFilePath);

			var buildEngine = new TestBuildEngine();
			return new ILMergeTool { BuildEngine = buildEngine, OutputFile = outputFilePath };
		}

		protected ITaskItem[] GetInputAssemblies()
		{
			var fileNames =
				new[]
				{
					"ILMerge.Tools.MSBuildToolTask.Tests.TestAssembly1.dll",
					"ILMerge.Tools.MSBuildToolTask.Tests.TestAssembly2.dll"

					// This assemly is not included in these unit tests because it is already loaded and when the merged assembly,
					// it will cause type-load issues because the runtime will be confused as to which instance of the assembly
					// should be bound to the merged assembly. This assembly will be tested in the merged assembly created by the
					// external MSBuild project located in .test\TestMSBuildProject\build.proj.
				//, "ILMerge.Tools.MSBuildToolTask.Tests.Common.dll"
				};

			var dir = GetTestExecutionDirectoryPath();

			return fileNames.Select(fileName => new TaskItem(Path.Combine(dir, fileName))).Cast<ITaskItem>().ToArray();
		}

		protected string GetILMergeToolExecutableFileName()
		{
			return "ILMerge.exe";
		}

		protected string GetILMergeToolExecutablePathAndFileName()
		{
			var dir = GetTestExecutionDirectoryPath();
			var fileName = GetILMergeToolExecutableFileName();
			return string.IsNullOrWhiteSpace(dir) ? null : Path.Combine(dir, fileName);
		}

		protected string GetTestExecutionDirectoryPath()
		{
			return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected string GetCallingMethodName()
		{
			var stackTrace = new StackTrace();
			var stackFrame = stackTrace.GetFrame(2);
			return stackFrame.GetMethod().Name;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected string GetCurrentMethodName()
		{
			var stackTrace = new StackTrace();
			var stackFrame = stackTrace.GetFrame(1);
			return stackFrame.GetMethod().Name;
		}

		protected ITestClass LoadTestClassFromOutputFile(string outputFile)
		{
			var assemblyName = Path.GetFileNameWithoutExtension(outputFile);
			var assemblyQualifiedTypeName = string.Format("ILMerge.Tools.MSBuildToolTask.Tests.TestAssembly1.TestClass, {0}", assemblyName);
			const bool throwOnError = true;
			const bool ignoreCase = true;
			var type = Type.GetType(assemblyQualifiedTypeName, throwOnError, ignoreCase);
			return Activator.CreateInstance(type) as ITestClass;
		}
	}
}