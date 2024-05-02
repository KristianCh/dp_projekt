using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Entities.WordProcessing
{
    public static class CsvReader
    {
        public static void Read(string filename, char separator, out List<string[]> loadedValues)
        {
            var data = "";
            loadedValues = new List<string[]>();
            if (string.IsNullOrEmpty(filename)) return;
            var streamReader = new StreamReader(filename);
            while (true)
            {
                var line = streamReader.ReadLine();
                
                data += "\"" + line + "\",\n";
                
                if (line == null)
                    break;
                var lineValues = line.Split(separator);
                loadedValues.Add(lineValues);
            }
            Debug.Log(data);
        }

        public static string[] ParseLine(string line, char separator)
        {
            if (line == null)
                return new string[] {};
            var lineValues = line.Split(separator);
            return lineValues;
        }
    }
}