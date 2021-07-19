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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        /***************************************************/
        /**** Prviate Methods                           ****/
        /***************************************************/

        private string CreateSectionFile(string section)
        {
            string newFolder = m_directory + "\\TextFiles\\";
            System.IO.Directory.CreateDirectory(newFolder);
            string path = newFolder + "\\" + section + ".txt";

            if (section.Contains("\\"))
            {
                string[] delimitted = section.Split('\\');
                section = delimitted[delimitted.Count() - 1];
            }

            if (!File.Exists(path))
            {
                using (StreamWriter sectionText = new StreamWriter(path, false, Encoding.GetEncoding(1252), 65536))
                {
                    if (section != "SELFWEIGHT")
                    {
                        sectionText.WriteLine("*" + section);
                    }
                    sectionText.Close();
                }
            }
            else
            {
                List<string> readSection = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();
                if (readSection.Count != 0)
                {
                    if (!(readSection[0].Contains("*" + section)))
                    {
                        using (StreamWriter sectionText = new StreamWriter(path, false, Encoding.GetEncoding(1252), 65536))
                        {
                            if (section != "SELFWEIGHT")
                            {
                                sectionText.WriteLine("*" + section);
                            }
                            sectionText.Close();
                        }
                    }
                }
            }

            return path;
        }

        /***************************************************/

    }
}
