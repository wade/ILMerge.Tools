How to Build
------------

MSBuild is used to perform the build.
The MSBuild project file is build.proj.
There is a batch file to simplify running a build named build.bat.

To run a Release build, execute:
> build.bat

To run a Release build and run tests, execute:
> build.bat Release true

To run a Debug build, execute:
> build.bat Debug

To run a Release build and run tests, execute:
> build.bat Debug true


Build Output
------------

All build output is placed in the /.build/out/<config> directory, where the <config> token 
is the build configuration, usually either Debug or Release.

- The /.build/out/<config>/bin directory contains the core compiled binaries and pdb files.

- The /.build/out/<config>/bindep directory contains the compiled binaries, 
  their pdb files and all dependency files.

- The /.build/out/<config>/projects directory contains a subdirectory for each project 
  that was built which in turn contains the build artifacts specific to that project.
  For test projects, this is the directory under which tests are executed which 
  reside in that test project.

- The /.build/out/<config>/TestResult.xml is the test output XML file produced by XUnit.
