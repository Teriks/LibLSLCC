using System.Configuration;
using System.Linq;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.Compilers;
using LibLSLCC.Utility;
using LSLCCEditor.EditControl;
using LSLCCEditor.Utility;

namespace LSLCCEditor.Settings
{
    /// <summary>
    /// The settings node that gets serialized by <see cref="AppSettings.Load"/>
    /// All members should have a publicly accessible constructor with no parameters, and be friendly to XmlSerializer
    /// </summary>
    public class SettingsNode : SettingsBaseClass
    {
        private XmlDictionary<string, CompilerSettingsNode> _compilerConfigurations;
        private XmlDictionary<string, EditorControlSettingsNode> _editorControlConfigurations;
        private string _currentEditorControlConfiguration;
        private string _currentCompilerConfiguration;





        private const string _clientSideScriptCompilerHeader =
@"//c#
/** 
*  Do not remove //c# from the first line of this script.
*
*  This is OpenSim CSharp code, CSharp scripting must be enabled on the server to run.
*
*  Please note this script does not support being reset, because a constructor was not generated.
*  Compile using the server side script option to generate a script constructor.
*
*  This code will run on an unmodified OpenSim server, however script resets will not reset global variables,
*  and OpenSim will be unable to save the state of this script as its global variables are created in an object container.
*
*/ 
";


        private const string _serverSideScriptCompilerHeader =
@"//c#-raw
/** 
*  Do not remove //c#-raw from the first line of this script.
*
*  This is OpenSim CSharp code, CSharp scripting must be enabled on the server to run.
*
*  This is a server side script.  It constitutes a fully generated script class that
*  will be sent to the CSharp compiler in OpenSim.  This code supports script resets.
*
*  This script is meant to upload compatible with the LibLSLCC OpenSim fork.
*
*  If you are running a version of OpenSim with the LibLSLCC compiler enabled, you must add 'csraw'
*  to the allowed list of compiler languages under [XEngine] for this script to successfully upload.
*
*  Adding 'csraw' to your allowed language list when using the old OpenSim compiler will have no effect
*  besides an error being written to your log file.  OpenSim will run but you will not actually be able
*  to use the 'csraw' upload type.
*
*  Note that you can also set 'CreateClassWrapperForCSharpScripts' to 'false' under the [LibLCLCC]
*  OpenSim.ini config section in order to enable 'csraw' mode uploads for every CSharp script sent to the 
*  LibLSLCC compiler;  Including those marked with '//c#' if you have 'cs' in your list of allowed languages.
*
*/ 
";



        private class EditorControlConfigurationsDefaultFactory : IDefaultSettingsValueFactory
        {
            public bool CheckForNecessaryResets(object settingsNode, object propertyValues)
            {
                if (settingsNode == null) return true;

                var dict = (XmlDictionary<string, EditorControlSettingsNode>)propertyValues;


                if (dict == null || dict.Count == 0)
                {
                    return true;
                }

                foreach (var kvp in dict.ToList())
                {
                    if (kvp.Value != null) continue;

                    var initNode = new EditorControlSettingsNode();
                    dict[kvp.Key] = initNode;
                    AppSettings.InitNullSettingsProperties(initNode);
                }

                return false;
            }

            public object GetDefaultValue(object settingsNode)
            {
                var d = new XmlDictionary<string, EditorControlSettingsNode>();

                

                d.Add("Default", new EditorControlSettingsNode() {EditorControlSettings = new LSLEditorControlSettings()});

                return d;
            }
        }


        [DefaultValueFactory(typeof (EditorControlConfigurationsDefaultFactory))]
        public XmlDictionary<string, EditorControlSettingsNode> EditorControlConfigurations
        {
            get { return _editorControlConfigurations; }
            set { SetField(ref _editorControlConfigurations,value, "EditorControlConfigurations"); }
        }


        private class CurrentEditorConfigurationDefaultFactory : IDefaultSettingsValueFactory
        {
            public bool CheckForNecessaryResets(object settingsNode, object propertyValue)
            {
                if (propertyValue == null) return true;

                var val = propertyValue.ToString();


                var settingsNodeInstance = (SettingsNode) settingsNode;

                if (!settingsNodeInstance.EditorControlConfigurations.ContainsKey(val))
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(val))
                {
                    return true;
                }

                return false;
            }

            public object GetDefaultValue(object settingsNode)
            {
                var settingsNodeInstance = (SettingsNode)settingsNode;

                if (settingsNodeInstance.EditorControlConfigurations.ContainsKey("Default"))
                {
                    return "Default";
                }
                return settingsNodeInstance.EditorControlConfigurations.First().Key;
            }
        }


        [DefaultValueFactory(typeof (CurrentEditorConfigurationDefaultFactory))]
        public string CurrentEditorControlConfiguration
        {
            get { return _currentEditorControlConfiguration; }
            set { SetField(ref _currentEditorControlConfiguration, value, "CurrentEditorControlConfiguration"); }
        }


        private class CompilerConfigurationsDefaultFactory : IDefaultSettingsValueFactory
        {
            public bool CheckForNecessaryResets(object settingsNode, object propertyValue)
            {
                if (propertyValue == null) return true;

                var dict = (XmlDictionary<string, CompilerSettingsNode>)propertyValue;


                if (dict == null || dict.Count == 0)
                {
                    return true;
                }

                foreach (var kvp in dict.ToList())
                {
                    if (kvp.Value != null) continue;

                    var initNode = new CompilerSettingsNode();
                    dict[kvp.Key] = initNode;
                    AppSettings.InitNullSettingsProperties(initNode);
                }

                return false;
            }

            public object GetDefaultValue(object settingsNode)
            {
                var d = new XmlDictionary<string, CompilerSettingsNode>();


                var clientCode = new CompilerSettingsNode
                {
                    OpenSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimClientUploadable()
                };

                clientCode.OpenSimCompilerSettings.ScriptHeader = _clientSideScriptCompilerHeader;
                d.Add("OpenSim Client Code", clientCode);


                var clientCodeCoOp = new CompilerSettingsNode
                {
                    OpenSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimClientUploadable()
                };

                clientCodeCoOp.OpenSimCompilerSettings.ScriptHeader = _clientSideScriptCompilerHeader;
                clientCodeCoOp.OpenSimCompilerSettings.InsertCoOpTerminationCalls = true;


                d.Add("OpenSim Client Code (co-op Stop)", clientCodeCoOp);


                var serverCode = new CompilerSettingsNode
                {
                    OpenSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimServerSideDefault()
                };

                serverCode.OpenSimCompilerSettings.ScriptHeader = _serverSideScriptCompilerHeader;
                d.Add("OpenSim Server Code", serverCode);



                var serverCodeCoOp = new CompilerSettingsNode
                {
                    OpenSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimServerSideDefault()
                };

                serverCodeCoOp.OpenSimCompilerSettings.ScriptHeader = _serverSideScriptCompilerHeader;
                serverCodeCoOp.OpenSimCompilerSettings.InsertCoOpTerminationCalls = true;


                d.Add("OpenSim Server Code (co-op Stop)", serverCodeCoOp);


                return d;
            }
        }


        [DefaultValueFactory(typeof (CompilerConfigurationsDefaultFactory))]
        public XmlDictionary<string, CompilerSettingsNode> CompilerConfigurations
        {
            get { return _compilerConfigurations; }
            set { SetField(ref _compilerConfigurations, value, "CompilerConfigurations"); }
        }


        private class CurrentCompilerConfigurationDefaultFactory : IDefaultSettingsValueFactory
        {
            public bool CheckForNecessaryResets(object settingsNode, object propertyValue)
            {
                if (propertyValue == null) return true;

                var val = propertyValue.ToString();

                var settingsNodeInstance = (SettingsNode) settingsNode;

                if (!settingsNodeInstance.CompilerConfigurations.ContainsKey(val))
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(val))
                {
                    return true;
                }

                return false;
            }

            public object GetDefaultValue(object settingsNode)
            {
                var settingsNodeInstance = (SettingsNode)settingsNode;

                if (settingsNodeInstance.EditorControlConfigurations.ContainsKey("OpenSim Client Code"))
                {
                    return "OpenSim Client Code";
                }
                return settingsNodeInstance.CompilerConfigurations.First().Key;
            }
        }


        [DefaultValueFactory(typeof (CurrentCompilerConfigurationDefaultFactory))]
        public string CurrentCompilerConfiguration
        {
            get { return _currentCompilerConfiguration; }
            set { SetField(ref _currentCompilerConfiguration, value, "CurrentCompilerConfiguration"); }
        }
    }
}