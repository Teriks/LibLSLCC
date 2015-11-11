using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LibLSLCC.Settings
{
    public enum SettingsErrorType
    {
        FileMissing,
        FileUnreadable,
        SyntaxError,
    }

    public class SettingsManagerConfigErrorEventArgs
    {
        public SettingsManagerConfigErrorEventArgs(SettingsErrorType errorType, bool settingsReset)
        {
            ErrorType = errorType;
            SettingsReset = settingsReset;
        }

        public bool SettingsReset { get; private set; }

        public SettingsErrorType ErrorType { get; private set; }
    }


    public class SettingsManager<T> where T :  new()
    {
        private readonly bool _resetOnConfigError;
        public T Settings { get; private set; }


        public event Action<object, SettingsManagerConfigErrorEventArgs> ConfigError;


        public SettingsManager(bool resetSettingsOnConfigError = true)
        {
            _resetOnConfigError = resetSettingsOnConfigError;
        }


        public void Save(string file)
        {

            var serializer = new XmlSerializer(typeof(T));

            using (var reader = new StreamWriter(File.Create(file)))
            {
                reader.AutoFlush = true;
                serializer.Serialize(reader, Settings);
            }
        }


        public void ApplyDefaults()
        {
            Settings = new T();

            DefaultValueInitializer.Init(Settings);
        }

        private void HandleLoadError(SettingsErrorType type)
        {
            if (_resetOnConfigError)
            {
                ApplyDefaults();
            }

            OnConfigError(type);
        }

        public void Load(string file)
        {
            if (File.Exists(file))
            {
                var serializer = new XmlSerializer(typeof(T));

                try
                {
                    using (var reader = new StreamReader(File.OpenRead(file)))
                    {
                        Settings = (T) serializer.Deserialize(reader);
                    }
                }
                catch (IOException)
                {
                    HandleLoadError(SettingsErrorType.FileUnreadable);
                }
                catch (Exception)
                {
                    HandleLoadError(SettingsErrorType.SyntaxError);
                }

                DefaultValueInitializer.Init(Settings);
            }
            else
            {
                HandleLoadError(SettingsErrorType.FileMissing);
            }
        }

        protected virtual void OnConfigError(SettingsErrorType type)
        {
            var handler = ConfigError;
            if (handler != null) handler(this, new SettingsManagerConfigErrorEventArgs(type, _resetOnConfigError));
        }
    }
}
