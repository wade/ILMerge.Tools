using System.IO;
using ILMerge.Tools.MSBuildToolTask.TestHelpers;
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
    }
}
