using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ILMerge.Tools.MSBuildToolTask
{
	/// <summary>
	/// MSBuild task that executes the ILMerge.exe as a command-line tool to merge multiple .NET assemblies into a single assembly.
	/// </summary>
	/// <remarks>
	/// Because the ILMergeTool MSBuild task calls the ILMerge.exe as a command-line tool,
	/// it is decoupled from the ILMerge assembly which allows this task to be used with
	/// multiple versions of ILMerge without necessarily having to rebuild this library.
	/// Future changes to ILMerge.exe that may cause ILMergeTool to be rebuilt would be
	/// the addition of new command-line arguments or any breaking changes to existing arguments.
	/// <para>The <see cref="ToolTask.ToolPath"/> property must be set to the path and file name of the ILMerge executable file (i.e. ILMerge.exe).</para>
	/// </remarks>
    public class ILMergeTool : ToolTask
	{
		private const int DefaultFileAlignment = 512;

		#region " Default Constructor "

		/// <summary>
		/// Initializes a new instance of the <see cref="ILMergeTool"/> class.
		/// </summary>
		public ILMergeTool()
		{
			// Set default property values.
			AllowMultipleAssemblyLevelAttributes = false;
			AllowWildCards = false;
			AllowZeroPeKind = false;
			AttributeFile = null;
			Closed = false;
			CopyAttributes = false;
			DebugInfo = true;
			DelaySign = false;
			DuplicateTypeNames = null;
			ExcludeFile = string.Empty;
			FileAlignment = DefaultFileAlignment;
			InputAssemblies = null;
			Internalize = false;
			KeyFile = null;
			LogFile = null;
			OutputFile = null;
			PublicKeyTokens = true;
			SearchDirectories = null;
			_toolExe = "ILMerge.exe";
			_toolPath = null;
			TargetPlatformDirectory = null;
			TargetPlatformVersion = null;
			TargetType = string.Empty;
			UnionMerge = false;
			Version = null;
			XmlDocumentation = false;
		}

		#endregion " Default Constructor "

		#region " ILMerge Properties "

		/// <summary>
		/// Gets or sets a value indicating whether duplicate type names are allowed.
		/// </summary>
		/// <value>
		/// <c>true</c> if duplicate type names are allowed; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// The normal behavior of ILMerge is to not allow there to be more than one public type with the same name.
		/// If such a duplicate is found, then an exception is thrown. However, ILMerge can just rename the type so
		/// that it no longer causes a conflict. For private types, this is not a problem since no outside client can
		/// see it anyway, so ILMerge just does the renaming by default. For public types, it is not often a useful
		/// feature to have it renamed. However, there are situations where it is needed. In particular, for obfuscated
		/// assemblies, it seems that the obfuscator defines an attribute and creates an assembly-level attribute for
		/// the obfuscated assembly using that attribute. This would mean that obfuscated assemblies cannot be merged.
		/// So this option allows the user to either allow all public types to be renamed when they are duplicates,
		/// or to specify it for arbitrary type names.
		/// <para>
		///		If this property is set to <c>true</c> and the <see cref="DuplicateTypeNames"/> property is null or empty,
		///		the "/allowDup" switch with no type names is specified on the command-line.
		///		If there is one or more values in the <see cref="DuplicateTypeNames"/> property, the "/allowDup:typeName"
		///		switch is specified for each type name regardless of the <see cref="AllowDuplicateTypeNames"/> property value.
		/// </para>
		/// <para>Default: <c>false</c></para>
		/// <para>Command line option: /allowDup</para>
		/// </remarks>
		public bool AllowDuplicateTypeNames { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether any assembly-level attributes names that have the same type are copied over
		/// into the target directory as long as the definition of the attribute type specifies that "AllowMultiple" is true.
		/// </summary>
		/// <value>
		/// <c>true</c> if any assembly-level attributes names that have the same type are copied over
		/// into the target directory as long as the definition of the attribute type specifies that "AllowMultiple" is true; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// When this is set before calling Merge, then if the <see cref="CopyAttributes"/> property is also set,
		/// any assembly-level attributes names that have the same type are copied over into the target directory
		/// as long as the definition of the attribute type specifies that "AllowMultiple" is true.
		/// <para>Default: <c>false</c></para>
		/// <para>Command line option: /allowMultiple</para>
		/// </remarks>
		public bool AllowMultipleAssemblyLevelAttributes { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether any wild cards in file names are expanded and all matching files will be used as input.
		/// </summary>
		/// <value>
		///   <c>true</c> if any wild cards in file names are expanded and all matching files will be used as input; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// When this is set before calling Merge, any wild cards in file names are expanded and all matching files will be used as input.
		/// Note that because the wild card matching is done by a call to Directory.GetFiles, it does not allow the characters ".."
		/// to appear in a file name. So if you want to specify a relative path containing ".." to move up a directory,
		/// you will have to use it with the "/lib" option (i.e. the <see cref="SearchDirectories"/> property).
		/// That option does allow the use of ".." to move up directories.
		/// <para>Default: <c>false</c></para>
		/// <para>Command line option: /wildcards</para>
		/// </remarks>
		public bool AllowWildCards { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether an assembly's PeKind flag is zero it will be treated as if it was ILonly.
		/// </summary>
		/// <value>
		///   <c>true</c> if an assembly's PeKind flag is zero it will be treated as if it was ILonly; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// When this is set before calling Merge, then if an assembly's PeKind flag
		/// (this is the value of the field listed as .corflags in the Manifest)
		/// is zero it will be treated as if it was ILonly. This can be used to allow
		/// C++ assemblies to be merged; it does not appear that the C++ compiler writes the value as ILonly.
		/// However, if such an assembly has any non-IL features, then they will probably not be copied over
		/// into the target assembly correctly. So please use this option with caution.
		/// <para>Default: <c>false</c></para>
		/// <para>Command line option: /zeroPeKind</para>
		/// </remarks>
		public bool AllowZeroPeKind { get; set; }

		/// <summary>
		/// Gets or sets the attribute file.
		/// </summary>
		/// <value>
		/// The attribute file.
		/// </value>
		/// <remarks>
		/// If this is set before calling Merge, then it specifies the path and filename to an atttribute assembly,
		/// an assembly that will be used to get all of the assembly-level attributes such as Culture, Version, etc.
		/// It will also be used to get the Win32 Resources from. It is mutually exclusive with the CopyAttributes property.
		/// When it is not specified, then the Win32 Resources from the primary assembly are copied over into the target assembly.
		/// If it is not a full path, then the current directory is used.
		/// <para>Default: <c>null</c></para>
		/// <para>Command line option: /attr:filename</para>
		/// </remarks>
		public string AttributeFile { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the "transitive closure" of the input assemblies is computed and added to the list of input assemblies.
		/// </summary>
		/// <value>
		///   <c>true</c> if the "transitive closure" of the input assemblies is computed and added to the list of input assemblies.; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// When this is set before calling Merge, then the "transitive closure" of the input assemblies is computed and added to the list of input assemblies.
		/// An assembly is considered part of the transitive closure if it is referenced, either directly or indirectly, from one of the originally specified
		/// input assemblies and it has an external reference to one of the input assemblies, or one of the assemblies that has such a reference.
		/// Complicated, but that is life...
		/// <para>Default: <c>false</c></para>
		/// <para>Command line option: /closed</para>
		/// </remarks>
		public bool Closed { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the assembly level attributes of each input assembly are copied over into the target assembly.
		/// </summary>
		/// <value>
		///   <c>true</c> if the assembly level attributes of each input assembly are copied over into the target assembly.; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// When this is set before calling Merge, then the assembly level attributes of each input assembly are copied over into the target assembly.
		/// Any duplicate attribute overwrites a previously copied attribute.
		/// If you want to allow duplicates (for those attributes whose type specifies "AllowMultiple" in their definition),
		/// then you can also set the <see cref="AllowMultipleAssemblyLevelAttributes"/>.
		/// The input assemblies are processed in the order they are specified.
		/// This option is mutually exclusive with specifying an attribute assembly, i.e., the property AttributeFile (Section 2.5).
		/// When an attribute assembly is specified, then no assembly-level attributes are copied over from the input assemblies.
		/// <para>Default: <c>false</c></para>
		/// <para>Command line option: /copyattrs</para>
		/// </remarks>
		public bool CopyAttributes { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a .pdb file is created for the output assembly and merges into it any .pdb files found for input assemblies.
		/// </summary>
		/// <value>
		///   <c>true</c> if a .pdb file is created for the output assembly and merges into it any .pdb files found for input assemblies.; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// When this is set to true, ILMerge creates a .pdb file for the output assembly and merges into it any .pdb files found for input assemblies.
		/// If you do not want a .pdb file created for the output assembly, either set this property to false or else specify the /ndebug option at the command line.
		/// <para>Default: <c>true</c></para>
		/// <para>Command line option: /ndebug</para>
		/// </remarks>
		public bool DebugInfo { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the target assembly should be delay signed.
		/// </summary>
		/// <value>
		///   <c>true</c> if target assembly should be delay signed; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// This can be set only in conjunction with the <see cref="KeyFile"/> property.
		/// <para>Default: <c>false</c></para>
		/// <para>Command line option: /delaysign</para>
		/// </remarks>
		public bool DelaySign { get; set; }

		/// <summary>
		/// Gets or sets the duplicate type names.
		/// </summary>
		/// <value>
		/// The duplicate type names.
		/// </value>
		/// <remarks>
		/// The normal behavior of ILMerge is to not allow there to be more than one public type with the same name.
		/// If such a duplicate is found, then an exception is thrown. However, ILMerge can just rename the type so
		/// that it no longer causes a conflict. For private types, this is not a problem since no outside client can
		/// see it anyway, so ILMerge just does the renaming by default. For public types, it is not often a useful feature
		/// to have it renamed. However, there are situations where it is needed. In particular, for obfuscated assemblies,
		/// it seems that the obfuscator defines an attribute and creates an assembly-level attribute for the obfuscated
		/// assembly using that attribute. This would mean that obfuscated assemblies cannot be merged.
		/// <para>
		///		So this option allows the user to either allow all public types to be renamed when they are duplicates,
		///		or to specify it for arbitrary type names. On the command line, there can be as many options as desired,
		///		if followed by a colon and a type name, otherwise just specify it alone with no colon (and type name) to
		///		allow all duplicates.
		/// </para>
		/// <para>When used through the API, an argument of "null" means to allow all public types to be renamed.</para>
		/// <para>Default: <c>null</c> (no duplicates of public types allowed)</para>
		/// <para>Command line option: [/allowDup[:typeName]]*</para>
		/// </remarks>
		public ITaskItem[] DuplicateTypeNames { get; set; }

		/// <summary>
		/// Gets or sets the path and filename that will be used to identify types that are not to have their visibility modified.
		/// </summary>
		/// <value>
		/// The path and filename that will be used to identify types that are not to have their visibility modified.
		/// </value>
		/// <remarks>
		/// This property is used only in conjunction with the <see cref="Internalize"/> property.
		/// When this is set before calling Merge, it indicates the path and filename that will be used to identify types that are not to have their visibility modified.
		/// If Internalize is true, but ExcludeFile is "", then all types in any assembly other than the primary assembly are made non-public.
		/// Setting this property implicitly sets Internalize to true.
		/// <para>
		///		The contents of the file should be one regular expression per line. The syntax is that defined in
		///		the .NET namespace System.Text.RegularExpressions for regular expressions.
		///		The regular expressions are matched against each type's full name, e.g., "System.Collections.IList".
		///		If the match fails, it is tried again with the assembly name (surrounded by square brackets) prepended to the type name.
		///		Thus, the pattern “\[A\].*” excludes all types in assembly A from being made non-public.
		///		(The backslashes are required because the string is treated as a regular expression.)
		///		The pattern “N.T” will match all types named T in the namespace named N no matter what assembly they are defined in.
		/// </para>
		/// <para>
		///		It is important to note that the regular expressions are not anchored to the beginning of the string;
		///		if this is desired, use the appropriate regular expression operator characters to do so.
		/// </para>
		/// <para>Default: <c>""</c></para>
		/// <para>Command line option: /internalize[:excludeFile]</para>
		/// </remarks>
		public string ExcludeFile { get; set; }

		/// <summary>
		/// Gets or sets the file alignment used for the target assembly.
		/// </summary>
		/// <value>
		/// The file alignment used for the target assembly.
		/// </value>
		/// <remarks>
		/// This controls the file alignment used for the target assembly.
		/// The setter sets the value to the largest power of two that is no larger than the supplied argument, and is at least 512.
		/// <para>Default: <c>512</c></para>
		/// <para>Command line option: /align:n</para>
		/// </remarks>
		public int FileAlignment { get; set; }

		/// <summary>
		/// Gets or sets the input assemblies.
		/// </summary>
		/// <value>
		/// The input assemblies.
		/// </value>
		/// <remarks>
		/// Each item should be the path and filename of an input assembly.
		/// The first element of the array is the primary assembly.
		/// <para>Default: <c>null</c></para>
		/// <para>Command line option: <c>&lt;primary assembly&gt; [&lt;other assemblies&gt;...]</c></para>
		/// </remarks>
		public ITaskItem[] InputAssemblies { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether types in assemblies other than the primary assembly have their visibility modified.
		/// </summary>
		/// <value>
		///   <c>true</c> if all non-exempt types that are visible outside of their assembly have their visibility
		///   modified so that they are not visible from outside of the merged assembly; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// This controls whether types in assemblies other than the primary assembly have their visibility modified.
		/// When it is true, then all non-exempt types that are visible outside of their assembly have their visibility
		/// modified so that they are not visible from outside of the merged assembly.
		/// A type is exempt if its full name matches a line from the <see cref="ExcludeFile"/> using the .NET regular expression engine.
		/// <para>Default: <c>false</c></para>
		/// <para>Command line option: /internalize[:excludeFile]</para>
		/// </remarks>
		public bool Internalize { get; set; }

		/// <summary>
		/// Gets or sets the path and filename to a .snk file.
		/// </summary>
		/// <value>
		/// The path and filename to a .snk file.
		/// </value>
		/// <remarks>
		/// The target assembly will be signed with its contents and will then have a strong name.
		/// It can be used with the <see cref="DelaySign"/> property to have the target assembly delay signed.
		/// This can be done even if the primary assembly was fully signed.
		/// <para>Default: <c>null</c></para>
		/// <para>Command line option: /keyfile:filename</para>
		/// </remarks>
		public string KeyFile { get; set; }

		/// <summary>
		/// Gets or sets the log file path and file name to which log messages are written.
		/// </summary>
		/// <value>
		/// The log file.
		/// </value>
		/// <remarks>
		/// When this is set before calling Merge, it indicates the path and filename that log messages are written to.
		/// If Log is true, but LogFile is null, then log messages are written to Console.Out.
		/// <para>Default: <c>null</c></para>
		/// <para>Command line option: /log[:logfile]</para>
		/// </remarks>
		public string LogFile { get; set; }

		/// <summary>
		/// Gets or sets the output file.
		/// </summary>
		/// <value>
		/// The output file.
		/// </value>
		/// <remarks>
		/// It specifies the path and filename that the target assembly will be written to.
		/// <para>Default: <c>null</c></para>
		/// <para>Command line option: /out:filename</para>
		/// </remarks>
		public string OutputFile { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether external assembly references in the manifest of the target assembly will use full public keys (false) or public key tokens (true).
		/// </summary>
		/// <value>
		///   <c>true</c> if external assembly references in the manifest of the target assembly will use public key tokens;
		///	  otherwise, <c>false</c> if external assembly references in the manifest of the target assembly will use full public keys.
		/// </value>
		/// <remarks>
		/// <para>Default: <c>true</c></para>
		/// <para>Command line option: /useFullPublicKeyForReferences</para>
		/// </remarks>
		public bool PublicKeyTokens { get; set; }

		/// <summary>
		/// Gets or sets the search directories to be used to search for input assemblies.
		/// </summary>
		/// <value>
		/// The search directories to be used to search for input assemblies.
		/// </value>
		/// <remarks>
		/// The SearchDirectories parameter property is optional.
		/// <para>Default: <c>null</c></para>
		/// <para>Command line option: /lib:directory</para>
		/// </remarks>
		public ITaskItem[] SearchDirectories { get; set; }

		/// <summary>
		/// Gets or sets the target platform directory path in which mscorlib.dll is located.
		/// </summary>
		/// <value>
		/// The target platform directory path in which mscorlib.dll is located.
		/// </value>
		/// <remarks>
		/// The directory in which mscorlib.dll is to be found for the version of the .NET Framework of the target assembly.
		/// This property must be set in conjunction with the <see cref="TargetPlatformVersion"/> property.
		/// <para>Default: <c>null</c></para>
		/// <para>Command line option: /targetplatform:version,platformdirectory</para>
		/// </remarks>
		public string TargetPlatformDirectory { get; set; }

		/// <summary>
		/// Gets or sets the target platform version.
		/// </summary>
		/// <value>
		/// The target platform version.
		/// </value>
		/// <remarks>
		/// The .NET Framework version of the target assembly.
		///	Valid values are "v1", "v1.1", "v2", and "v4".
		/// The "v" is case insensitive and is also optional.
		/// This way ILMerge can be used to "cross-compile", i.e., it can run in one version of the
		/// framework and generate the target assembly so it will run under a different assembly.
		/// <para>Default: <c>null</c></para>
		/// <para>Command line option: /targetplatform:version,platformdirectory</para>
		/// </remarks>
		public string TargetPlatformVersion { get; set; }

		/// <summary>
		/// Gets or sets the target assembly type that is created either as a library, a console application or as a Windows application.
		/// </summary>
		/// <value>
		/// The target assembly type that is created either as a library, a console application or as a Windows application.
		/// </value>
		/// <remarks>
		/// This controls whether the target assembly is created as a library, a console application or as a Windows application.
		/// When it is not specified (null or empty string), then the target assembly will be the same kind as that of the primary assembly.
		/// (In that case, the file extensions found on the specified target assembly and the primary assembly must match.)
		/// When it is specified, then the file extension of the target assembly must match the specification.
		/// <para>
		///		The valid values that the <see cref="TargetType"/> property may be set to are: "library", "exe" or "winexe".
		/// </para>
		/// <para>Default: <c>""</c></para>
		/// <para>Command line option: /target:(library|exe|winexe)</para>
		/// </remarks>
		public string TargetType { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether types with the same name are all merged into a single type in the target assembly.
		/// </summary>
		/// <value>
		///   <c>true</c> if types with the same name are all merged into a single type in the target assembly; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// When this is true, then types with the same name are all merged into a single type in the target assembly.
		/// The single type is the union of all of the individual types in the input assemblies:
		/// it contains all of the members from each of the corresponding types in the input assemblies.
		/// It cannot be specified at the same time as /allowDup.
		/// <para>Default: <c>false</c></para>
		/// <para>Command line option: /union</para>
		/// </remarks>
		public bool UnionMerge { get; set; }

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		/// <remarks>
		/// When this has a non-null value, then the target assembly will be given its value as the version number of the assembly.
		/// When specified on the command line, the version is read in as a string and should look like "6.2.1.3" (but without the quote marks).
		/// The version must be a valid assembly version as defined by the attribute AssemblyVersion in the System.Reflection namespace.
		/// <para>Default: <c>null</c></para>
		/// <para>Command line option: /ver:version</para>
		/// </remarks>
		public string Version { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether XML documentation files are merged to produce an XML documentation file for the target assembly.
		/// </summary>
		/// <value>
		///   <c>true</c> if XML documentation files are merged to produce an XML documentation file for the target assembly; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// This property controls whether XML documentation files are merged to produce an XML documentation file for the target assembly.
		/// <para>Default: <c>false</c></para>
		/// <para>Command line option: /xmldocs</para>
		/// </remarks>
		public bool XmlDocumentation { get; set; }

		#endregion " ILMerge Properties "

		#region " Overridden Members "

		/// <summary>
		/// Returns the fully qualified path to the executable file.
		/// </summary>
		/// <returns>
		/// The fully qualified path to the executable file.
		/// </returns>
	    protected override string GenerateFullPathToTool()
	    {
			// It seems that the base class, ToolTask, ignores the return value of this abstract method
			// and simply performs a Path.Combine(ToolPath, ToolExe);
		    return Path.Combine(ToolPath, ToolExe);
	    }

		/// <summary>
		/// Returns a string value containing the command line arguments to pass directly to the executable file.
		/// </summary>
		/// <returns>
		/// A string value containing the command line arguments to pass directly to the executable file.
		/// </returns>
		protected override string GenerateCommandLineCommands()
		{
			const string errorReturnValue = "";

			if (false == ValidateInputAssemblies() ||
				false == ValidateOutputFile())
				return errorReturnValue;


			#region " ILMerge.exe Command-Line Arguments "
			/*
			 * [/lib:directory]*
			 * [/log[:filename]]
			 * [/keyfile:filename [/delaysign]]
			 * [/internalize[:filename]]
			 * [/t[arget]:(library|exe|winexe)]
			 * [/closed]
			 * [/ndebug]
			 * [/ver:version]
			 * [/copyattrs [/allowMultiple]]
			 * [/xmldocs]
			 * [/attr:filename]
			 * ([/targetplatform:<version>[,<platformdir>]]|v1|v1.1|v2|v4)
			 * [/useFullPublicKeyForReferences]
			 * [/zeroPeKind]
			 * [/wildcards]
			 * [/allowDup[:typename]]*
			 * [/allowDuplicateResources]
			 * [/union]
			 * [/align:n]
			 * /out:filename
			 * <primary assembly> [<other assemblies>...]
			 */
			#endregion " ILMerge.exe Command-Line Arguments "

			var builder = new CommandLineBuilder();

			if (null != DuplicateTypeNames && DuplicateTypeNames.Length > 0)
			{
				foreach (var item in DuplicateTypeNames)
				{
					var typeName = item.ItemSpec;
					builder.AppendSwitch(string.Format("/allowDup:{0}", typeName.Trim()));
				}
			}
			else if (AllowDuplicateTypeNames)
			{
				builder.AppendSwitch("/allowDup");
			}

			if (AllowMultipleAssemblyLevelAttributes)
				builder.AppendSwitch("/allowMultiple");

			if (AllowWildCards)
				builder.AppendSwitch("/wildcards");

			if (AllowZeroPeKind)
				builder.AppendSwitch("/zeroPeKind");

			if (false == string.IsNullOrWhiteSpace(AttributeFile))
				builder.AppendSwitch(string.Format("/attr:\"{0}\"", AttributeFile.Trim()));

			if (Closed)
				builder.AppendSwitch("/closed");

			if (CopyAttributes)
				builder.AppendSwitch("/copyattrs");

			if (false == DebugInfo)
				builder.AppendSwitch("/ndebug");

			if (DelaySign)
				builder.AppendSwitch("/delaysign");

			if (FileAlignment != DefaultFileAlignment)
				builder.AppendSwitch(string.Format("/align:{0}", FileAlignment));

			if (Internalize)
				builder.AppendSwitch(string.Format("/internalize{0}", string.IsNullOrWhiteSpace(ExcludeFile) ? "" : string.Format(":\"{0}\"", ExcludeFile.Trim())));

			if (false == string.IsNullOrWhiteSpace(KeyFile))
				builder.AppendSwitch(string.Format("/keyfile:\"{0}\"", KeyFile.Trim()));

			if (false == string.IsNullOrWhiteSpace(LogFile))
				builder.AppendSwitch(string.Format("/log:\"{0}\"", LogFile.Trim()));

			if (false == PublicKeyTokens)
				builder.AppendSwitch("/useFullPublicKeyForReferences");

			if (null != SearchDirectories && SearchDirectories.Length > 0)
			{
				foreach (var item in SearchDirectories)
				{
					var directory = item.ItemSpec;
					builder.AppendSwitch(string.Format("/lib:\"{0}\"", directory.Trim()));
				}
			}

			// Target Platform
			if (false == string.IsNullOrWhiteSpace(TargetPlatformDirectory) &&
			    false == string.IsNullOrWhiteSpace(TargetPlatformVersion))
			{
				var value = string.Format("{0},\"{1}\"", TargetPlatformVersion.Trim().ToLowerInvariant(), TargetPlatformDirectory.Trim());
				builder.AppendSwitch(string.Format("/targetplatform:{0}", value));
			}

			if (false == string.IsNullOrWhiteSpace(TargetType))
				builder.AppendSwitch(string.Format("/target:{0}", TargetType.Trim().ToLowerInvariant()));

			if (UnionMerge)
				builder.AppendSwitch("/union");

			if (XmlDocumentation)
				builder.AppendSwitch("/xmldocs");

			if (false == string.IsNullOrWhiteSpace(Version))
				builder.AppendSwitch(string.Format("/ver:{0}", Version.Trim()));

			// OutputFile is added after all other switches and before the InputAssemblies.
			builder.AppendSwitch(string.Format("/out:\"{0}\"", OutputFile));

			// InputAssemblies must be added after all other switches.
			if (null != InputAssemblies && InputAssemblies.Length > 0)
			{
				foreach (var inputAssemblyPath in InputAssemblies)
				{
					builder.AppendTextUnquoted(string.Format(" \"{0}\"", inputAssemblyPath));
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Projects may set this to override a task's ToolName. Tasks may override this to prevent that.
		/// </summary>
		public override string ToolExe
		{
			get { return _toolExe; }
			set { _toolExe = value; }
		}

		private string _toolExe;

		/// <summary>
		/// Gets or sets the path of the executable file to run.
		/// </summary>
		public new string ToolPath
		{
			get { return _toolPath; }
			set { SetToolPath(value); }
		}

		private string _toolPath;

		/// <summary>
		/// Gets the name of the executable file to run.
		/// </summary>
		protected override string ToolName
		{
			get { return _toolExe; }
		}

		#endregion " Overridden Members "

		private void SetToolPath(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				_toolPath = value;
				return;
			}

			var path = value.Trim();
			var fileAttributes = File.GetAttributes(path);
			if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
			{
				_toolPath = path;
				return;
			}

			_toolExe = Path.GetFileName(path);
			_toolPath = Path.GetDirectoryName(path);
		}

		private bool ValidateInputAssemblies()
		{
			const string inputAssembliesExpectations =
				"The InputAssemblies property is required and must be set to a valid array of TaskItems that represent the path and file name of each input assembly to be merged." +
				"The first assembly in the array is the primary assembly.";

			if (null == InputAssemblies)
			{
				Log.LogError("The InputAssemblies property value is null. It may not have been set. " + inputAssembliesExpectations);
				return false;
			}
			if (InputAssemblies.Length < 1)
			{
				Log.LogError("The InputAssemblies property value is an empty array. " + inputAssembliesExpectations);
				return false;
			}
			return true;
		}

		private bool ValidateOutputFile()
		{
			const string outputFileExpectations =
				"The OutputFile property is required and must be set to a valid file name (and optionally may include a path) of the assembly produced by ILMerge.";

			return ValidateRequiredStringProperty(OutputFile, "OutputFile", outputFileExpectations, Log);
		}

		private static bool ValidateRequiredStringProperty(string value, string propertyName, string propertyExpectations, TaskLoggingHelper log)
		{
			if (null == value)
			{
				log.LogError(string.Format("The {0} property value is null which is not valid. It may not have been set. {1}", propertyName, propertyExpectations));
				return false;
			}
			if (string.Empty == value)
			{
				log.LogError(string.Format("The {0} property value is empty which is not valid. {1}", propertyName, propertyExpectations));
				return false;
			}
			if (string.IsNullOrWhiteSpace(value))
			{
				log.LogError(string.Format("The {0} property value contains only white space characters which is not valid. {1}", propertyName, propertyExpectations));
				return false;
			}
			return true;
		}
	}
}
