using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityLogger.GUI
{
    public class AutoStartHelper
    {
        public static bool HasAutoStartConfigured()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", false);
            string value = (string)key.GetValue("ActivityLogger", string.Empty);
            if (value != String.Empty)
            {
                return true;
            }
            return false;
        }

        public static void SetAutoStart()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key.SetValue("ActivityLogger", String.Format("\"{0}\"", System.Windows.Forms.Application.ExecutablePath));
        }

        public static void RemoveAutoStart()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key.DeleteValue("ActivityLogger", false);
        }
    }
}
