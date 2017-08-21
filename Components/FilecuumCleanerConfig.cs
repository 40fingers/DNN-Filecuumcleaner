using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using FortyFingers.FilecuumCleaner.Library;

namespace FortyFingers.FilecuumCleaner.Components
{
    public class FilecuumCleanerConfig: ConfigFileBase<FilecuumCleanerConfig>
    {
        public FilecuumCleanerConfig()
            : base(Null.NullInteger, "FilecuumCleaner", "FilecuumCleanerConfig")
        {
            FilecuumCleanerJobs = new List<FilecuumCleanerJob>();
        }

        public static FilecuumCleanerConfig GetConfig(bool useCache)
        {
            var cfg = new ConfigFileBase<FilecuumCleanerConfig>(Null.NullInteger, "FilecuumCleaner", "FilecuumCleanerConfig");
            return cfg.GetConfig();
        }

        [XmlArrayItem("FilecuumCleanerJob")]
        public List<FilecuumCleanerJob> FilecuumCleanerJobs { get; set; }

        public void DeleteJob(string jobId)
        {
            // remove the job
            FilecuumCleanerJobs.RemoveAll(j => j.Id == jobId);
            // save the file
            ToFile();
            // empty cache
            ClearCache();
        }

        public void MoveJobUp(string jobId)
        {
            // find the job to move
            var job = FilecuumCleanerJobs.FirstOrDefault(j => j.Id == jobId);
            if (job == null)
                // nothing to move
                return;

            // get the job index
            var jobIx = FilecuumCleanerJobs.IndexOf(job);
            // Remove it from the old position
            FilecuumCleanerJobs.RemoveAt(jobIx);
            // insert it at the new position
            FilecuumCleanerJobs.Insert(jobIx == 0 ? 0 : jobIx - 1, job);
            // Save the file
            ToFile();
            // invalidate cache
            ClearCache();
        }

        public void MoveJobDown(string jobId)
        {
            // find the job to move
            var job = FilecuumCleanerJobs.FirstOrDefault(j => j.Id == jobId);
            if (job == null)
                // nothing to move
                return;

            // get the job index
            var jobIx = FilecuumCleanerJobs.IndexOf(job);
            // Remove it from the old position
            FilecuumCleanerJobs.RemoveAt(jobIx);
            // insert it at the new position
            int maxIndex = FilecuumCleanerJobs.Count - 1;
            FilecuumCleanerJobs.Insert(jobIx == maxIndex ? maxIndex : jobIx + 1, job);
            // Save the file
            ToFile();
            // invalidate cache
            ClearCache();
        }
    }

    public class FilecuumCleanerJob
    {
        public FilecuumCleanerJob()
        {
            RootPath = Common.RootPathWebsite;
        }

        // dummy xmldocument for creating cdata elements
        [XmlIgnore]
        private XmlDocument _dummyXmlDoc = null;

        private string _path;

        [XmlIgnore]
        private XmlDocument DummyXmlDoc
        {
            get
            {
                if (_dummyXmlDoc == null)
                    _dummyXmlDoc = new XmlDocument();

                return _dummyXmlDoc;
            }
        }

        [XmlElement("Id")]
        public string Id { get; set; }
        [XmlElement("PortalId")]
        public int PortalId { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("RootPath")]
        public int RootPath { get; set; }
        [XmlElement("Path")]
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value
                        .Trim() // remove leading and trailing spaces
                        .Replace('\\', '/') // correct backslashes to slashes. Users make these kind of mistakes
                        .TrimStart('/') // remove leading slash
                        .TrimEnd('/'); // remove trailing slash too
                ;
            }
        }
        [XmlElement("IncludedExtensions")]
        public string IncludedExtensions { get; set; }

        [XmlIgnore()] private List<string> _includedExtensionsList = null;
        [XmlIgnore()]
        public List<string> IncludedExtensionsList
        {
            get
            {
                if (_includedExtensionsList == null)
                {
                    _includedExtensionsList = new List<string>();
                    var arr = IncludedExtensions.Split(',');
                    foreach (var ext in arr)
                    {
                        var cleanExt = ext.Trim();
                        if (!cleanExt.StartsWith("."))
                            cleanExt = "." + cleanExt;

                        _includedExtensionsList.Add(cleanExt);
                    }
                }
                return _includedExtensionsList;
            }
        }
        [XmlElement("MaxBytes")]
        public int MaxBytes { get; set; }
        [XmlElement("MaxAgeDays")]
        public int MaxAgeDays { get; set; }
        [XmlElement("IncludeSubFolders")]
        public bool IncludeSubFolders { get; set; }
        [XmlElement("Enabled")]
        public bool Enabled { get; set; }
        [XmlElement("DeleteEmptyFolders")]
        public bool DeleteEmptyFolders { get; set; }
    }
}