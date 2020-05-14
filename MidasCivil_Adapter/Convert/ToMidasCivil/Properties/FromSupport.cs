﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using System.IO;
using System.Collections.Generic;
using BH.oM.Structure.Constraints;
using BH.Adapter.MidasCivil;

namespace BH.Adapter.Adapters.MidasCivil
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string FromSupport(this Constraint6DOF constraint6DOF)
        {
            string midasSupport = (
                 " " + "," +
                 SupportString(constraint6DOF) + "," +
                 constraint6DOF.Name
                 );

            return midasSupport;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string SupportString(Constraint6DOF constraint6DOF)
        {
            List<DOFType> freedoms = new List<DOFType>
            {
                constraint6DOF.TranslationX, constraint6DOF.TranslationY, constraint6DOF.TranslationZ,
                constraint6DOF.RotationX, constraint6DOF.RotationY, constraint6DOF.RotationZ
            };

            string support = "";

            foreach (DOFType freedom in freedoms)
            {
                if (MidasCivilAdapter.GetSupportedDOFType(freedom))
                {
                    if (freedom == DOFType.Fixed)
                    {
                        support = support + "1";
                    }
                    else if (freedom == DOFType.Free)
                    {
                        support = support + "0";
                    }
                    else
                    {
                        Engine.Reflection.Compute.RecordWarning(
                                     "Unsupported DOFType in " + constraint6DOF.Name + " assumed to be" + DOFType.Free);
                        support = support + "0";
                    }
                }
            }
            return support;

        }

        /***************************************************/

    }
}