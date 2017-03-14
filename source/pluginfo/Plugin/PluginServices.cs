using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace pluginfo
{
    #region PluginServices

    public class PluginServices
    {
        private Types.AvailablePlugins colAvailablePlugins = new Types.AvailablePlugins();

        /// <summary>
        /// A Collection of all Plugins Found and Loaded by the FindPlugins() Method
        /// </summary>
        public Types.AvailablePlugins AvailablePlugins
        {
            get { return colAvailablePlugins; }
            set { colAvailablePlugins = value; }
        }

        /// <summary>
        /// Searches the Application's Startup Directory for Plugins
        /// </summary>
        public void FindPlugins()
        {
            FindPlugins(AppDomain.CurrentDomain.BaseDirectory);
        }
        /// <summary>
        /// Searches the passed Path for Plugins
        /// </summary>
        /// <param name="Path">Directory to search for Plugins in</param>
        public void FindPlugins(string Path)
        {
            colAvailablePlugins.Clear();

            foreach (string dirOn in Directory.GetDirectories(Path))
            {
                //Directory dir = new Directory(dirOn);
                foreach (string fileOn in Directory.GetFiles(dirOn))
                {
                    FileInfo file = new FileInfo(fileOn);
                    if (file.Extension.Equals(".dll"))
                    {
                        this.AddPlugin(fileOn);
                    }
                }
            }
        }

        /// <summary>
        /// Unloads and Closes all AvailablePlugins
        /// </summary>
        public void ClosePlugins()
        {
            foreach (Types.AvailablePlugin pluginOn in colAvailablePlugins)
            {
                pluginOn.Instance = null;
            }

            colAvailablePlugins.Clear();
        }

        private void AddPlugin(string FileName)
        {
            Assembly pluginAssembly = Assembly.LoadFrom(FileName);

            try
            {
                foreach (Type pluginType in pluginAssembly.GetTypes())
                {
                    if (pluginType.IsPublic)
                    {
                        if (!pluginType.IsAbstract)
                        {
                            //Gets a type object of the interface we need the plugins to match
                            Type typeInterface = pluginType.GetInterface("pluginfo.IPlugin", true);

                            //Make sure the interface we want to use actually exists
                            if (typeInterface != null)
                            {
                                //Create a new available plugin since the type implements the IPlugin interface
                                Types.AvailablePlugin newPlugin = new Types.AvailablePlugin();

                                //Set the filename where we found it
                                newPlugin.AssemblyPath = FileName;
                                newPlugin.Instance = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));

                                //Add the new plugin to our collection here
                                this.colAvailablePlugins.Add(newPlugin);
                                newPlugin = null;
                            }

                            typeInterface = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There is an error in:\n" + FileName + "\n\nOriginal error:\n" + ex.Message);
            }

            pluginAssembly = null;
        }
    }

    #endregion

    #region namespace Types

    namespace Types
    {
        #region AvailablePlugins

        /// <summary>
        /// Collection for AvailablePlugin Type
        /// </summary>
        public class AvailablePlugins : System.Collections.CollectionBase
        {
            /// <summary>
            /// Add a Plugin to the collection of Available plugins
            /// </summary>
            /// <param name="pluginToAdd">The Plugin to Add</param>
            public void Add(Types.AvailablePlugin pluginToAdd)
            {
                this.List.Add(pluginToAdd);
            }

            /// <summary>
            /// Remove a Plugin to the collection of Available plugins
            /// </summary>
            /// <param name="pluginToRemove">The Plugin to Remove</param>
            public void Remove(Types.AvailablePlugin pluginToRemove)
            {
                this.List.Remove(pluginToRemove);
            }

            /// <summary>
            /// Finds a plugin in the available Plugins
            /// </summary>
            /// <param name="pluginNameOrPath">The name or File path of the plugin to find</param>
            /// <returns>Available Plugin, or null if the plugin is not found</returns>
            public Types.AvailablePlugin Find(string pluginNameOrPath)
            {
                Types.AvailablePlugin toReturn = null;

                foreach (Types.AvailablePlugin pluginOn in this.List)
                {
                    if ((pluginOn.Instance.pName.Equals(pluginNameOrPath)) || pluginOn.AssemblyPath.Equals(pluginNameOrPath))
                    {
                        toReturn = pluginOn;
                        break;
                    }
                }
                return toReturn;
            }
        }

        #endregion

        #region AvailablePlugin

        /// <summary>
        /// Data Class for Available Plugin.  Holds and instance of the loaded Plugin, as well as the Plugin's Assembly Path
        /// </summary>
        public class AvailablePlugin
        {
            //This is the actual AvailablePlugin object
            //Holds an instance of the plugin to access
            private IPlugin myInstance = null;
            private string myAssemblyPath = "";

            public IPlugin Instance
            {
                get { return myInstance; }
                set { myInstance = value; }
            }
            public string AssemblyPath
            {
                get { return myAssemblyPath; }
                set { myAssemblyPath = value; }
            }
        }

        #endregion
    }

    #endregion
}

