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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Adapter;
using BH.oM.Adapter;
using BH.oM.Reflection;
using BH.oM.Adapter.Commands;
using BH.oM.Structure.Loads;
using System.Runtime.InteropServices;
using System.Text;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter

    {
        /***************************************************/
        /**** IAdapter Interface                        ****/
        /***************************************************/

        public override Output<List<object>, bool> Execute(IExecuteCommand command, ActionConfig actionConfig = null)
        {
            var output = new Output<List<object>, bool>() { Item1 = null, Item2 = false };

            output.Item2 = RunCommand(command as dynamic);

            return output;
        }

        /***************************************************/
        /**** Commands                                  ****/
        /***************************************************/

        public bool RunCommand(NewModel command)
        {
            string newDirectory = GetDirectoryRoot(m_directory) + "\\Untitled";

            bool directoryExists = Directory.Exists(newDirectory);

            int i = 1;

            while (directoryExists)
            {
                newDirectory = newDirectory + 1;
                directoryExists = Directory.Exists(newDirectory);
                i++;
            }

            string unitExtension = "\\TextFiles\\" + "UNIT" + ".txt";
            string versionExtension = "\\TextFiles\\" + "VERSION" + ".txt";
            string unitFile = m_directory + unitExtension;
            string versionFile = m_directory + versionExtension;

            Directory.CreateDirectory(newDirectory + "\\TextFiles");

            if (!File.Exists(unitFile))
                File.Copy(unitFile, newDirectory + unitExtension);
            else
                File.AppendAllLines(newDirectory + unitExtension, new List<string>() { "*UNIT", "N,M,KJ,C" }, Encoding.GetEncoding(1252));

            if (!File.Exists(versionFile))
                File.Copy(versionFile, newDirectory + versionExtension);
            else
                File.AppendAllLines(newDirectory + versionExtension, new List<string>() { "*VERSION", m_midasCivilVersion }, Encoding.GetEncoding(1252));

            m_directory = newDirectory;
            Directory.CreateDirectory(newDirectory + "\\Results");

            return true;
        }

        /***************************************************/

        public bool RunCommand(Save command)
        {
            Engine.Reflection.Compute.RecordWarning($"The command {command.GetType().Name} is not supported by this Adapter.");
            return false;
        }

        /***************************************************/

        public bool RunCommand(SaveAs command)
        {
            string newDirectory = GetDirectoryRoot(m_directory) + "\\" + command.FileName;

            if (Directory.Exists(newDirectory))
            {
                Engine.Reflection.Compute.RecordError("File with the same name already exists, please choose another.");
                return false;
            }

            Directory.CreateDirectory(newDirectory);
            string[] mcbFiles = Directory.GetFiles(m_directory, "*.mcb");
            foreach (string mcbFile in mcbFiles)
                File.Copy(mcbFile, newDirectory);
            string[] mctFiles = Directory.GetFiles(m_directory, "*.mcb");
            foreach (string mctFile in mctFiles)
                File.Copy(mctFile, newDirectory);
            CopyAll(new DirectoryInfo(m_directory + "\\TextFiles"), new DirectoryInfo(newDirectory + "\\TextFiles"));
            CopyAll(new DirectoryInfo(m_directory + "\\Results"), new DirectoryInfo(newDirectory + "\\Results"));

            m_directory = newDirectory;

            return true;
        }

        /***************************************************/

        public bool RunCommand(Open command)
        {
            string filePath = command.FileName;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("No file path given");
            }
            else
            {
                if (IsApplicationRunning())
                {
                    Engine.Reflection.Compute.RecordWarning("MidasCivil process already running");
                }
                else
                {
                    try
                    {
                        System.Diagnostics.Process.Start(filePath);
                    }
                    catch (System.ComponentModel.Win32Exception)
                    {
                        throw new Exception("File does not exist, please reference an .mcb file");
                    }
                }
                m_directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string txtFile = m_directory + "\\" + fileName + ".txt";
                string mctFile = m_directory + "\\" + fileName + ".mct";

                if (File.Exists(txtFile))
                {
                    m_midasText = File.ReadAllLines(txtFile, Encoding.GetEncoding(1252)).ToList();
                    SetSectionText();
                }
                else if (File.Exists(mctFile))
                {
                    m_midasText = File.ReadAllLines(mctFile, Encoding.GetEncoding(1252)).ToList();
                    SetSectionText();
                }

                string versionFile = m_directory + "\\TextFiles\\" + "VERSION" + ".txt";
                if (!(m_midasCivilVersion == ""))
                {
                    m_midasCivilVersion = m_midasCivilVersion.Trim();
                    if (File.Exists(versionFile))
                    {
                        Engine.Reflection.Compute.RecordWarning("*VERSION file found, user input used to overide: version =  " + m_midasCivilVersion);
                    }
                }
                else if (File.Exists(versionFile))
                {
                    List<string> versionText = GetSectionText("VERSION");
                    m_midasCivilVersion = versionText[0].Trim();
                }
                else
                {
                    m_midasCivilVersion = "8.8.1";
                    Engine.Reflection.Compute.RecordWarning("*VERSION file not found in directory and no version specified, MidasCivil version assumed default value =  " + m_midasCivilVersion);
                }

                try
                {
                    List<string> units = GetSectionText("UNIT")[0].Split(',').ToList();
                    m_forceUnit = units[0].Trim();
                    m_lengthUnit = units[1].Trim();
                    m_heatUnit = units[2].Trim();
                    m_temperatureUnit = units[3].Trim();
                }
                catch (DirectoryNotFoundException)
                {
                    Engine.Reflection.Compute.RecordWarning(
                        "No UNITS.txt file found, MidasCivil model units assumed to be Newtons, metres, kilojoules and celcius. Therefore, no unit conversion will occur when pushing and pulling to/from MidasCivil.");
                }
                catch (ArgumentOutOfRangeException)
                {
                    Engine.Reflection.Compute.RecordWarning(
                        "No UNITS.txt file found, MidasCivil model units assumed to be Newtons, metres, kilojoules and celcius. Therefore, no unit conversion will occur when pushing and pulling to/from MidasCivil.");
                }

                Directory.CreateDirectory(m_directory + "\\Results");

            }

                return true;
        }

        /***************************************************/

        public bool RunCommand(Analyse command)
        {
            Engine.Reflection.Compute.RecordWarning($"The command {command.GetType().Name} is not supported by this Adapter.");
            return false;
        }

        /***************************************************/

        public bool RunCommand(AnalyseLoadCases command)
        {
            Engine.Reflection.Compute.RecordWarning($"The command {command.GetType().Name} is not supported by this Adapter.");
            return false;
        }

        /***************************************************/

        public bool RunCommand(ClearResults command)
        {
            DirectoryInfo directory = new DirectoryInfo(m_directory + "\\Results");
            foreach (FileInfo file in directory.EnumerateFiles())
            {
                file.Delete();
            }

            return true;
        }

        /***************************************************/

        public bool RunCommand(IExecuteCommand command)
        {
            Engine.Reflection.Compute.RecordWarning($"The command {command.GetType().Name} is not supported by this Adapter.");
            return false;
        }

        /***************************************************/
        /**** Private helper methods                    ****/
        /***************************************************/
        private string GetDirectoryRoot(string directory)
        {
            List<string> directoryRoot = m_directory.Split('\\').ToList();
            directoryRoot.RemoveAt(directoryRoot.Count - 1);

            return String.Join("\\", directoryRoot.ToArray());
        }

        private static void CopyAll(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
        {
            Directory.CreateDirectory(targetDirectory.FullName);

            foreach (FileInfo file in sourceDirectory.GetFiles())
                file.CopyTo(Path.Combine(targetDirectory.FullName, file.Name));

            foreach (DirectoryInfo sourceSubDirectory in sourceDirectory.GetDirectories())
            {
                DirectoryInfo targetSubDirectory = targetDirectory.CreateSubdirectory(sourceSubDirectory.Name);
                CopyAll(sourceSubDirectory, targetSubDirectory);
            }
        }

        /***************************************************/

    }
}
