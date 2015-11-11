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

namespace LSLCCEditor.Settings
{

 

    public static class AppSettings
    {
        public static SettingsNode Settings { get; private set; }


        public static string AppDataDir { get; private set; }

        public static string SettingsFile { get; private set; }



        static AppSettings()
        {
            AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LibLSLCC", "LSLCCEditor");

            Directory.CreateDirectory(AppDataDir);

            SettingsFile = Path.Combine(AppDataDir, "settings.xml");

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

        public static void InitNullSettingsProperties(object instance)
        {
            var settingsProperties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).ToArray();


            const BindingFlags constructorBindingFlags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


            foreach (var field in settingsProperties)
            {

                var fieldValue = field.GetValue(instance);

                var attr = field.GetCustomAttributes(typeof(DefaultValueFactoryAttribute), true).ToList();
                if (attr.Any())
                {
                    var factory = ((DefaultValueFactoryAttribute)attr.First());
                    if (factory.Factory.CheckForNecessaryResets(instance, fieldValue))
                    {
                        field.SetValue(instance, factory.Factory.GetDefaultValue(instance));
                    }
                }
                else if (fieldValue == null)
                {
                    attr = field.GetCustomAttributes(typeof(DefaultValueFactoryAttribute), true).ToList();
                    if (attr.Any())
                    {
                        var defaultValue = ((DefaultValueAttribute) attr.First());

                        field.SetValue(instance, defaultValue.Value);
                    }
                    else
                    {
                        var constructors = field.PropertyType.GetConstructors(constructorBindingFlags).Where(x=>!x.GetParameters().Any()).ToList();
                        if (!constructors.Any())
                        {
                            throw new Exception(typeof (AppSettings).FullName +
                                                ".InitNullSettingsProperties():  SettingsNode property has no parameterless constructor.");
                        }

                        var constructor = constructors.First();
                            
                        field.SetValue(instance, constructor.Invoke(null));
                    }
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


                InitNullSettingsProperties(Settings);
            }
            else
            {
                Settings = new SettingsNode();

                InitNullSettingsProperties(Settings);

                Save();
            }
        }
    }
}
