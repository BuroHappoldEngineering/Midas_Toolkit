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

using BH.oM.Geometry;
using BH.oM.Spatial.ShapeProfiles;
using System;
using System.Collections.Generic;

namespace BH.Adapter.Adapters.MidasCivil
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IProfile ToProfile(List<string> sectionProfile, string shape, string lengthUnit)
        {
            IProfile bhomProfile = null;
            double width;
            double webSpacing;
            double webThickness;
            double topFlangeThickness;
            double bottomFlangeThickness;
            double topFlangeWidth;
            double bottomFlangeWidth;

            switch (shape)
            {
                case "SB":
                    bhomProfile = Engine.Spatial.Create.RectangleProfile(
                        System.Convert.ToDouble(sectionProfile[0]).LengthToSI(lengthUnit), System.Convert.ToDouble(sectionProfile[1]).LengthToSI(lengthUnit), 0);
                    break;
                case "B":
                    width = System.Convert.ToDouble(sectionProfile[1]).LengthToSI(lengthUnit);
                    webSpacing = System.Convert.ToDouble(sectionProfile[4]).LengthToSI(lengthUnit);
                    webThickness = System.Convert.ToDouble(sectionProfile[2]).LengthToSI(lengthUnit);
                    double corbel;
                    if (webSpacing < Tolerance.Distance || System.Math.Abs(width - webSpacing - webThickness) < Tolerance.Distance)
                        corbel = 0;
                    else
                    {
                        corbel = (width / 2 - webSpacing / 2 - webThickness / 2);
                        width = (webSpacing + webThickness);
                    }

                    topFlangeThickness = System.Convert.ToDouble(sectionProfile[3]).LengthToSI(lengthUnit);
                    bottomFlangeThickness = System.Convert.ToDouble(sectionProfile[5]).LengthToSI(lengthUnit);
                    if (bottomFlangeThickness < Tolerance.Distance)
                        bottomFlangeThickness = topFlangeThickness;

                    bhomProfile = Engine.Spatial.Create.GeneralisedFabricatedBoxProfile(
                            System.Convert.ToDouble(sectionProfile[0]).LengthToSI(lengthUnit), width, webThickness,
                            topFlangeThickness, bottomFlangeThickness,
                            corbel, corbel);
                    break;
                case "P":
                    bhomProfile = Engine.Spatial.Create.TubeProfile(System.Convert.ToDouble(sectionProfile[0]).LengthToSI(lengthUnit),
                        System.Convert.ToDouble(sectionProfile[1]).LengthToSI(lengthUnit));
                    break;
                case "SR":
                    bhomProfile = Engine.Spatial.Create.CircleProfile(
                         System.Convert.ToDouble(sectionProfile[0]).LengthToSI(lengthUnit));
                    break;
                case "H":
                    topFlangeWidth = System.Convert.ToDouble(sectionProfile[1]).LengthToSI(lengthUnit);
                    bottomFlangeWidth = System.Convert.ToDouble(sectionProfile[4]).LengthToSI(lengthUnit);
                    topFlangeThickness = System.Convert.ToDouble(sectionProfile[3]).LengthToSI(lengthUnit);
                    bottomFlangeThickness = System.Convert.ToDouble(sectionProfile[5]).LengthToSI(lengthUnit);

                    if (bottomFlangeWidth < Tolerance.Distance)
                        bottomFlangeWidth = topFlangeWidth;
                    if (bottomFlangeThickness < Tolerance.Distance)
                        bottomFlangeThickness = topFlangeThickness;

                    bhomProfile = Engine.Spatial.Create.FabricatedISectionProfile(
                        System.Convert.ToDouble(sectionProfile[0]).LengthToSI(lengthUnit), topFlangeWidth,
                         bottomFlangeWidth, System.Convert.ToDouble(sectionProfile[2]).LengthToSI(lengthUnit),
                        topFlangeThickness, bottomFlangeThickness, System.Convert.ToDouble(sectionProfile[6]).LengthToSI(lengthUnit));
                    break;
                case "T":
                    bhomProfile = Engine.Spatial.Create.TSectionProfile(
                        System.Convert.ToDouble(sectionProfile[0]).LengthToSI(lengthUnit), System.Convert.ToDouble(sectionProfile[1]).LengthToSI(lengthUnit),
                        System.Convert.ToDouble(sectionProfile[2]).LengthToSI(lengthUnit), System.Convert.ToDouble(sectionProfile[3]).LengthToSI(lengthUnit),
                        0, 0);
                    break;
                case "C":
                    topFlangeWidth = System.Convert.ToDouble(sectionProfile[1]);
                    bottomFlangeWidth = System.Convert.ToDouble(sectionProfile[4]);
                    topFlangeThickness = System.Convert.ToDouble(sectionProfile[3]);
                    bottomFlangeThickness = System.Convert.ToDouble(sectionProfile[5]);

                    if(Math.Abs(topFlangeWidth - bottomFlangeWidth) > Tolerance.Distance && bottomFlangeWidth > Tolerance.Distance)
                    {
                        Engine.Reflection.Compute.RecordWarning("Asymmetric channel sections are not yet supported by the BHoM. The top flange width will be used for the profile.");
                    }
                    
                    if(Math.Abs(topFlangeThickness - bottomFlangeThickness) > Tolerance.Distance && bottomFlangeThickness > Tolerance.Distance)
                    {
                        Engine.Reflection.Compute.RecordWarning("Asymmetric channel sections are not yet supported by the BHoM. The top flange thickness will be used for the profile.");
                    }


                    bhomProfile = Engine.Spatial.Create.ChannelProfile(
                            System.Convert.ToDouble(sectionProfile[0]).LengthToSI(lengthUnit), topFlangeWidth,
                            System.Convert.ToDouble(sectionProfile[2]).LengthToSI(lengthUnit), topFlangeThickness,
                            System.Convert.ToDouble(sectionProfile[6]).LengthToSI(lengthUnit), System.Convert.ToDouble(sectionProfile[7]).LengthToSI(lengthUnit));
                    break;
                case "L":
                    bhomProfile = Engine.Spatial.Create.AngleProfile(
                            System.Convert.ToDouble(sectionProfile[0]).LengthToSI(lengthUnit), System.Convert.ToDouble(sectionProfile[1]).LengthToSI(lengthUnit),
                            System.Convert.ToDouble(sectionProfile[2]).LengthToSI(lengthUnit), System.Convert.ToDouble(sectionProfile[3]).LengthToSI(lengthUnit),
                            0, 0, false, true);
                    break;
            }

            if (shape.Contains("TAPERED"))
            {
                string shapeCode = shape.Split('-')[1].Trim();
                int interpolationOrder = System.Convert.ToInt32(shape.Split('-')[2].Trim());
                int midIndex = sectionProfile.Count / 2;
                IProfile startProfile = Convert.ToProfile(sectionProfile.GetRange(0, midIndex), shapeCode, lengthUnit);
                IProfile endProfile = Convert.ToProfile(sectionProfile.GetRange(midIndex, midIndex), shapeCode, lengthUnit);
                bhomProfile = Engine.Spatial.Create.TaperedProfile(startProfile, endProfile, interpolationOrder);
            }

            return bhomProfile;
        }

        /***************************************************/

    }
}

