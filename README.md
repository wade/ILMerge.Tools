# ILMerge.Tools.MSBuildToolTask #

ILMerge.Tools.MSBuildToolTask is a simple **MSBuild task** that wraps the ILMerge.exe command-line tool. Unlike other ILMerge MSBuild tasks,ILMerge.Tools.MSBuildToolTask is decoupled from the ILMerge assembly which means it does not reference it nor does it load it in-process. Instead, it executes ILMerge from the command-line. This allows the ILMerge.Tools.MSBuildToolTask to be used with multiple versions of ILMerge without having to recompile or rebind ILMerge.

All features of ILMerge command-line interface are supported as a pass-through from this MSBuild task.


## NuGet package ##
This task is available as the **ILMerge.Tools.MSBuildToolTask** NuGet package. This package does not have any hard dependencies and intentionally does not prescribe which version or package of ILMerge you should use.

To install **ILMerge.Tools.MSBuildToolTask**, run the following command in the Package Manager Console

    Install-Package ILMerge.Tools.MSBuildToolTask

The NuGet package installs the **ILMerge.Tools.MSBuildToolTask.dll** assembly to the `packages/ILMerge.Tools.MSBuildToolTask.<version>/tools` directory and does not add any files or references to any projects. It does **not** include a `.targets` file, but it is very easy to reference it from your MSBuild project file. 


## Version ##
The **ILMerge.Tools.MSBuildToolTask** assembly and NuGet package uses a four-part version number. The first 3 parts of the version reflects the latest version of ILMerge for which is was tested against. The fourth part of the version number is the revision of the ILMerge.Tools project and is incremented independently of the ILMerge version number.  

For example, ILMerge.Tools.MSBuildToolTask version 2.14.1208.43 was tested against ILMerge 2.14.1208 and its private revision number is 43, which has nothing to do with ILMerge and is only used to track changes to the ILMerge.Tools project. If on a later date a new release of ILMerge.Tools.MSBuildToolTask with version 2.14.1208.48 is released, it still was tested against ILMerge 2.14.1208 but has changes that only affect the ILMerge.Tools.MSBuildToolTask project, such as fixes or other updates.  

This does not mean that you must only use it with the same version of ILMerge. As long as the command-line interface of the version of ILMerge you are using is compatible with ILMerge.Tools.MSBuildToolTask and the options you choose to configure, it should work fine, even against future versions barring any breaking command-line interface changes or future options.


## Dependencies ##
The **ILMerge.Tools.MSBuildToolTask** assembly targets the Microsoft .NET Framework version 4.0 and is dependent upon MSBuild 4.0 or later (e.g. MSBuild 12.0).

The **ILMerge.Tools.MSBuildToolTask** NuGet package does not have any hard dependencies, but to use the MSBuild task, you must supply your own copy of ILMerge.exe and set MSBuild task's ToolPath property with the its directory path.


## Add the task to a project ##
To use the **ILMerge.Tools.MSBuildToolTask** in your MSBuild project, add the following `<UsingTask />` element to your project file:

    <UsingTask
        AssemblyFile="$(SolutionDir)packages\ILMerge.Tools.MSBuildToolTask.2.14.1208\tools\ILMerge.Tools.MSBuildToolTask.dll"
        TaskName="ILMerge.Tools.MSBuildToolTask.ILMergeTool"
        />

The value of the `AssemblyFile` attribute above is the full path and file name of the **ILMerge.Tools.MSBuildToolTask.dll** assembly file and the example assumes your have the `SolutionDir` property configured and that the `packages` directory resides under the solution directory and that you have installed the **ILMerge.Tools.MSBuildToolTask.2.14.1208** NuGet package. You must adjust the value as necessary for your project.

The value of the `TaskName` attribute above is the full namespace and type name of the task.  

## Minimum required properties ##
To use **ILMerge.Tools.MSBuildToolTask**, you must set the following minimum required properties:

* **InputAssemblies** - A `TaskItem` array containing at least two items that represent the file names of the assemblies to be merged. The first assembly in the array is the primary assembly.
* **OutputFile** - The path and file name of the resulting output assembly, such as `C:\MergedAssemblies\MyMergedAssembly.dll`.
* **ToolPath** - Either the full path of the directory that contains the ILMerge executable or the full path and file name of the ILMerge executable.

Additionally, if your copy of ILMerge has a different file name than the default, **ILMerge.exe**, you must either set the **ToolExe** property with the file name (without any path, e.g. MyRenamedILMerge.exe) or set the **ToolPath** property to the full directory path and file name.



## Example Use ##
Here is an example excerpt from an MSBuild project file that sets up the **ILMergeTool** task for use.

    <UsingTask
        AssemblyFile="$(SolutionDir)packages\ILMerge.Tools.MSBuildToolTask.2.14.1208\tools\ILMerge.Tools.MSBuildToolTask.dll"
        TaskName="ILMerge.Tools.MSBuildToolTask.ILMergeTool"
        />

	<ItemGroup>
		<ILMergeInputAssemblies Include="$(OutDir)MyPrimaryAssembly.dll" />
		<ILMergeInputAssemblies Include="$(OutDir)MyDependencyAssembly.dll" />
		<ILMergeInputAssemblies Include="$(OutDir)MyOtherDependencyAssembly.dll" />
	</ItemGroup>
	
	<Target Name="MergeAssemblies">
		<ILMergeTool
			InputAssemblies="@(ILMergeInputAssemblies)"
			OutputFile="$(MergedOutDir)MyPrimaryAssembly.dll"
			ToolPath="$(SolutionDir)packages\ILMerge.Tools.2.14.1208\tools\ILMerge.exe"
			/>
	</Target>

The **ILMergeInputAssemblies** item group defines three assemblies to be merged where the first assembly defined in the list, `MyPrimaryAssembly.dll` is the primary assembly. The example assumes that you have the `OutDir` property has been defined and initialized. 

A new target is defined named **MergeAssemblies**. In the example, this target is not called, but this target would normally be called toward the end of the build process after the build and test runs have completed successfully (i.e. during the packaging stage).

The **ILMergeTool** task is configured with the minimum required properties. The example assumes that the `MergedOutDir` property has been defined and initialized, which is the path to the directory that will contain the merged assembly output file.

The **ToolPath** property value assumes that the `SolutionDir` property has been defined and initialized and that the `packages` directory resides within it. The example points to the location of the ILMerge executable that was installed by the **ILMerge.Tools** NuGet package (a separate package). It is important to note that the **ILMerge.Tools** NuGet package is **not** required and that the ToolPath may point to any valid, compatible instance of ILMerge that you have installed by any means (although using the **ILMerge.Tools** NuGet package is not a bad idea).

