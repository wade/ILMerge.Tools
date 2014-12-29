using System.IO;
using ILMerge.Tools.MSBuildToolTask.TestHelpers;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Should;
using Xunit;

namespace ILMerge.Tools.MSBuildToolTask
{
	public class ILMergeToolTests : ILMergeToolTestBase
    {
		[Fact]
		public void CanExecuteILMergeToolForSimpleTwoClassLibraryAssemblyMergeSetsToolPathToDirectoryPath()
		{
			// Arrange
			const string expectedTestClassText = "This is the default dependency text value.";
			var ilMergeToolTask = CreateILMergeToolAndInitializeOutputFile();
			ilMergeToolTask.InputAssemblies = GetInputAssemblies();
			ilMergeToolTask.ToolPath = GetTestExecutionDirectoryPath();

			// Get the TestBuildEngine instance from the ILMergeTool task instance to check for execution errors.
			var buildEngine = (TestBuildEngine)ilMergeToolTask.BuildEngine;

			// Act
			ilMergeToolTask.Execute();

			// Assert
			buildEngine.ErrorOccurred.ShouldBeFalse();
			File.Exists(ilMergeToolTask.OutputFile).ShouldBeTrue();
			var testClass = LoadTestClassFromOutputFile(ilMergeToolTask.OutputFile);
			testClass.ShouldNotBeNull();
			testClass.Text.ShouldEqual(expectedTestClassText);
		}

		[Fact]
		public void CanExecuteILMergeToolForSimpleTwoClassLibraryAssemblyMergeSetsToolPathToFullFilePath()
		{
			// Arrange
			const string expectedTestClassText = "This is the default dependency text value.";
			var ilMergeToolTask = CreateILMergeToolAndInitializeOutputFile();
			ilMergeToolTask.InputAssemblies = GetInputAssemblies();
			ilMergeToolTask.ToolPath = GetILMergeToolExecutablePathAndFileName();

			// Get the TestBuildEngine instance from the ILMergeTool task instance to check for execution errors.
			var buildEngine = (TestBuildEngine)ilMergeToolTask.BuildEngine;

			// Act
			ilMergeToolTask.Execute();

			// Assert
			buildEngine.ErrorOccurred.ShouldBeFalse();
			File.Exists(ilMergeToolTask.OutputFile).ShouldBeTrue();
			var testClass = LoadTestClassFromOutputFile(ilMergeToolTask.OutputFile);
			testClass.ShouldNotBeNull();
			testClass.Text.ShouldEqual(expectedTestClassText);
		}

		[Fact]
		public void CanExecuteILMergeToolForSimpleTwoClassLibraryAssemblyMergeSetsToolPathToFullFilePathWithAllowDuplicateTypeNames()
		{
			// Arrange
			const string expectedTestClassText = "This is the default dependency text value.";
			var ilMergeToolTask = CreateILMergeToolAndInitializeOutputFile();
			ilMergeToolTask.InputAssemblies = GetInputAssemblies();
			ilMergeToolTask.ToolPath = GetILMergeToolExecutablePathAndFileName();
			ilMergeToolTask.AllowDuplicateTypeNames = true;

			// Get the TestBuildEngine instance from the ILMergeTool task instance to check for execution errors.
			var buildEngine = (TestBuildEngine)ilMergeToolTask.BuildEngine;

			// Act
			ilMergeToolTask.Execute();

			// Assert
			buildEngine.ErrorOccurred.ShouldBeFalse();
			File.Exists(ilMergeToolTask.OutputFile).ShouldBeTrue();
			var testClass = LoadTestClassFromOutputFile(ilMergeToolTask.OutputFile);
			testClass.ShouldNotBeNull();
			testClass.Text.ShouldEqual(expectedTestClassText);
		}

		[Fact]
		public void CanExecuteILMergeToolForSimpleTwoClassLibraryAssemblyMergeSetsToolPathToFullFilePathWithSpecificDuplicateTypeName()
		{
			// Arrange
			const string expectedTestClassText = "This is the default dependency text value.";
			var ilMergeToolTask = CreateILMergeToolAndInitializeOutputFile();
			ilMergeToolTask.InputAssemblies = GetInputAssemblies();
			ilMergeToolTask.ToolPath = GetILMergeToolExecutablePathAndFileName();
			ilMergeToolTask.DuplicateTypeNames = new ITaskItem[] { new TaskItem("TestClass") };

			// Get the TestBuildEngine instance from the ILMergeTool task instance to check for execution errors.
			var buildEngine = (TestBuildEngine)ilMergeToolTask.BuildEngine;

			// Act
			ilMergeToolTask.Execute();

			// Assert
			buildEngine.ErrorOccurred.ShouldBeFalse();
			File.Exists(ilMergeToolTask.OutputFile).ShouldBeTrue();
			var testClass = LoadTestClassFromOutputFile(ilMergeToolTask.OutputFile);
			testClass.ShouldNotBeNull();
			testClass.Text.ShouldEqual(expectedTestClassText);
		}

		[Fact]
		public void CanExecuteILMergeToolForSimpleTwoClassLibraryAssemblyMergeSetsToolPathToFullFilePathWithMultipleDuplicateTypeNames()
		{
			// Arrange
			const string expectedTestClassText = "This is the default dependency text value.";
			var ilMergeToolTask = CreateILMergeToolAndInitializeOutputFile();
			ilMergeToolTask.InputAssemblies = GetInputAssemblies();
			ilMergeToolTask.ToolPath = GetILMergeToolExecutablePathAndFileName();
			ilMergeToolTask.DuplicateTypeNames = new ITaskItem[] { new TaskItem("TestClass"), new TaskItem("DependencyTestClass"), };

			// Get the TestBuildEngine instance from the ILMergeTool task instance to check for execution errors.
			var buildEngine = (TestBuildEngine)ilMergeToolTask.BuildEngine;

			// Act
			ilMergeToolTask.Execute();

			// Assert
			buildEngine.ErrorOccurred.ShouldBeFalse();
			File.Exists(ilMergeToolTask.OutputFile).ShouldBeTrue();
			var testClass = LoadTestClassFromOutputFile(ilMergeToolTask.OutputFile);
			testClass.ShouldNotBeNull();
			testClass.Text.ShouldEqual(expectedTestClassText);
		}
    }
}
