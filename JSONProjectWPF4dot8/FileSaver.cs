using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JSONProjectWPF4dot8
{
    /// <summary>
    /// Class responsible for saving an output to a new file
    /// </summary>
    internal class FileSaver
    {
        private StreamWriter streamWriter;

        /// <summary>
        /// Empty constructor
        /// </summary>
        public FileSaver()
        {
            // empty constructor
        }

        /// <summary>
        /// Save the contents of the input JSON Object into a new text file
        /// </summary>
        /// <param name="jsonObj">json object to save</param>
        /// <returns>the name of the file that was saved</returns>
        public string saveFile(JSONObject jsonObj, string outputFileName)
        {
            if (jsonObj == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            string fileSaveName = outputFileName;

            try
            {
                sb.Append(outputFileName);
                fileSaveName = sb.ToString();

                // write to new file
                FileStream fileStream = new FileStream(fileSaveName, FileMode.Create, FileAccess.Write);
                streamWriter = new StreamWriter(fileStream);
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);

                // for each line in json object write
                //streamWriter.WriteLine("{");
                List<KeyValuePair> keyValuePairs = jsonObj.getAllEntries();
                writeTree(keyValuePairs, 1, false);
                //streamWriter.WriteLine("}");

                streamWriter.Flush();
                streamWriter.Close();
                fileStream.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("FileSaver.saveFile -- Exception saving file: " + e.Message);
                return null;
            }

            return fileSaveName;
        }

        private void writeTree(List<KeyValuePair> entries, int tabIndex, bool isNestedJson)
        {
            int currEntry = 0;
            streamWriter.WriteLine("");
            foreach (KeyValuePair kvp in entries)
            {
                currEntry = currEntry + 1;
                for (int i = 0; i < tabIndex; i++)
                {
                    streamWriter.Write("     ");
                    if (i == tabIndex - 1)
                    {
                        streamWriter.Write("+");
                    }
                }

                streamWriter.Write("\"");

                streamWriter.Write(kvp.getKey());
                streamWriter.Write("\"");
                streamWriter.Write(": ");

                Object val = kvp.getVal();
                if (val is JSONObject)
                {
                    writeTree(((JSONObject)val).getAllEntries(), tabIndex + 1, true);
                }
                else
                {
                    streamWriter.Write("\"");
                    streamWriter.Write((string)val);
                    streamWriter.Write("\"");
                    streamWriter.WriteLine("");
                }
                if (!isNestedJson)
                {
                    streamWriter.WriteLine("");
                }
            }
            for (int i = 0; i < tabIndex - 1; i++)
            {
                streamWriter.Write("    ");
                if (i == tabIndex - 2 && !isNestedJson)
                {
                    streamWriter.Write("+");
                }
            }
        }

        /// <summary>
        /// Writes key-value pairs to a file
        /// </summary>
        /// <param name="keyValuePairs">key-value pairs to write</param>
        private void writeJson(List<KeyValuePair> keyValuePairs)
        {
            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                KeyValuePair kvp = keyValuePairs[i];

                // print key
                string key = kvp.getKey();
                streamWriter.Write("\"" + key + "\": ");

                // print val
                Object val = kvp.getVal();
                if (val is string)
                {
                    streamWriter.Write("\"" + val);
                    if (i != keyValuePairs.Count - 1)
                    {
                        streamWriter.WriteLine("\",");
                    }
                }
                else // val is JSONObject
                {
                    streamWriter.WriteLine("{");
                    writeJson((val as JSONObject).getAllEntries());
                    streamWriter.WriteLine("}" + (i != keyValuePairs.Count - 1 ? "," : ""));
                }
            }
        }
    }
}
