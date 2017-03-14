using System;
using System.CodeDom.Compiler;

namespace plugEditor
{
    public class CompileCode
    {
        #region Compiler

        public enum Languages
        {
            VB,
            CSharp
        }

        public static CompilerResults CompileScript(string source, string assembly, Languages Language)
        {
            CodeDomProvider provider = null;

            switch (Language)
            {
                case Languages.VB:
                    provider = new Microsoft.VisualBasic.VBCodeProvider();
                    break;
                case Languages.CSharp:
                    provider = new Microsoft.CSharp.CSharpCodeProvider();
                    break;
            }

            return Compile(source, assembly, provider);
        }

        public static CompilerResults Compile(string source, string assembly, CodeDomProvider provider)
        {
            ICodeCompiler compiler = provider.CreateCompiler();
            CompilerParameters parms = new CompilerParameters();
            CompilerResults results;

            // Configure parameters
            parms.GenerateExecutable = false;
            parms.GenerateInMemory = true;
            parms.IncludeDebugInformation = false;
            parms.ReferencedAssemblies.AddRange(new string[]{
                "System.dll",
                "System.Data.dll",
                "System.Drawing.dll",
                "System.Windows.Forms.dll",
                "control.dll",
                "pluginfo.dll"
            });

            string dir = System.Windows.Forms.Application.StartupPath + @"\plugins\";
            System.IO.Directory.CreateDirectory(dir + assembly);
            parms.OutputAssembly = dir + assembly + @"\" + assembly + ".dll";

            // Compile
            results = compiler.CompileAssemblyFromSource(parms, source);

            return results;
        }

        #endregion
    }
}