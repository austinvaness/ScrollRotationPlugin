using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VRage.FileSystem;

namespace avaness.ScrollRotationPlugin
{
    public class Config
    {
        private const string FileName = "ScrollRotationConfig.xml";

        /// <summary>
        /// True if the user should be controlling the axis directly, where Shift = rotate and Alt = change axis
        /// </summary>
        public bool AxisControl { get; set; } = false;

        public void Save()
        {
            try
            {
                string path = Path.Combine(MyFileSystem.UserDataPath, "Storage", FileName);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Config));

                using (StreamWriter output = File.CreateText(path))
                {
                    xmlSerializer.Serialize(output, this);
                    Main.Log("Config saved.");
                }
            }
            catch (Exception e)
            {
                Main.Log("An error occurred while saving the config: " + e);
            }
        }

        public static Config Load()
        {

            try
            {
                string path = Path.Combine(MyFileSystem.UserDataPath, "Storage", FileName);

                if(File.Exists(path))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Config));
                    using (FileStream input = File.OpenRead(path))
                    {
                        Config result = xmlSerializer.Deserialize(input) as Config;
                        if (result != null)
                            return result;
                    }
                }
            }
            catch (Exception e)
            {
                Main.Log("An error occurred while reading the config: " + e);
            }

            return new Config();
        }
    }
}
