using ILMerge.Tools.MSBuildToolTask.Tests.Common;
using ILMerge.Tools.MSBuildToolTask.Tests.TestAssembly2;

namespace ILMerge.Tools.MSBuildToolTask.Tests.TestAssembly1
{
	public class TestClass : ITestClass
    {
		public TestClass()
			: this(new DependencyTestClass())
		{
		}

	    public TestClass(DependencyTestClass dependency)
	    {
		    Dependency = dependency;
	    }

		private DependencyTestClass Dependency { get; set; }

		public string Text { get { return Dependency.MyDependencyText; } }
    }
}
