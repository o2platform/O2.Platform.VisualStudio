﻿using System.Threading;
using O2.FluentSharp.VisualStudio.ExtensionMethods;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;
using FluentSharp.REPL.Utils;

namespace O2.FluentSharp.VisualStudio
{
    public class VisualStudio_O2_Utils
    {
        /*public static void installO2Scripts_IfDoesntExist()
        {
            if (PublicDI.config.LocalScriptsFolder.dirExists().isFalse())
            {
                O2Scripts.downloadO2Scripts();
            }
        
        } */        
        public static Thread compileAndExecuteScript(string scriptFile, string type, string method)
        {
            "[VisualStudio_O2_Utils]: compileAndExecuteScript: {0}!{1}.{2}".debug(scriptFile, type, method);
            addVisualStudioReferencesForCompilation();
            return O2Thread.mtaThread(() =>
            {                
                "[compileAndExecuteScript] Got DTE object".info();
                var file =scriptFile.local(); //pathCombine_With_ExecutingAssembly_Folder();
                if (file.fileExists().isFalse())
                    "[compileAndExecuteScript] could not find script with O2 Platform menu: {0}".error(file);
                else
                {
                    "[compileAndExecuteScript] compiling {0}".info(file);
                    var assembly = file.compile();
                    if (assembly.notNull())
                    {
                        "[compileAndExecuteScript] executing {0}.{1} method".info(type, method);
                        assembly.type(type)
                        .method(method)
                        .invoke();
                    }
                    else
                    {
                        "[compileAndExecuteScript] failed to compile file: {0}".error(file);
                        "[compileAndExecuteScript] opening an o2 Script editor to help debugging the issue".info();
                        file.script_Me("file");
                    }
                }                
            });
        }
        public static void createO2PlatformMenu()
        {
            compileAndExecuteScript(@"VS_Scripts\O2_Menus_In_VisualStudio.cs", "O2_Menus_In_VisualStudio" ,"buildMenus")
                .Join();
        }

        public static bool waitForDTEObject()
        {
            var maxWaitLoops = 10;
            while (maxWaitLoops-- > 0)
            {
                if (VisualStudio_2010.DTE2.notNull())
                    return true;
                "[waitForDTEObject] waiting 500 ms".info()
						.wait(500,false);                
            }
            "[waitForDTEObject] failed to get DTE object after {0} attempts".error(maxWaitLoops);
            return false;
        }
		public static bool waitForOutputWindow()
		{
			var maxWaitLoops = 10;
			while (maxWaitLoops-- > 0)
			{
				VisualStudio_2010.O2_OutputWindow = VisualStudio_2010.DTE2.outputWindow_Create("O2 Platform", false);
				if (VisualStudio_2010.O2_OutputWindow.notNull())
					return true;
				"[waitForOutputWindow] waiting 500 ms".info()
						.wait(500, false);
			}
			"[waitForOutputWindow] failed to get DTE object after {0} attempts".error(maxWaitLoops);
			return false;			
		}
        public static void addVisualStudioReferencesForCompilation()
        {
            CompileEngine.DefaultReferencedAssemblies
                            .add_OnlyNewItems(//needed for VS scripting                                              
                                              "Microsoft.VisualStudio.Shell.10.0.dll",
                                              "Microsoft.VisualStudio.Shell.Interop.dll",
                                              "Microsoft.VisualStudio.Shell.Interop.8.0.dll",
                                              "Microsoft.VisualStudio.Shell.Interop.9.0.dll",
                                              "Microsoft.VisualStudio.Shell.Interop.10.0.dll",
                                              "Microsoft.VisualStudio.Shell.UI.Internal.dll",
                                              "Microsoft.VisualStudio.OLE.Interop.dll",
                                              "Microsoft.VisualStudio.CommandBars.dll",
                                              "Microsoft.VisualStudio.Platform.WindowManagement.dll",
                                              "EnvDTE.dll",
                                              "EnvDTE80.dll",
                                              //needed for VS2015
                                              "Microsoft.VisualStudio.Shell.Interop.dll",
                                              //"EnvDTE80.DTE2.dll",
                                              "Microsoft.VisualStudio.Shell.ViewManager.dll",
                                              //needed for WPF manipulation
                                              "WindowsFormsIntegration.dll",
                                              "Microsoft.VisualStudio.Text.UI.dll",
                                              "Microsoft.VisualStudio.Text.UI.Wpf.dll",
                                              "Microsoft.VisualStudio.CoreUtility.dll",
                                              "Microsoft.VisualStudio.Platform.VSEditor.dll",
                                              "Microsoft.VisualStudio.Text.Data.dll",
                                              "FluentSharp.WPF.dll",
                                              //needed for WPF
                                              "PresentationFramework.dll",
                                              "PresentationCore.dll",
                                              "WindowsBase.dll",
                                              "System.Xaml.dll",
                                              //needed for FluentSharp ExtensionMethods
                                              "O2.Platform.VisualStudio_2010.dll"
                                              );
            CompileEngine.DefaultUsingStatements
                            .add_OnlyNewItems("O2.FluentSharp.VisualStudio",
											  "O2.FluentSharp.VisualStudio.ExtensionMethods",
											  "O2.FluentSharp.VisualStudio.Packages",
                                              "WPF_Media = System.Windows.Media",
                                              "WPF_Controls = System.Windows.Controls");
        }

        public static void open_LogViewer()
        {
            open.logViewer();  
        }
        public static void open_ScriptEditor()
        {
            addVisualStudioReferencesForCompilation();

            var scriptEditor = new VisualStudio_2010().script_Me("visualStudio");

            scriptEditor.Code += ("//O2File:" + "ExtensionMethods/VisualStudio_2010_ExtensionMethods.cs").lineBeforeAndAfter();

//            var scriptEditor = open.scriptEditor();
            
//            scriptEditor.inspector.Code = @"return VisualStudio_2010.Package;";            
        }
    }

/*var defaultCode =
"//testing autoupdate
var ErrorList = VisualStudio_2010.ErrorListProvider;

ErrorList.clear();
ErrorList.add_Error(""You can Errors like this"");
ErrorList.add_Warning(""You can Warnings like this"");
ErrorList.add_Message(""You can Messages like this"");
""This is another error"".add_Error();
""This is another Warning"".add_Warning();
""This is anpther Message"".add_Message();

return VisualStudio_2010.Package;
//O2File:ExtensionMethods/VisualStudio_2010_ExtensionMethods.cs";
scriptEditor.wait(2000);
scriptEditor.inspector.set_Script(defaultCode);
CompileEngine.clearCompilationCache();            
scriptEditor.inspector.compile();
 * */
}
