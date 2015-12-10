using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Microsoft.Win32;
using MessageBox = System.Windows.Forms.MessageBox;

namespace LSLCCEditor.SettingsUI
{
    internal static class ImportExportTools
    {
        public static void DoImportSettingsWindow(Window owner, string fileFilter, string fileExt, Action<XmlTextReader> serialize)
        {
            var openDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = fileFilter,
                AddExtension = true,
                DefaultExt = fileExt
            };
            if (!openDialog.ShowDialog(owner).Value)
            {
                return;
            }

            try
            {
                using (var file = new XmlTextReader(openDialog.OpenFile()))
                {
                    serialize(file);
                }
            }
            catch (XmlSyntaxException ex)
            {
                System.Windows.Forms.MessageBox.Show("An XML syntax error was encountered while loading the file, settings could not be applied: "
                                + Environment.NewLine + Environment.NewLine + ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("There was an unknown error while loading the settings file, settings could not be applied: "
                                + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }


        public static void DoExportSettingsWindow(Window owner, string fileFilter, string fileName, Action<XmlTextWriter> serialize)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = fileFilter,
                FileName = fileName
            };


            if (!saveDialog.ShowDialog(owner).Value)
            {
                return;
            }

            try
            {
                using (var file = new XmlTextWriter(saveDialog.OpenFile(), Encoding.Unicode))
                {
                    file.Formatting = Formatting.Indented;
                    serialize(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected problem occurred while trying to save the file: "
                                + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }
    }
}
