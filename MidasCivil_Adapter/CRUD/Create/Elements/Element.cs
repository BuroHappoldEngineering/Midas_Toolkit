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

using BH.oM.Adapters.MidasCivil;
using BH.Engine.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Structure;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private bool CreateCollection(IEnumerable<Bar> bars)
        {
            string path = CreateSectionFile("ELEMENT");
            List<string> midasElements = new List<string>();

            CreateGroups(bars);

            foreach (Bar bar in bars)
            {
                if (new string(bar.Release.DescriptionOrName().Replace(",", "").Take(m_groupCharacterLimit).ToArray()) != "FixFix" && bar.FEAType == BarFEAType.TensionOnly)
                {
                    Engine.Reflection.Compute.RecordError("Tension only elements cannot support bar releases in Midas");
                }

                if (!(bar.Release == null) && new string(bar.Release.DescriptionOrName().Replace(",", "").Take(m_groupCharacterLimit).ToArray()) != "FixFix")
                {
                    AssignBarRelease(bar.AdapterId<string>(typeof(MidasCivilId)), new string(bar.Release.DescriptionOrName().Replace(",", "").Take(m_groupCharacterLimit).ToArray()), "FRAME-RLS");
                }

                midasElements.Add(Adapters.MidasCivil.Convert.FromBar(bar));
            }

            File.AppendAllLines(path, midasElements, Encoding.GetEncoding(1252));

            return true;
        }

        /***************************************************/

        private bool CreateCollection(IEnumerable<FEMesh> meshes)
        {
            string path = CreateSectionFile("ELEMENT");
            List<string> midasElements = new List<string>();

            int index = System.Convert.ToInt32(NextFreeId(typeof(FEMesh)));

            foreach (FEMesh mesh in meshes)
            {
                midasElements.Add(Adapters.MidasCivil.Convert.FromFEMesh(mesh, ref index));
            }

            CreateGroups(meshes);

            File.AppendAllLines(path, midasElements, Encoding.GetEncoding(1252));

            return true;
        }

        /***************************************************/

    }
}
