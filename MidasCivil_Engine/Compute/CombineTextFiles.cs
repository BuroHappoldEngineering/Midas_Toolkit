/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using System.Text;

namespace BH.Engine.Adapters.MidasCivil
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Combines all text files specified by type in to a single MidasCivilText (MCT) to be loaded in to MidasCivil")]
        [Input("filePath", "The same filepath used for the adapter (pointing to an mcb file)")]
        [Input("types", "BHoM object types to specify the text files to be combined. A null value will combine all text files.")]
        [Input("active", "Execute the method")]
        [Output("success", "Was the execution successful?")]

        public static bool CombineTextFiles(string filePath, List<Type> types = null, bool active = false)
        {
            if (active)
            {
                DateTime Date = DateTime.Now;
                string intro = ";---------------------------------------------------------------------------"
                    + "\n;   MIDAS/Civil Text(MCT) File"
                    + $"\n;   Created using the BHoM v{BH.Engine.Reflection.Query.BHoMVersion()}"
                    + $"\n;   Date: {Date.ToString("yyyy-MM-dd")}"
                    + "\n;---------------------------------------------------------------------------\n\n";

                string directory = Path.GetDirectoryName(filePath) + "\\TextFiles";

                string path = directory + "\\" + "Combined.mct";

                // Retrieve type strings: select all from directory if none provided

                List<string> typeNames = new List<string>();
                List<string> metadata = new List<string>(3);
                metadata.Add("VERSION");
                metadata.Add("UNIT");
                metadata.Add("PROJINFO");

                bool includeLoadcases = true;

                if (types.Count == 0)
                {
                    try
                    {
                        typeNames = Directory.GetFiles(directory, "*.txt").Select(Path.GetFileName).ToList();
                    }
                    catch (DirectoryNotFoundException)
                    {
                        throw new Exception("Directory not found, please specify a valid file path to an .mcb file");
                    }


                    for (int i = 0; i < typeNames.Count; i++)
                    {
                        typeNames[i] = typeNames[i].Replace(".txt", "");
                    }
                }
                else
                {
                    types.ForEach(x => typeNames.Add(ToType(x.ToString())));

                    if (!typeNames.Contains("LOADCASE"))
                    {
                        includeLoadcases = false;
                    }

                    typeNames.Remove("LOADCASE");
                }

                // Check type dependencies to see if valid

                List<string> independents = new List<string> { "NODE", "ELEMENT", "MATERIAL", "SECTION", "STLDCASE" };
                List<string> nodeDependencies = new List<string> { "ELEMENT", "CONSTRAINT", "GROUPS", "LOCALAXIS", "SPRING", "CONLOAD" };
                List<string> elementDependencies = new List<string> { "GROUPS", "BEAMLOAD", "PRESSURE", "ELTEMPER" };
                List<string> materialDependencies = new List<string> { "DGN-MATL" };
                List<string> sectionDependencies = new List<string> { "DGN-SECT" };
                List<string> loadcaseDependencies = new List<string> { "LOADCOMB", "LC-COLOUR" };

                List<List<string>> dependents = new List<List<string>>();
                dependents.Add(nodeDependencies);
                dependents.Add(elementDependencies);
                dependents.Add(materialDependencies);
                dependents.Add(sectionDependencies);
                dependents.Add(loadcaseDependencies);

                for (int i = 0; i < independents.Count; i++)
                {
                    if (!typeNames.Contains(independents[i]))
                    {
                        foreach (string dependent in dependents[i])
                        {
                            if (typeNames.Contains(dependent))
                                Reflection.Compute.RecordError(dependent + " must have a " + independents[i] + " file associated with it");
                        }
                    }
                }

                independents.Insert(0, "REBAR-MATL-CODE");
                independents.Add("LOAD-GROUP");
                independents.Add("LOADCOMB");

                List<string> loadcases = Directory.GetDirectories(directory).ToList();

                using (FileStream combined = File.Create(path))
                {
                    using (StreamWriter writer = new StreamWriter(combined, Encoding.GetEncoding(1252), 512000))
                    {
                        writer.Write(intro);
                        writer.Flush();
                        foreach (string file in metadata)
                        {
                            if (File.Exists(directory + "\\" + file + ".txt"))
                            {
                                using (var input = File.OpenRead(directory + "\\" + file + ".txt"))
                                {
                                    if (new FileInfo(directory + "\\" + file + ".txt").Length != 0)
                                    {
                                        input.CopyTo(combined);
                                        input.Close();
                                    }
                                }
                                writer.Write(System.Environment.NewLine);
                                writer.Flush();
                            }
                        }

                        foreach (string independent in independents)
                        {
                            if (typeNames.Contains(independent))
                            {
                                if (new FileInfo(directory + "\\" + independent + ".txt").Length != 0)
                                {
                                    var input = File.OpenRead(directory + "\\" + independent + ".txt");
                                    input.CopyTo(combined);
                                    input.Close();
                                    writer.Write(System.Environment.NewLine);
                                    writer.Flush();
                                }
                            }
                        }

                        typeNames = typeNames.Except(independents).Except(metadata).ToList();

                        if (loadcases.Count() != 0 && includeLoadcases)
                        {
                            foreach (string loadcase in loadcases)
                            {
                                string loadcaseName = Path.GetFileName(loadcase);

                                List<string> loadNames = Directory.GetFiles(loadcase, "*.txt")
                                            .Select(Path.GetFileName)
                                            .ToList();

                                bool endIncluded = false;

                                foreach (string loadName in loadNames)
                                {
                                    List<string> contents = File.ReadAllLines(loadcase + "\\" + loadName).ToList();
                                    foreach (string line in contents)
                                    {
                                        if (line.Contains("; End of data") && new FileInfo(loadcase + "\\" + loadName).Length != 0)
                                        {
                                            loadNames.RemoveAt(loadNames.IndexOf(loadName));
                                            loadNames.Add(loadName);
                                            endIncluded = true;
                                            goto ESCAPE;
                                        }
                                    }
                                }

                            ESCAPE:

                                loadNames.Remove("USE-STLD.txt");
                                writer.Write("*USE-STLD, " + loadcaseName);
                                writer.Write(System.Environment.NewLine);
                                writer.Flush();

                                foreach (string load in loadNames)
                                {
                                    using (var input = File.OpenRead(loadcase + "\\" + load))
                                    {
                                        input.CopyTo(combined);
                                        input.Close();
                                    }

                                    writer.Write(System.Environment.NewLine);
                                    writer.Flush();
                                }

                                if (!endIncluded)
                                {
                                    writer.Write("; End of data for load case " + loadcaseName);
                                    writer.Write(System.Environment.NewLine);
                                    writer.Write(System.Environment.NewLine);
                                    writer.Flush();
                                }
                            }
                        }

                        foreach (string file in typeNames)
                        {
                            using (var input = File.OpenRead(directory + "\\" + file + ".txt"))
                            {
                                if (new FileInfo(directory + "\\" + file + ".txt").Length != 0)
                                {
                                    input.CopyTo(combined);
                                    input.Close();
                                }
                            }
                            writer.Write(System.Environment.NewLine);
                            writer.Flush();
                        }

                        writer.Write("*ENDDATA");
                        writer.Flush();
                        writer.Close();
                    }
                    combined.Close();

                    string fName = $"{Path.GetDirectoryName(filePath)}\\{ Date.ToString("yyMMdd")}_{ Path.GetFileNameWithoutExtension(filePath).Replace(" ", "_")}";
                    int i = 1;
                    if (File.Exists(fName + ".mct"))
                    {
                        string fName_ = fName;
                        while (i > 0)
                        {
                            if (File.Exists(fName_ + ".mct"))
                            {
                                i++;
                                fName_ = $"{fName}_v{i}";
                            }
                            else { fName = fName_; i = -1; }
                        }
                    }

                    File.Copy(path, fName + ".mct", true);
                    if (Path.GetFileName(path) == "Combined.mct") { File.Delete(path); };


                }

                return true;
            }

            return false;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string ToType(string type)
        {

            string[] delimited = type.Split('.');
            type = delimited[delimited.Count() - 1];

            Dictionary<string, string> conversion = new Dictionary<string, string>
            {
                {"Node", "NODE" },
                {"Bar", "ELEMENT" },
                {"FEMesh", "ELEMENT" },
                {"Constraint6DOF", "CONSTRAINT" },
                {"Material", "MATERIAL" },
                {"SteelSection", "SECTION" },
                {"ConcreteSection", "SECTION" },
                {"ConstantThickness", "THICKNESS" },
                {"Loadcase", "LOADCASE" },
            };

            string midasVersion;
            conversion.TryGetValue(type, out midasVersion);
            return midasVersion;
        }

        /***************************************************/

    }
}