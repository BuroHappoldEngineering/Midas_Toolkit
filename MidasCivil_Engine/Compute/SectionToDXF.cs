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
using System;
using BH.oM.Structure.SectionProperties;
using netDxf;
using BH.oM.Geometry;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;

namespace BH.Engine.Adapters.MidasCivil
{
    public static partial class Compute
    {
        public static bool sectionToDXF(IGeometricalSection section, string directory, string name, bool activate)
        {
            if (activate)
            {
                DxfDocument DXFdoc = new DxfDocument();
                netDxf.AciColor colour = GetColourFromType(section);
                DXFdoc = profiletoDxfDocument(section.SectionProfile, DXFdoc, colour, name);


                netDxf.Entities.MText MText = new netDxf.Entities.MText(PropertyTextBlock(section.Material));
                netDxf.Tables.Layer pLayer = new netDxf.Tables.Layer(name + "Properties");
                pLayer.IsVisible = false;
                MText.Layer = pLayer;
                pLayer.Color = AciColor.DarkGray;

                DXFdoc.AddEntity(MText);

                DXFdoc.Save(directory + "\\" + name + ".dxf");
                return true;

            }
            return false;
        }

        public static bool shapeProfileToDXF(IProfile profile, string directory, string name, bool activate)
        {
            if (activate)
            {
                DxfDocument DXFdoc = new DxfDocument();
                netDxf.AciColor colour = AciColor.LightGray;
                DXFdoc = profiletoDxfDocument(profile, DXFdoc, colour, name);
                DXFdoc.Save(directory + "\\" + name + ".dxf");
                return true;
            }
            return false;
        }

        private static netDxf.AciColor GetColourFromType(IGeometricalSection section)
        {
            if (section.GetType() == typeof(SteelSection))
            {
                return AciColor.Red;
            }
            else if (section.GetType() == typeof(ConcreteSection))
            {
                return AciColor.Green;
            }
            else if (section.GetType() == typeof(AluminiumSection))
            {
                return AciColor.Yellow;
            }
            else if (section.GetType() == typeof(CableSection))
            {
                return AciColor.Blue;
            }
            else
            {
                return AciColor.LightGray;
            }
        }

        private static netDxf.Vector3 DXFPoint(Point point)
        {
            netDxf.Vector3 point_ = new netDxf.Vector3(point.X * 1000, point.Y * 1000, point.Z * 1000);
            return point_;
        }
        private static netDxf.Entities.Line DXFLine(Line line)
        {
            netDxf.Entities.Line line_ = new netDxf.Entities.Line(DXFPoint(line.Start), DXFPoint(line.End));
            return line_;
        }
        private static netDxf.Entities.Circle DXFCircle(Circle circle)
        {
            netDxf.Entities.Circle circle_ = new netDxf.Entities.Circle(DXFPoint(circle.Centre), circle.Radius * 1000);
            return circle_;
        }
        private static netDxf.Entities.Polyline DXFPolyline(Polyline polyline)
        {
            List<netDxf.Vector3> points = new List<netDxf.Vector3>();
            foreach (Point point in polyline.ControlPoints)
            {
                points.Add(DXFPoint(point));
            }
            netDxf.Entities.Polyline polyline_ = new netDxf.Entities.Polyline(points);
            return polyline_;
        }
        private static netDxf.Entities.Polyline DXFArc(Arc arc)
        {
            List<Point> points = Geometry.Convert.ToNurbsCurve(arc).ControlPoints;
            List<double> weights = Geometry.Convert.ToNurbsCurve(arc).Weights;

            int i = points.Count - 1;
            //netDxf.Entities.Polyline arc_ = new netDxf.Entities.Polyline;
            netDxf.Entities.Polyline arc_ = new netDxf.Entities.Polyline
                (
                new List<netDxf.Vector3>
                    {
                    DXFPoint(points[0]),
                    DXFPoint(points[i])
                    }
                );
            Engine.Reflection.Compute.RecordWarning("Arc shapes are currently not supported");
            return arc_;

        }
        private static string PropertyTextBlock(IMaterialFragment material)
        {
            string properties = "MATERIAL PROPERTIES\\P" +
                $"Name:   {material.Name}\\P" +
                $"Damping Ratio:   {material.DampingRatio.ToString()}\\P" +
                $"Density:   {material.Density.ToString()}\\P";

            if (material is IIsotropic)
            {
                IIsotropic isotropic = material as IIsotropic;
                properties += 
                    $"Poissons Ratio:   {isotropic.PoissonsRatio}\\P" +
                    $"Thermal Expansion Coefficient:   {isotropic.ThermalExpansionCoeff}\\P" +
                    $"Youngs Modulus:   {isotropic.YoungsModulus}\\P";
            }
            else if (material is IOrthotropic)
            {
                IOrthotropic orthotropic = material as IOrthotropic;
                properties +=
                    $"Poissons Ratio X:   {orthotropic.PoissonsRatio.X}\\P" +
                    $"Poissons Ratio Y:   {orthotropic.PoissonsRatio.Y}\\P" +
                    $"Poissons Ratio Z:   {orthotropic.PoissonsRatio.Z}\\P" +
                    $"Thermal Expansion Coefficient X:   {orthotropic.ThermalExpansionCoeff.X}\\P" +
                    $"Thermal Expansion Coefficient Y:   {orthotropic.ThermalExpansionCoeff.Y}\\P" +
                    $"Thermal Expansion Coefficient Z:   {orthotropic.ThermalExpansionCoeff.Z}\\P" +
                    $"Youngs Modulus X:   {orthotropic.YoungsModulus.X}\\P" +
                    $"Youngs Modulus Y:   {orthotropic.YoungsModulus.Y}\\P" +
                    $"Youngs Modulus Z:   {orthotropic.YoungsModulus.Z}\\P";
            }

            return properties;
        }

        private static DxfDocument profiletoDxfDocument (IProfile profile, DxfDocument DXFdoc, AciColor colour, string name)
        {
            IEnumerable<ICurve> edges = profile.Edges;

            netDxf.Tables.Layer layer = new netDxf.Tables.Layer(name);
            layer.Color = colour;

            foreach (BH.oM.Geometry.ICurve edge in edges)
            {
                if (edge.GetType() == typeof(Line))
                {
                    netDxf.Entities.Line line = DXFLine((Line)edge);
                    line.Layer = layer;
                    DXFdoc.AddEntity(line);
                }

                else if (edge.GetType() == typeof(Polyline))
                {
                    netDxf.Entities.Polyline line = DXFPolyline((Polyline)edge);
                    line.Layer = layer;
                    DXFdoc.AddEntity(line);
                }

                else if (edge.GetType() == typeof(Circle))
                {
                    netDxf.Entities.Circle line = DXFCircle((Circle)edge);
                    line.Layer = layer;
                    DXFdoc.AddEntity(line);
                }
                else if (edge.GetType() == typeof(Arc))
                {
                    netDxf.Entities.Polyline line = DXFArc((Arc)edge);
                    line.Layer = layer;
                    DXFdoc.AddEntity(line);
                }
                else if (edge.GetType() == typeof(PolyCurve))
                {
                    foreach (ICurve curve in ((PolyCurve)edge).Curves)
                    {
                        if (edge.GetType() == typeof(Line))
                        {
                            netDxf.Entities.Line line = DXFLine((Line)edge);
                            line.Layer = layer;
                            DXFdoc.AddEntity(line);
                        }

                        else if (edge.GetType() == typeof(Polyline))
                        {
                            netDxf.Entities.Polyline line = DXFPolyline((Polyline)edge);
                            line.Layer = layer;
                            DXFdoc.AddEntity(line);
                        }
                        else if (edge.GetType() == typeof(Arc))
                        {
                            netDxf.Entities.Polyline line = DXFArc((Arc)edge);
                            line.Layer = layer;
                            DXFdoc.AddEntity(line);
                        }
                    }
                }
            }
            DXFdoc.DrawingVariables.AUnits = netDxf.Units.AngleUnitType.DecimalDegrees;
            DXFdoc.DrawingVariables.LUnits = netDxf.Units.LinearUnitType.Decimal;
            DXFdoc.DrawingVariables.InsUnits = netDxf.Units.DrawingUnits.Millimeters;
            Engine.Reflection.Compute.RecordWarning(".dxf file units have been set to milimeters");

            return DXFdoc;
        }
    }
}

