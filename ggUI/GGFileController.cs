/**
 * Performs input and output operations on the storage file used by GG.
 * The storage file is a comma sperated values (CSV) file. The columns in the
 * file is defined as description, startDate, endDate, createdDate. The default
 * file name is data.csv.
 * 
 * 
 * @author Peng Ziwei
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace gg
{
    class GGFileController : IFileController
    {
        private string pathToStorageFile = "data.csv";
        private string pathToDeletedStorageFile = "deleteddata.csv";
        private string pathToPreferenceFile = "pref.ini";

        private const string PREF_TITLE = "[GG]";
        private const int TUPLE_LENGTH = 2;
        private const int PREF_KEY_INDEX = 0;
        private const int PREF_VALUE_INDEX = 1;

        private const int NUM_OF_FIELDS = 6;
        private const int DESCRIPTION_INDEX = 0;
        private const int ENDDATE_INDEX = 1;
        private const int TAG_INDEX = 2;
        private const int LAST_MODIFIED_TIME_INDEX = 3;
        private const int ID_INDEX = 4;
        private const int PATH_INDEX = 5;

        /// <summary>
        /// Use default paths
        /// </summary>
        public GGFileController()
        {
            
        }

        /// <summary>
        /// Use specified storage path and default backup path
        /// </summary>
        /// <param name="pathToStorageFile"></param>
        public GGFileController(string pathToStorageFile)
        {
            this.pathToStorageFile = pathToStorageFile;
        }

        /// <summary>
        /// Use specified storage and backup path
        /// </summary>
        /// <param name="pathToStorageFile"></param>
        /// <param name="pathToDeletedStorageFile"></param>
        public GGFileController(string pathToStorageFile, string pathToDeletedStorageFile)
        {
            this.pathToStorageFile = pathToStorageFile;
            this.pathToDeletedStorageFile = pathToDeletedStorageFile;
        }

        /// <summary>
        /// Read from files
        /// </summary>
        /// <returns>GGList object</returns>
        public GGList ReadFromFile()
        {
            List<GGItem> innerList = ReadFromACSVFile(pathToStorageFile);
            List<GGItem> deletedList = ReadFromACSVFile(pathToDeletedStorageFile);

            GGList ggList = new GGList();

            ggList.SetInnerList(innerList);
            ggList.SetDeletedList(deletedList);

            return ggList;
        }

        /// <summary>
        /// Write to files
        /// </summary>
        /// <param name="ggList">GGList object to be written</param>
        public void WriteToFile(GGList ggList)
        {
            WriteToACSVFile(ggList.GetInnerList(), pathToStorageFile);
            WriteToACSVFile(ggList.GetDeletedList(), pathToDeletedStorageFile);
        }

        /// <summary>
        /// Read user preferences from ini file
        /// </summary>
        /// <returns>A dictionary of user preferences</returns>
        public Dictionary<string, string> ReadPreference()
        {
            Dictionary<string, string> pref = new Dictionary<string, string>();

            FileStream GGFileStream = new FileStream(pathToPreferenceFile, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader GGReader = new StreamReader(GGFileStream);

            string line = GGReader.ReadLine();

            if (line != PREF_TITLE)
            {
                return null;
            }
            
            while (!GGReader.EndOfStream)
            {
                line = GGReader.ReadLine();

                ParsePreferenceString(pref, line);
            }
       
            GGReader.Close();
            GGFileStream.Close();
            return pref;
        }

        /// <summary>
        /// Write user preferences to ini file
        /// </summary>
        /// <param name="userPreference"></param>
        public void WritePreference(Dictionary<string, string> userPreference)
        {
            FileStream GGFileStream = new FileStream(pathToPreferenceFile, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter GGWriter = new StreamWriter(GGFileStream);

            GGWriter.WriteLine(PREF_TITLE);
            GGWriter.Flush();

            Dictionary<string, string>.Enumerator GGEnum = userPreference.GetEnumerator();

            int flushCounter = 0;
            while (GGEnum.MoveNext())
            {
                string prefString = ToPreferenceString(ref GGEnum);

                GGWriter.WriteLine(prefString);
                if (flushCounter % 10 == 9)
                    GGWriter.Flush();
                flushCounter++;
            }

            GGWriter.Flush();
            GGWriter.Close();
            GGFileStream.Close();
        }

        /// <summary>
        /// Read from a CSV file
        /// </summary>
        /// <param name="path">The path of the CSV file</param>
        /// <returns>List of GGItems</returns>
        private List<GGItem> ReadFromACSVFile(string path)
        {
            FileStream GGFileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader GGReader = new StreamReader(GGFileStream);

            List<GGItem> list = new List<GGItem>();

            while (!GGReader.EndOfStream)
            {
                string csvLine = GGReader.ReadLine();
                if (!(csvLine == null || csvLine.Equals(string.Empty)))
                {
                    GGItem ggItem = ParseCSVString(csvLine);
                    if (ggItem != null)
                        list.Add(ggItem);
                }
            }

            GGReader.Close();
            GGFileStream.Close();
            return list;
        }

        /// <summary>
        /// Write to a CSV file
        /// </summary>
        /// <param name="list">The list of GGItems to be written</param>
        /// <param name="path">Path of the CSV file</param>
        private void WriteToACSVFile(List<GGItem> list, string path)
        {
            try
            {
                FileStream GGFileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                StreamWriter GGWriter = new StreamWriter(GGFileStream);

                GGWriter.Flush();

                for (int i = 0; i < list.Count(); i++)
                {
                    Debug.WriteLine("WriteToACsvFile: "+ToCSVString(list.ElementAt(i)) +" written to " + path, "info");
                    GGWriter.WriteLine(ToCSVString(list.ElementAt(i)));
                    if (i % 10 == 9)
                        GGWriter.Flush();
                }

                GGWriter.Flush();
                GGWriter.Close();
                GGFileStream.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine("GGFileController.WriteTOACSVFile: " + e.Message + " when writing to " + path, "error");
            }
        }

        /// <summary>
        /// Parse a CSV string to a GGItem
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private GGItem ParseCSVString(string line)
        {
            Debug.WriteLine("GGFileController.ParseCSVString: Parsing " + line);
            string[] parameters = line.Split(',');

            if (parameters.Length != NUM_OF_FIELDS)
            {
                throw new Exception("Invalid CSV string" + line);
            }
            try
            {
                CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");
                DateTime endDate = DateTime.Parse(parameters[ENDDATE_INDEX], usCulture);
                DateTime lastModifiedTime = DateTime.Parse(parameters[LAST_MODIFIED_TIME_INDEX]);

                string description = parameters[DESCRIPTION_INDEX].Replace("[newline]", Environment.NewLine);
                description = description.Replace("[comma]", ",");

                string tag = parameters[TAG_INDEX].Replace("[newline]", Environment.NewLine);
                tag = tag.Replace("[comma]", ",");

                return new GGItem(description, endDate, tag, lastModifiedTime, parameters[ID_INDEX],parameters[PATH_INDEX]);
            }
            catch (System.FormatException e)
            {
                Debug.WriteLine("GGFileController.ParseCSVString: "+e.Data, "error");
                return null;
            }

        }

        /// <summary>
        /// Parse a GGItem to CSV string
        /// </summary>
        /// <param name="ggItem"></param>
        /// <returns></returns>
        private string ToCSVString(GGItem ggItem)
        {
            string description = ggItem.GetDescription().Replace(Environment.NewLine, "[newline]");
            description = description.Replace(",", "[comma]");

            string tag = ggItem.GetTag().Replace(Environment.NewLine, "[newline]");
            tag = tag.Replace(",", "[comma]");
            CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");

            return description + ","
                   + ggItem.GetEndDate().ToString(usCulture) + ","
                   + tag + ","
                   + ggItem.GetLastModifiedTime().ToString() + ","
                   + ggItem.GetEventAbsoluteUrl() + ","
                   + ggItem.GetPath();
                   
        }

        /// <summary>
        /// Parse a preference string
        /// </summary>
        /// <param name="pref"></param>
        /// <param name="line"></param>
        private void ParsePreferenceString(Dictionary<string, string> pref, string line)
        {
            if ((line != null) && (!line.Equals(string.Empty)))
            {
                string[] tuple = line.Split('=');

                if (tuple.Length != TUPLE_LENGTH)
                {
                    Debug.WriteLine("GGFileController.ParsePreferenceString: Invalid preference string" + line, "error");
                    Environment.Exit(0);
                }

                try
                {
                    string prefKey = tuple[PREF_KEY_INDEX];
                    string prefValue = tuple[PREF_VALUE_INDEX];

                    prefKey.Replace("[equal]", "=");
                    prefValue.Replace("[equal]", "=");

                    pref.Add(prefKey, prefValue);

                }
                catch (System.FormatException e)
                {
                    Debug.WriteLine("GGFileController.ParsePreferenceString: "+e.Data,"error");
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Parse a dictionary pair to ini string
        /// </summary>
        /// <param name="GGEnum"></param>
        /// <returns></returns>
        private string ToPreferenceString(ref Dictionary<string, string>.Enumerator GGEnum)
        {
            string prefKey = GGEnum.Current.Key;
            string prefValue = GGEnum.Current.Value;

            prefKey = prefKey.Replace("=", "[space]");
            prefValue = prefValue.Replace("=", "[space]");

            string prefString = prefKey + "=" + prefValue;
            return prefString;
        }
    }
}

