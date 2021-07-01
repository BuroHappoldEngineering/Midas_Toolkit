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
using BH.oM.Structure.SectionProperties;
using netDxf;
using BH.oM.Geometry;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;
using System;

namespace BH.Engine.Adapters.MidasCivil
{
    public static partial class Create
    {
        public static GenericSection DXFToGenericSection(string filePath, string name, bool activate)
        {
            if (activate)
            {
                GenericSection section = Structure.Create.GenericSectionFromProfile(DXFToFreeFormProfile(filePath, name, true), DXFMaterialProperties(filePath), name);
                Engine.Reflection.Compute.RecordWarning(".dxf file units are assumed to be in milimeters and will be converted to meters");
                return section;
            }
            else
            {
                return null;
            }
        }

        public static FreeFormProfile DXFToFreeFormProfile(string filePath, string name, bool activate)
        {
            if (activate)
            {
                DxfDocument DXFdoc = netDxf.DxfDocument.Load(filePath);
                List<ICurve> edges = new List<ICurve>();

                foreach (netDxf.Entities.Line line in DXFdoc.Lines)
                {
                    edges.Add(BHOMLine(line));
                }
                foreach (netDxf.Entities.Polyline line in DXFdoc.Polylines)
                {
                    edges.Add(BHOMPolyline(line));
                }
                foreach (netDxf.Entities.Circle circle in DXFdoc.Circles)
                {
                    edges.Add(BHOMCircle(circle));
                }
                foreach(netDxf.Entities.Arc arc in DXFdoc.Arcs)
                {
                    edges.Add(BHOMArc(arc));
                }
                foreach (netDxf.Entities.Spline spline in DXFdoc.Splines)
                {
                    edges.Add(BHOMSpline(spline));
                }

                FreeFormProfile profile = new FreeFormProfile(edges);
                Engine.Reflection.Compute.RecordWarning(".dxf file units are assumed to be in milimeters and will be converted to meters");
                return profile;
            }
            else { return null; }
        }
        private static Point BHOMPoint(netDxf.Vector3 point)
        {
            Point point_ = new Point();
            point_.X = point.X / 1000;
            point_.Y = point.Y / 1000;
            point_.Z = point.Z / 1000;
            return point_;
        }
        private static Line BHOMLine(netDxf.Entities.Line line)
        {
            Line line_ = new Line();
            line_.Start = BHOMPoint(line.StartPoint);
            line_.End = BHOMPoint(line.EndPoint);
            return line_;
        }

        private static Polyline BHOMPolyline(netDxf.Entities.Polyline line)
        {
            List<Point> points = new List<Point>();
            foreach (netDxf.Entities.PolylineVertex point in line.Vertexes)
            {
                points.Add(BHOMPoint(point.Position));
            }
            Polyline line_ = new Polyline();
            line_.ControlPoints = points;
            return line_;
        }

        private static Circle BHOMCircle(netDxf.Entities.Circle circle)
        {
            Point centre = BHOMPoint(circle.Center);
            double radius = circle.Radius / 1000;

            Circle circle_ = new Circle();
            circle_.Radius = radius;
            circle_.Centre = centre;
            return circle_;
        }

        private static Arc BHOMArc(netDxf.Entities.Arc arc)
        {
            List<Vector2> points = arc.PolygonalVertexes(2);
            Arc arc_ = Geometry.Create.Arc(
                BHOMPoint(new Vector3(points[0].X, points[0].Y, 0) + arc.Center),
                BHOMPoint(new Vector3(points[1].X, points[1].Y, 0) + arc.Center),
                BHOMPoint(new Vector3(points[2].X, points[2].Y, 0) + arc.Center)
                );
            return arc_;
        }

        private static ICurve BHOMSpline(netDxf.Entities.Spline spline)
        {
            Engine.Reflection.Compute.RecordWarning("Spline has been converted to polyline");
            if(spline.ControlPoints.Count != 0)
            {
                return BHOMPolyline(spline.ToPolyline(36));
            }
            else
            {
                Polyline polyline_ = new Polyline();
                List<Point> points_ = new List<Point>();
                List<Vector3> points = spline.FitPoints;
                foreach(Vector3 point in points)
                {
                    points_.Add(BHOMPoint(point));
                }
                polyline_.ControlPoints = points_;
                return polyline_;
            }
                
        }

        private static IMaterialFragment DXFMaterialProperties(string filePath)
        {
            DxfDocument DXFDoc = DxfDocument.Load(filePath);
            foreach (netDxf.Entities.MText text in DXFDoc.MTexts)
            {
                string text_ = text.Value.Replace(" ", "");
                if (text_.Contains("MATERIALPROPERTIES"))
                {
                    if (text_.Contains("PoissonsRatioX") || text_.Contains("PoissonsRatioY") || text_.Contains("PoissonsRatioZ") ||
                        text_.Contains("ThermalExpansionCoefficientX") || text_.Contains("ThermalExpansionCoefficientY") || text_.Contains("ThermalExpansionCoefficientZ") ||
                        text_.Contains("YoungsModulusX") || text_.Contains("YoungsModulusY") || text_.Contains("YoungsModulusZ"))
                    {
                        GenericOrthotropicMaterial material = new GenericOrthotropicMaterial();
                        string[] properties = text_.Split(new string[] { "\\P" }, StringSplitOptions.None);
                        foreach (string property in properties)
                        {
                            if (property.Contains("Name")) { material.Name = property.Split(':')[1]; }
                            else if (property.Contains("DampingRatio")) { material.DampingRatio = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("Density")) { material.Density = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("PoissonsRatioX")) { material.PoissonsRatio.X = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("PoissonsRatioY")) { material.PoissonsRatio.Y = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("PoissonsRatioZ")) { material.PoissonsRatio.Z = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("ThermalExpansionCoefficientX")) { material.ThermalExpansionCoeff.X = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("ThermalExpansionCoefficientY")) { material.ThermalExpansionCoeff.Y = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("ThermalExpansionCoefficientY")) { material.ThermalExpansionCoeff.Z = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("YoungsModulusX")) { material.YoungsModulus.X = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("YoungsModulusY")) { material.YoungsModulus.Y = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("YoungsModulusZ")) { material.YoungsModulus.Z = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                        }
                        return material;
                    }
                    else
                    {
                        GenericIsotropicMaterial material = new GenericIsotropicMaterial();
                        string[] properties = text_.Split(new string[] { "\\P" }, StringSplitOptions.None);
                        foreach (string property in properties)
                        {
                            if (property.Contains("Name")) { material.Name = property.Split(':')[1]; }
                            else if (property.Contains("DampingRatio")) { material.DampingRatio = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("Density")) { material.Density = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("PoissonsRatio")) { material.PoissonsRatio = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("ThermalExpansionCoefficient")) { material.ThermalExpansionCoeff = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                            else if (property.Contains("YoungsModulus")) { material.YoungsModulus = double.Parse(property.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture); }
                        }
                        return material;
                    }
                }
            }
            Engine.Reflection.Compute.RecordError("No material properties are associated with this dxf file");
            return null;
        }
    }
}


