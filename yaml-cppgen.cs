using Sharpmake;
using System.IO;

[Generate]
public class YamlCPP : Project
{
    public YamlCPP()
    {
        Name = "YamlCPP";
        SourceRootPath = @"[project.SharpmakeCsPath]\src";
        AddTargets(new Target(
            Platform.win32 | Platform.win64,
            DevEnv.vs2022,
            Optimization.Debug | Optimization.Release,
            OutputType.Dll | OutputType.Lib
        ));
    }

    [Configure]
    public void ConfigureAll(Project.Configuration conf, Target target)
    {
        // This is the name of the configuration. By default, it is set to
        // [target.Optimization] (so Debug or Release), but both the debug and
        // release configurations have both a shared and a static version so
        // that would not create unique configuration names.
        conf.Name = @"[target.Optimization] [target.OutputType]";

        // Gives a unique path for the project because Visual Studio does not
        // like shared intermediate directories.
        conf.ProjectPath = Path.Combine("[project.SharpmakeCsPath]/projects/[project.Name]");

        // Sets the include path of the library. Those will be shared with any
        // project that adds this one as a dependency. (The executable here.)
        conf.IncludePaths.Add(@"[project.SourceRootPath]/../include");

        // The library wants LIBRARY_COMPILE defined when it compiles the
        // library, so that it knows whether it must use dllexport or
        // dllimport.
        conf.Defines.Add("YAML_CPP_STATIC_DEFINE");

        if (target.OutputType == OutputType.Dll)
        {
            // We want this to output a shared library. (DLL)
            conf.Output = Configuration.OutputType.Dll;

            // This library project expects LIBRARY_DLL symbol to be defined
            // when used as a DLL. While we could define it in the executable,
            // it is better to put it as an exported define. That way, any
            // projects with a dependency on this one will have LIBRARY_DLL
            // automatically defined by Sharpmake.
            //conf.ExportDefines.Add("LIBRARY_DLL");

            // Exported defines are not necessarily defines as well, so we need
            // to add LIBRARY_DLL as an ordinary define too.
            //conf.Defines.Add("LIBRARY_DLL");
        }   
        else if (target.OutputType == OutputType.Lib)
        {
            // We want this to output a static library. (LIB)
            conf.Output = Configuration.OutputType.Lib;
        }
    }
}