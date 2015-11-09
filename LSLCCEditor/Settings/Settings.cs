using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.Compilers;
using LSLCCEditor.EditControl;

namespace LSLCCEditor.Settings
{

    /// <summary>
    /// The settings node that gets serialized by <see cref="AppSettings.Load"/>
    /// All members should have a publicly accessible constructor with no parameters, and be friendly to XmlSerializer
    /// </summary>
    public class SettingsNode
    {

        public LSLEditorControlSettings EditorControlSettings { get; set; }

        public LSLExpressionValidatorSettings ExpressionValidatorSettings { get; set; }

        public LSLOpenSimCSCompilerSettings OpenSimCSCompilerSettings { get; set; }


    }


    public static class AppSettings
    {
        public static SettingsNode Settings { get; private set; }


        public static string AppDataDir { get; private set; }

        public static string SettingsFile { get; private set; }



        private static readonly FieldInfo[] SettingsFields;

        private static readonly PropertyInfo[] SettingsProperties;

        static AppSettings()
        {
            AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LibLSLCC", "LSLCCEditor");

            Directory.CreateDirectory(AppDataDir);

            SettingsFile = Path.Combine(AppDataDir, "settings.xml");

            SettingsFields = typeof(SettingsNode).GetFields(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).ToArray();
            SettingsProperties = typeof(SettingsNode).GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).ToArray();
        }


        public static void Save()
        {


            XmlSerializer serializer = new XmlSerializer(typeof(SettingsNode));

            using (var reader = new StreamWriter(File.Create(SettingsFile)))
            {
                reader.AutoFlush = true;
                serializer.Serialize(reader, Settings);
            }
        }

        private static void InitNullSettingsProperties()
        {
            foreach (var field in SettingsFields)
            {
                if (field.GetValue(Settings) != null) continue;

                if (field.FieldType.GetConstructors().Any(x => !x.GetParameters().Any()))
                {
                    field.SetValue(Settings, Activator.CreateInstance(field.FieldType));
                }
                else
                {
                    throw new Exception(typeof(AppSettings).FullName + ".InitNullSettingsProperties():  SettingsNode field/property has no default parameterless constructor.");
                }
            }


            foreach (var field in SettingsProperties)
            {
                if (field.GetValue(Settings) != null) continue;

                if (field.PropertyType.GetConstructors().Any(x => !x.GetParameters().Any()))
                {
                    field.SetValue(Settings, Activator.CreateInstance(field.PropertyType));
                }
                else
                {
                    throw new Exception(typeof(AppSettings).FullName+".InitNullSettingsProperties():  SettingsNode field/property has no default parameterless constructor.");
                }
            }
        }



        private static void HandleLoadError(Exception e)
        {
            MessageBox.Show(
                "A problem was encountered loading the applications settings, settings will be returned to their default values.", 
                "Configuration Error", 
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            Settings = new SettingsNode();
        }


        public static void Load()
        {

            if (File.Exists(SettingsFile))
            {

                XmlSerializer serializer = new XmlSerializer(typeof (SettingsNode));

                try
                {
                    using (var reader = new StreamReader(File.OpenRead(SettingsFile)))
                    {
                        Settings = (SettingsNode) serializer.Deserialize(reader);
                    }
                }
                catch (Exception err)
                {
                    HandleLoadError(err);
                }


                InitNullSettingsProperties();
            }
            else
            {
                Settings = new SettingsNode();

                InitNullSettingsProperties();

                Save();
            }
        }
    }
}
