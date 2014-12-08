namespace ILMerge.Tools.MSBuildToolTask.Tests.TestAssembly2
{
	public class DependencyTestClass
    {
		public DependencyTestClass()
		{
			MyDependencyText = "This is the default dependency text value.";
		}

		public string MyDependencyText { get; set; }
    }
}
