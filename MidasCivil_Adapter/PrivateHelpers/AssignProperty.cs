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
using System.Linq;
using System.IO;
using System.Text;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private void AssignProperty(string bhomID, string propertyName, string section)
        {
            string path = m_directory + "\\TextFiles\\" + section + ".txt";

            List<string> propertyText = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();

            int index = propertyText.FindIndex(x => x.Contains(propertyName));

            string constraint = propertyText[index];

            string[] split = constraint.Split(',');

            string assignmentList = split[0];

            if (!(string.IsNullOrWhiteSpace(assignmentList)))
            {
                List<string> assignmentRanges = new List<string>();

                assignmentRanges = assignmentList.Split(' ').
                    Select(x => x.Trim()).
                    Where(x => !string.IsNullOrEmpty(x)).
                    ToList();

                List<int> assignments = MidasCivilAdapter.GetAssignmentIds(assignmentRanges);
                assignments.Add(int.Parse(bhomID));

                split[0] = MidasCivilAdapter.CreateAssignmentString(assignments);
            }
            else
            {
                split[0] = bhomID;
            }

            string updatedProperty = split[0];

            for (int i = 1; i < split.Count(); i++)
            {
                updatedProperty = updatedProperty + "," + split[i];
            }

            propertyText[index] = updatedProperty;

            using (StreamWriter sectionText = new StreamWriter(path, false, Encoding.GetEncoding(1252), 65536))
            {
                foreach (string property in propertyText)
                {
                    sectionText.WriteLine(property);
                }
                sectionText.Close();
            }
        }

        /***************************************************/

    }
}

