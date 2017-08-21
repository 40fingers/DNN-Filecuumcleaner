using System;
using System.IO;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.Exceptions;

namespace FortyFingers.FilecuumCleaner.Library
{
    public class ConfigFileBase<T> where T:ConfigFileBase<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Name of the config file withour extension. This parameter is also used for generating cachekey.</param>
        public ConfigFileBase(int portalId, string moduleName, string fileName)
        {
            FileName = fileName;
            ModuleName = moduleName;
            PortalId = portalId;
        }

        protected int PortalId { get; set; }
        protected string ModuleName { get; private set; }
        protected string FileName { get; private set; }
        protected Type ConfigType { get; private set; }

        public string ConfigFile
        {
            get
            {
                var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                                        "Portals\\_default\\40Fingers\\{0}\\{1}.xml");
                return
                    Globals.ResolveUrl(String.Format(path,
                                                     ModuleName,
                                                     FileName));
            }
        }

        private string ConfigCacheKey
        {
            get
            {
                return String.Format("40FINGERS.{0}.{1}.CONFIG,{2}", ModuleName, FileName, PortalId);
            }
        }
        public T GetConfig()
        {
            return GetConfig(true);
        }
        public T GetConfig(bool useCache) 
        {
            T config = default(T);

            if (useCache)
                config = DataCache.GetCache<T>(ConfigCacheKey);

            if (config == null)
            {
                var file = ConfigFile;
                config = FromFile(file);
                DataCache.SetCache(ConfigCacheKey, config, new DNNCacheDependency(file));
            }

            return config;
        }

        public void ClearCache()
        {
            DataCache.RemoveCache(ConfigCacheKey);
        }

        private static T FromFile(string filename) 
        {
            // create the file if it doesn't exsist
            if (!File.Exists(filename))
                CreateFile(filename);

            T retval = default(T);
            FileStream fs = null;

            try
            {
                // now we can just open it: i must exsist
                fs = new FileStream(filename, FileMode.Open);
                retval = (T)XmlSer().Deserialize(fs);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            return retval;
        }

        public static XmlSerializer XmlSer() 
        {
            return new XmlSerializer(typeof(T));
        }

        public void ToFile()
        {
            ToFile(ConfigFile);
        }
        public void ToFile(string filename)
        {
            var filePath = filename.Substring(0, filename.LastIndexOf("\\"));
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            FileStream fs = null;

            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);

                fs = new FileStream(filename, FileMode.CreateNew);
                XmlSer().Serialize(fs, this);

                ClearCache();
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }

        }

        public static void CreateFile(string filename)
        {
            var emptyConfig = Activator.CreateInstance<T>();
            emptyConfig.ToFile(filename);
        }
    }
}