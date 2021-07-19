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
        /**** Private Methods                           ****/
        /***************************************************/

        private List<string> GetSectionText(string section)
        {
            string path = m_directory + "\\TextFiles\\" + section + ".txt";
            List<string> sectionText = new List<string>();

            if (File.Exists(path))
            {
                sectionText = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();
                CleanString(ref sectionText);
            }
            return sectionText;
        }

        /***************************************************/

        private static void CleanString(ref List<string> sectionText)
        {
            List<string> cleanString = new List<string>();

            for (int i = 0; i < sectionText.Count; i++)
            {
                if (!(sectionText[i].Contains(";")) && !(sectionText[i].Contains("*")) && !(string.IsNullOrEmpty(sectionText[i])))
                {
                    if (sectionText[i].Contains("\\"))
                    {
                        string combined = sectionText[i].Replace("\\", "");
                        bool finished = false;
                        while (!finished)
                        {
                            i++;
                            if (!sectionText[i].Contains("\\"))
                            {
                                combined = combined + sectionText[i];
                                finished = true;
                            }
                            else
                            {
                                combined = combined + sectionText[i].Replace("\\", "");
                            }
                        }
                        cleanString.Add(combined);
                    }
                    else
                    {
                        cleanString.Add(sectionText[i]);
                    }
                }
            }

            sectionText = cleanString;
        }

        /***************************************************/

    }
}

