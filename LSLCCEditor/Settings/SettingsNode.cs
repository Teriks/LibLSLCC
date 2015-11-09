using System.Configuration;
using System.Linq;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.Compilers;
using LibLSLCC.Utility;
using LSLCCEditor.EditControl;
using LSLCCEditor.Utility;

namespace LSLCCEditor.Settings
{


    public class CompilerSettingsNode : SettingsBaseClass
    {
        private LSLOpenSimCompilerSettings _openSimCompilerSettings;


        public LSLOpenSimCompilerSettings OpenSimCompilerSettings
        {
            get { return _openSimCompilerSettings; }
            set { SetField(ref _openSimCompilerSettings,value,"OpenSimCompilerSettings"); }
        }
    }



    /// <summary>
    /// The settings node that gets serialized by <see cref="AppSettings.Load"/>
    /// All members should have a publicly accessible constructor with no parameters, and be friendly to XmlSerializer
    /// </summary>
    public class SettingsNode
    {
        private XmlDictionary<string, CompilerSettingsNode> _compilerConfigurations;

        private class EditorControlConfigurationsDefaultFactory : IDefaultSettingsValueFactory
        {
            public bool NeedsToBeReset(SettingsNode settingsNode, object obj)
            {
                if (obj == null) return true;

                var dict = (XmlDictionary<string, LSLEditorControlSettings>)obj;


                if (dict.Count == 0)
                {
                    return true;
                }

                return false;
            }

            public object GetDefaultValue(SettingsNode settingsNode)
            {
                var d = new XmlDictionary<string, LSLEditorControlSettings>();

                

                d.Add("Default", new LSLEditorControlSettings());

                return d;
            }
        }

        [DefaultValueFactory(typeof(EditorControlConfigurationsDefaultFactory))]
        public XmlDictionary<string, LSLEditorControlSettings> EditorControlConfigurations { get; set; }



        private class CurrentEditorConfigurationDefaultFactory : IDefaultSettingsValueFactory
        {
            public bool NeedsToBeReset(SettingsNode settingsNode, object obj)
            {
                if (obj == null) return true;

                var val = obj.ToString();
                

                if (!settingsNode.EditorControlConfigurations.ContainsKey(val))
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(val))
                {
                    return true;
                }

                return false;
            }

            public object GetDefaultValue(SettingsNode settingsNode)
            {
                if (settingsNode.EditorControlConfigurations.ContainsKey("Default"))
                {
                    return "Default";
                }
                return settingsNode.EditorControlConfigurations.First().Key;
            }
        }


        [DefaultValueFactory(typeof (CurrentEditorConfigurationDefaultFactory))]
        public string CurrentEditorControlConfiguration { get; set; }


        private class CompilerConfigurationsDefaultFactory : IDefaultSettingsValueFactory
        {
            public bool NeedsToBeReset(SettingsNode settingsNode, object obj)
            {
                if (obj == null) return true;

                var dict = (XmlDictionary<string, CompilerSettingsNode>) obj;


                if (dict.Count == 0)
                {
                    return true;
                }

                return false;
            }

            public object GetDefaultValue(SettingsNode settingsNode)
            {
                var d = new XmlDictionary<string, CompilerSettingsNode>();


                var clientCode = new CompilerSettingsNode
                {
                    OpenSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimClientUploadable()
                };


                d.Add("OpenSim Client Code", clientCode);


                var clientCodeCoOp = new CompilerSettingsNode
                {
                    OpenSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimClientUploadable()
                };

                clientCodeCoOp.OpenSimCompilerSettings.InsertCoOpTerminationCalls = true;


                d.Add("OpenSim Client Code (co-op Stop)", clientCodeCoOp);


                var serverCode = new CompilerSettingsNode
                {
                    OpenSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimServerSideDefault()
                };


                d.Add("OpenSim Server Code", serverCode);



                var serverCodeCoOp = new CompilerSettingsNode
                {
                    OpenSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimServerSideDefault()
                };

                serverCodeCoOp.OpenSimCompilerSettings.InsertCoOpTerminationCalls = true;


                d.Add("OpenSim Server Code (co-op Stop)", serverCodeCoOp);


                return d;
            }
        }


        [DefaultValueFactory(typeof (CompilerConfigurationsDefaultFactory))]
        public XmlDictionary<string, CompilerSettingsNode> CompilerConfigurations
        {
            get { return _compilerConfigurations; }
            set { _compilerConfigurations = value; }
        }


        private class CurrentCompilerConfigurationDefaultFactory : IDefaultSettingsValueFactory
        {
            public bool NeedsToBeReset(SettingsNode settingsNode, object obj)
            {
                if (obj == null) return true;

                var val = obj.ToString();


                if (!settingsNode.CompilerConfigurations.ContainsKey(val))
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(val))
                {
                    return true;
                }

                return false;
            }

            public object GetDefaultValue(SettingsNode settingsNode)
            {
                if (settingsNode.EditorControlConfigurations.ContainsKey("OpenSim Client Code"))
                {
                    return "OpenSim Client Code";
                }
                return settingsNode.CompilerConfigurations.First().Key;
            }
        }


        [DefaultValueFactory(typeof (CurrentCompilerConfigurationDefaultFactory))]
        public string CurrentCompilerConfiguration { get; set; }
    }
}