-----------------------------------------------------------------------------------------------
The .test\TestMSBuildProject directory contains a build that is used to test the freshly built
ILMerge.Tools.MSBuildToolTask.ILMergeTool class.

It performs the following actions:

  * Executes the main build of the ILMerge.Tools.MSBuildToolTask project that is
    located in the .build directory.
  * Executes the test build project located in the .test\TestMSBuildProject directory.


The .test\TestMSBuildProject\build.proj only builds the four test projects and the MSBuild
project imports the freshly built ILMerge.Tools.MSBuildToolTask.ILMergeTool pointing to the
.build/out/<Configuration>/bin directory.

After building the test projects, it executes the ILMergeTool task to merge the following 
three assemblies (in order):
  * ILMerge.Tools.MSBuildToolTask.Tests.TestAssembly1.dll (primary assembly)
  * ILMerge.Tools.MSBuildToolTask.Tests.TestAssembly2.dll
  * ILMerge.Tools.MSBuildToolTask.Tests.Common.dll

The merged assembly is placed in the following directory:
.test\TestMSBuildProject\out\Release\bin\merged

The merged assembly is named MergedTestAssembly.dll.

The primary purpose of this test MSBuild project is to illustrate the use of the
ILMergeTool MSBuild task.
