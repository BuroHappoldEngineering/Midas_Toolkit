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
using BH.oM.Structure.Loads;
using BH.oM.Geometry;
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

        private bool CreateCollection(IEnumerable<BarUniformlyDistributedLoad> barUniformlyDistributedLoads)
        {
            string loadGroupPath = CreateSectionFile("LOAD-GROUP");

            foreach (BarUniformlyDistributedLoad barUniformlyDistributedLoad in barUniformlyDistributedLoads)
            {
                List<string> midasBarLoads = new List<string>();
                string barLoadPath = CreateSectionFile(barUniformlyDistributedLoad.Loadcase.Name + "\\BEAMLOAD");
                string midasLoadGroup = Adapters.MidasCivil.Convert.FromLoadGroup(barUniformlyDistributedLoad);

                List<string> assignedBars = barUniformlyDistributedLoad.Objects.Elements.Select(x => x.AdapterId<string>(typeof(MidasCivilId))).ToList();

                List<double> loadVectors = new List<double> { barUniformlyDistributedLoad.Force.X,
                                                              barUniformlyDistributedLoad.Force.Y,
                                                              barUniformlyDistributedLoad.Force.Z,
                                                              barUniformlyDistributedLoad.Moment.X,
                                                              barUniformlyDistributedLoad.Moment.Y,
                                                              barUniformlyDistributedLoad.Moment.Z};

                Vector zeroVector = new Vector { X = 0, Y = 0, Z = 0 };

                for (int i = 0; i < 6; i++)
                {
                    barUniformlyDistributedLoad.Force = zeroVector;
                    barUniformlyDistributedLoad.Moment = zeroVector;

                    if (loadVectors[i] != 0)
                    {
                        if (i < 3)
                        {
                            barUniformlyDistributedLoad.Force = CreateSingleComponentVector(i, loadVectors[i]);

                            foreach (string assignedBar in assignedBars)
                            {
                                midasBarLoads.Add(Adapters.MidasCivil.Convert.FromBarUniformlyDistributedLoad(barUniformlyDistributedLoad, assignedBar, "Force", m_forceUnit, m_lengthUnit));
                            }
                        }
                        else
                        {
                            barUniformlyDistributedLoad.Moment = CreateSingleComponentVector(i - 3, loadVectors[i]);

                            foreach (string assignedBar in assignedBars)
                            {
                                midasBarLoads.Add(Adapters.MidasCivil.Convert.FromBarUniformlyDistributedLoad(barUniformlyDistributedLoad, assignedBar, "Moment", m_forceUnit, m_lengthUnit));
                            }
                        }

                    }
                }

                CompareLoadGroup(midasLoadGroup, loadGroupPath);
                RemoveEndOfDataString(barLoadPath);
                File.AppendAllLines(barLoadPath, midasBarLoads, Encoding.GetEncoding(1252));
            }
            return true;
        }

        /***************************************************/

    }
}

