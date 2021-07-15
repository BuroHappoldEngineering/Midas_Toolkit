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
using System.Collections.Generic;
using BH.oM.Adapters.MidasCivil;
using BH.Engine.Adapter;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.Engine.Structure;
using System.Linq;
using BH.oM.Structure.MaterialFragments;

namespace BH.Adapter.Adapters.MidasCivil
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<string> FromSectionProperty(this ISectionProperty sectionProperty, string lengthUnit, int sectionPropertyCharacterLimit)
        {
            return CreateSection(sectionProperty as dynamic, lengthUnit, sectionPropertyCharacterLimit) ?? null;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<string> CreateSection(SteelSection sectionProperty, string lengthUnit, int sectionPropertyCharacterLimit)
        {
            List<string> midasSectionProperty = new List<string>();
            if (sectionProperty.SectionProfile is TaperedProfile)
            {
                TaperedProfile taperedProfile = (TaperedProfile)sectionProperty.SectionProfile;
                List<IProfile> profiles = new List<IProfile>(taperedProfile.Profiles.Values);
                if (profiles.Any(x => x.Shape != profiles.First().Shape))
                {
                    Engine.Reflection.Compute.RecordError("MidasCivil_Toolkit does not support TaperedProfiles with different section shapes.");
                    return null;
                }
                else
                {
                    midasSectionProperty.Add(sectionProperty.AdapterId<string>(typeof(MidasCivilId)) + ",TAPERED," +
                        new string(sectionProperty.DescriptionOrName().Replace(",", "").Take(sectionPropertyCharacterLimit).ToArray()) +
                        ",CC, 0,0,0,0,0,0,0,0,YES,NO," + GetSectionShapeCode(sectionProperty) + "," + GetInterpolationOrder(sectionProperty) +
                        "," + GetInterpolationOrder(sectionProperty) + ",USER");
                    midasSectionProperty.Add(CreateProfile(sectionProperty.SectionProfile as dynamic, lengthUnit));
                }
            }
            else
            {
                midasSectionProperty.Add(sectionProperty.AdapterId<string>(typeof(MidasCivilId)) + ",DBUSER," +
                    new string(sectionProperty.DescriptionOrName().Replace(",", "").Take(sectionPropertyCharacterLimit).ToArray()) +
                    ",CC, 0, 0, 0, 0, 0, 0, YES, NO," + CreateProfile(sectionProperty.SectionProfile as dynamic, lengthUnit));
            }

            return midasSectionProperty;
        }

        /***************************************************/

        private static List<string> CreateSection(ConcreteSection sectionProperty, string lengthUnit, int sectionPropertyCharacterLimit)
        {
            List<string> midasSectionProperty = new List<string>();
            if (sectionProperty.SectionProfile is TaperedProfile)
            {
                TaperedProfile taperedProfile = (TaperedProfile)sectionProperty.SectionProfile;
                List<IProfile> profiles = new List<IProfile>(taperedProfile.Profiles.Values);
                if (profiles.Any(x => x.Shape != profiles.First().Shape))
                {
                    Engine.Reflection.Compute.RecordError("MidasCivil_Toolkit does not support TaperedProfiles with different section shapes.");
                    return null;
                }
                else
                {
                    midasSectionProperty.Add(sectionProperty.AdapterId<string>(typeof(MidasCivilId)) + ",TAPERED," +
                        new string(sectionProperty.DescriptionOrName().Replace(",", "").Take(sectionPropertyCharacterLimit).ToArray()) +
                        ",CC, 0,0,0,0,0,0,0,0,YES,NO," + GetSectionShapeCode(sectionProperty) + "," + GetInterpolationOrder(sectionProperty) +
                        "," + GetInterpolationOrder(sectionProperty) + ",USER");
                    midasSectionProperty.Add(CreateProfile(sectionProperty.SectionProfile as dynamic, lengthUnit));
                }
            }
            else
            {
                midasSectionProperty.Add(sectionProperty.AdapterId<string>(typeof(MidasCivilId)) + ",DBUSER," +
                    new string(sectionProperty.DescriptionOrName().Replace(",", "").Take(sectionPropertyCharacterLimit).ToArray()) +
                    ",CC, 0, 0, 0, 0, 0, 0, YES, NO," + CreateProfile(sectionProperty.SectionProfile as dynamic, lengthUnit));
            }

            return midasSectionProperty;
        }

        /***************************************************/

        private static List<string> CreateSection(TimberSection sectionProperty, string lengthUnit, int sectionPropertyCharacterLimit)
        {
            List<string> midasSectionProperty = new List<string>();
            if (sectionProperty.SectionProfile is TaperedProfile)
            {
                TaperedProfile taperedProfile = (TaperedProfile)sectionProperty.SectionProfile;
                List<IProfile> profiles = new List<IProfile>(taperedProfile.Profiles.Values);
                if (profiles.Any(x => x.Shape != profiles.First().Shape))
                {
                    Engine.Reflection.Compute.RecordError("MidasCivil_Toolkit does not support TaperedProfiles with different section shapes.");
                    return null;
                }
                else
                {
                    midasSectionProperty.Add(sectionProperty.AdapterId<string>(typeof(MidasCivilId)) + ",TAPERED," +
                        new string(sectionProperty.DescriptionOrName().Replace(",", "").Take(sectionPropertyCharacterLimit).ToArray()) +
                        ",CC, 0,0,0,0,0,0,0,0,YES,NO," + GetSectionShapeCode(sectionProperty) + "," + GetInterpolationOrder(sectionProperty) +
                        "," + GetInterpolationOrder(sectionProperty) + ",USER");
                    midasSectionProperty.Add(CreateProfile(sectionProperty.SectionProfile as dynamic, lengthUnit));
                }
            }
            else
            {
                midasSectionProperty.Add(sectionProperty.AdapterId<string>(typeof(MidasCivilId)) + ",DBUSER," +
                    new string(sectionProperty.DescriptionOrName().Replace(",", "").Take(sectionPropertyCharacterLimit).ToArray()) +
                    ",CC, 0, 0, 0, 0, 0, 0, YES, NO," + CreateProfile(sectionProperty.SectionProfile as dynamic, lengthUnit));
            }

            return midasSectionProperty;
        }

        /***************************************************/

        private static List<string> CreateSection(GenericSection sectionProperty, string lengthUnit, int sectionPropertyCharacterLimit)
        {
            List<string> midasSectionProperty = new List<string>();
            if (sectionProperty.SectionProfile is TaperedProfile)
            {
                TaperedProfile taperedProfile = (TaperedProfile)sectionProperty.SectionProfile;
                List<IProfile> profiles = new List<IProfile>(taperedProfile.Profiles.Values);
                if (profiles.Any(x => x.Shape != profiles.First().Shape))
                {
                    Engine.Reflection.Compute.RecordError("MidasCivil_Toolkit does not support TaperedProfiles with different section shapes.");
                    return null;
                }
                else
                {
                    midasSectionProperty.Add(sectionProperty.AdapterId<string>(typeof(MidasCivilId)) + ",TAPERED," +
                        new string(sectionProperty.DescriptionOrName().Replace(",", "").Take(sectionPropertyCharacterLimit).ToArray()) +
                        ",CC, 0,0,0,0,0,0,0,0,YES,NO," + GetSectionShapeCode(sectionProperty) + "," + GetInterpolationOrder(sectionProperty) +
                        "," + GetInterpolationOrder(sectionProperty) + ",USER");
                    midasSectionProperty.Add(CreateProfile(sectionProperty.SectionProfile as dynamic, lengthUnit));
                }
            }
            else
            {
                midasSectionProperty.Add(sectionProperty.AdapterId<string>(typeof(MidasCivilId)) + ",DBUSER," +
                    new string(sectionProperty.DescriptionOrName().Replace(",", "").Take(sectionPropertyCharacterLimit).ToArray()) +
                    ",CC, 0, 0, 0, 0, 0, 0, YES, NO," + CreateProfile(sectionProperty.SectionProfile as dynamic, lengthUnit));
            }

            return midasSectionProperty;
        }

        /***************************************************/

        private static List<string> CreateSection(AluminiumSection sectionProperty, string lengthUnit, int sectionPropertyCharacterLimit)
        {
            List<string> midasSectionProperty = new List<string>();
            if (sectionProperty.SectionProfile is TaperedProfile)
            {
                TaperedProfile taperedProfile = (TaperedProfile)sectionProperty.SectionProfile;
                List<IProfile> profiles = new List<IProfile>(taperedProfile.Profiles.Values);
                if (profiles.Any(x => x.Shape != profiles.First().Shape))
                {
                    Engine.Reflection.Compute.RecordError("MidasCivil_Toolkit does not support TaperedProfiles with different section shapes.");
                    return null;
                }
                else
                {
                    midasSectionProperty.Add(sectionProperty.AdapterId<string>(typeof(MidasCivilId)) + ",TAPERED," +
                        new string(sectionProperty.DescriptionOrName().Replace(",", "").Take(sectionPropertyCharacterLimit).ToArray()) +
                        ",CC, 0,0,0,0,0,0,0,0,YES,NO," + GetSectionShapeCode(sectionProperty) + "," + GetInterpolationOrder(sectionProperty) +
                        "," + GetInterpolationOrder(sectionProperty) + ",USER");
                    midasSectionProperty.Add(CreateProfile(sectionProperty.SectionProfile as dynamic, lengthUnit));
                }
            }
            else
            {
                midasSectionProperty.Add(sectionProperty.AdapterId<string>(typeof(MidasCivilId)) + ",DBUSER," +
                    new string(sectionProperty.DescriptionOrName().Replace(",", "").Take(sectionPropertyCharacterLimit).ToArray()) +
                    ",CC, 0, 0, 0, 0, 0, 0, YES, NO," + CreateProfile(sectionProperty.SectionProfile as dynamic, lengthUnit));
            }

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateSection(ExplicitSection sectionProperty, string lengthUnit, int sectionPropertyCharacterLimit)
        {
            Engine.Reflection.Compute.RecordError("ExplicitSection not supported in MidasCivil_Toolkit");

            return null;
        }

        /***************************************************/

        private static string CreateProfile(RectangleProfile profile, string lengthUnit)
        {
            string midasSectionProperty = "SB, 2," + profile.Height.LengthFromSI(lengthUnit) + "," + profile.Width.LengthFromSI(lengthUnit) + " ,0, 0, 0, 0, 0, 0, 0, 0";

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateProfile(BoxProfile profile, string lengthUnit)
        {
            double webSpacing = (profile.Width - profile.Thickness).LengthFromSI(lengthUnit);
            string midasSectionProperty = "B, 2," +
                profile.Height.LengthFromSI(lengthUnit) + "," + profile.Width.LengthFromSI(lengthUnit) + "," + profile.Thickness.LengthFromSI(lengthUnit) + "," +
                profile.Thickness.LengthFromSI(lengthUnit) + "," + webSpacing + "," + profile.Thickness.LengthFromSI(lengthUnit) +
                ", 0, 0, 0, 0";

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateProfile(FabricatedBoxProfile profile, string lengthUnit)
        {
            double webSpacing = (profile.Width - profile.WebThickness).LengthFromSI(lengthUnit);
            string midasSectionProperty = "B, 2," +
                profile.Height.LengthFromSI(lengthUnit) + "," + profile.Width.LengthFromSI(lengthUnit) + "," + profile.WebThickness.LengthFromSI(lengthUnit) + "," +
                profile.TopFlangeThickness.LengthFromSI(lengthUnit) + "," + webSpacing + "," + profile.BotFlangeThickness.LengthFromSI(lengthUnit) +
                ", 0, 0, 0, 0";

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateProfile(CircleProfile profile, string lengthUnit)
        {
            string midasSectionProperty = "SR, 2," +
                profile.Diameter.LengthFromSI(lengthUnit) +
                ", 0, 0, 0, 0, 0, 0, 0, 0, 0";

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateProfile(TubeProfile profile, string lengthUnit)
        {
            string midasSectionProperty = "P  , 2," +
                profile.Diameter.LengthFromSI(lengthUnit) + "," + profile.Thickness.LengthFromSI(lengthUnit) +
                ", 0, 0, 0, 0, 0, 0, 0, 0";

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateProfile(ISectionProfile profile, string lengthUnit)
        {
            string midasSectionProperty = "H, 2, " + profile.Height.LengthFromSI(lengthUnit) + "," + profile.Width.LengthFromSI(lengthUnit) + "," +
                profile.WebThickness.LengthFromSI(lengthUnit) + "," + profile.FlangeThickness.LengthFromSI(lengthUnit) + "," + profile.Width.LengthFromSI(lengthUnit) + "," +
                profile.FlangeThickness.LengthFromSI(lengthUnit) + "," + profile.RootRadius.LengthFromSI(lengthUnit) + "," + profile.ToeRadius.LengthFromSI(lengthUnit) +
                ", 0, 0";

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateProfile(TSectionProfile profile, string lengthUnit)
        {
            string midasSectionProperty = "T, 2," + profile.Height.LengthFromSI(lengthUnit) + "," + profile.Width.LengthFromSI(lengthUnit) + "," +
                profile.WebThickness.LengthFromSI(lengthUnit) + "," + profile.FlangeThickness.LengthFromSI(lengthUnit) +
                ",0, 0, 0, 0, 0, 0";

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateProfile(FabricatedISectionProfile profile, string lengthUnit)
        {
            string midasSectionProperty = "H, 2, " + profile.Height.LengthFromSI(lengthUnit) + "," + profile.TopFlangeWidth.LengthFromSI(lengthUnit) + "," +
                profile.WebThickness.LengthFromSI(lengthUnit) + "," + profile.TopFlangeThickness.LengthFromSI(lengthUnit) + "," + profile.BotFlangeWidth.LengthFromSI(lengthUnit) + "," +
                profile.BotFlangeThickness.LengthFromSI(lengthUnit) + "," + profile.WeldSize.LengthFromSI(lengthUnit) +
                ", 0, 0, 0";
            Engine.Reflection.Compute.RecordWarning("The weld size for the FabiricatedISectionProfile been assumed equal to the root radius parameter in MidasCivil");

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateProfile(AngleProfile profile, string lengthUnit)
        {
            string midasSectionProperty = "L, 2," + profile.Height.LengthFromSI(lengthUnit) + "," + profile.Width.LengthFromSI(lengthUnit) + "," +
                profile.WebThickness.LengthFromSI(lengthUnit) + "," + profile.FlangeThickness.LengthFromSI(lengthUnit) +
                ", 0, 0, 0, 0, 0, 0";

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateProfile(ChannelProfile profile, string lengthUnit)
        {
            string midasSectionProperty = "C  , 2," + profile.Height.LengthFromSI(lengthUnit) + "," + profile.FlangeWidth.LengthFromSI(lengthUnit) + "," +
                profile.WebThickness.LengthFromSI(lengthUnit) + "," + profile.FlangeThickness.LengthFromSI(lengthUnit) + "," +
                profile.FlangeWidth.LengthFromSI(lengthUnit) + "," + profile.FlangeThickness.LengthFromSI(lengthUnit) + ", 0, 0, 0, 0";

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateProfile(GeneralisedFabricatedBoxProfile profile, string lengthUnit)
        {
            double webSpacing = 0;
            double flangeWidth = (profile.Width);
            if (profile.TopLeftCorbelWidth != 0 || profile.TopRightCorbelWidth != 0 || profile.BotLeftCorbelWidth != 0 || profile.BotRightCorbelWidth != 0)
            {
                webSpacing = (profile.Width - profile.WebThickness).LengthFromSI(lengthUnit);
                flangeWidth = (flangeWidth + profile.TopLeftCorbelWidth + profile.TopRightCorbelWidth);

                Engine.Reflection.Compute.RecordWarning("MidasCivil does not support unequal corbel widths. Therefore the section width has been calculated using the TopCorbelWidths.");
            }
            string midasSectionProperty = "B, 2," +
                profile.Height.LengthFromSI(lengthUnit) + "," + flangeWidth.LengthFromSI(lengthUnit) + "," + profile.WebThickness.LengthFromSI(lengthUnit) + "," +
                profile.TopFlangeThickness.LengthFromSI(lengthUnit) + "," + webSpacing + "," + profile.BotFlangeThickness.LengthFromSI(lengthUnit) +
                ", 0, 0, 0, 0";

            return midasSectionProperty;
        }

        /***************************************************/

        private static string CreateProfile(ZSectionProfile profile, string lengthUnit)
        {
            Engine.Reflection.Compute.RecordError("ZSectionProfile not supported by the MidasCivil_Toolkit");

            return null;
        }

        /***************************************************/

        private static string CreateProfile(FreeFormProfile profile, string lengthUnit)
        {
            Engine.Reflection.Compute.RecordError("FreeFormProfile not supported by the MidasCivil_Toolkit");

            return null;
        }

        /***************************************************/

        private static string CreateProfile(KiteProfile profile, string lengthUnit)
        {
            Engine.Reflection.Compute.RecordError("KiteProfile not supported by the MidasCivil_Toolkit");

            return null;
        }

        /***************************************************/

        private static string CreateProfile(GeneralisedTSectionProfile profile, string lengthUnit)
        {
            Engine.Reflection.Compute.RecordError("GeneralisedTSectionProfile not supported by the MidasCivil_Toolkit");

            return null;
        }

        /***************************************************/

        private static string CreateProfile(TaperedProfile profile, string lengthUnit)
        {
            List<IProfile> profiles = new List<IProfile>(profile.Profiles.Values);

            if (profiles.Count > 2)
                Engine.Reflection.Compute.RecordWarning("MidasCivil only supports TaperedProfiles with a startProfile and endProfile, any intermediate profiles will be ignored.");

            List<string> startProfile = new List<string>(CreateProfile(profiles[0] as dynamic, lengthUnit).Split(','));
            List<string> endProfile = new List<string>(CreateProfile(profiles[profiles.Count - 1] as dynamic, lengthUnit).Split(','));

            startProfile.GetRange(2, startProfile.Count - 4);

            return string.Join(",", startProfile.GetRange(2, startProfile.Count - 4)) + "," + string.Join(",", endProfile.GetRange(2, startProfile.Count - 4));
        }

        /***************************************************/

        private static string GetInterpolationOrder(ISectionProperty sectionProperty)
        {
            if (sectionProperty is SteelSection)
            {
                SteelSection section = (SteelSection)sectionProperty;
                TaperedProfile taperedProfile = (TaperedProfile)section.SectionProfile;
                return taperedProfile.InterpolationOrder.Max().ToString();

            }
            else if (sectionProperty is ConcreteSection)
            {
                ConcreteSection section = (ConcreteSection)sectionProperty;
                TaperedProfile taperedProfile = (TaperedProfile)section.SectionProfile;
                return taperedProfile.InterpolationOrder.Max().ToString();
            }
            else if (sectionProperty is AluminiumSection)
            {
                AluminiumSection section = (AluminiumSection)sectionProperty;
                TaperedProfile taperedProfile = (TaperedProfile)section.SectionProfile;
                return taperedProfile.InterpolationOrder.Max().ToString();
            }
            else if (sectionProperty is TimberSection)
            {
                TimberSection section = (TimberSection)sectionProperty;
                TaperedProfile taperedProfile = (TaperedProfile)section.SectionProfile;
                return taperedProfile.InterpolationOrder.Max().ToString();
            }
            else if (sectionProperty is GenericSection)
            {
                GenericSection section = (GenericSection)sectionProperty;
                TaperedProfile taperedProfile = (TaperedProfile)section.SectionProfile;
                return taperedProfile.InterpolationOrder.Max().ToString();
            }
            return null;
        }

        /***************************************************/

        private static string GetSectionShapeCode(SteelSection sectionProperty)
        {
            return GetProfileShapeCode(sectionProperty.SectionProfile as dynamic);
        }

        /***************************************************/

        private static string GetSectionShapeCode(ConcreteSection sectionProperty)
        {
            return GetProfileShapeCode(sectionProperty.SectionProfile as dynamic);
        }

        /***************************************************/

        private static string GetSectionShapeCode(TimberSection sectionProperty)
        {
            return GetProfileShapeCode(sectionProperty.SectionProfile as dynamic);
        }

        /***************************************************/

        private static string GetSectionShapeCode(AluminiumSection sectionProperty)
        {
            return GetProfileShapeCode(sectionProperty.SectionProfile as dynamic);
        }

        /***************************************************/

        private static string GetSectionShapeCode(GenericSection sectionProperty)
        {
            return GetProfileShapeCode(sectionProperty.SectionProfile as dynamic);
        }

        /***************************************************/

        private static string GetSectionShapeCode(ExplicitSection sectionProperty)
        {
            return null;
        }

        /***************************************************/

        private static string GetProfileShapeCode(RectangleProfile sectionProfile)
        {
            return "SB";
        }

        /***************************************************/

        private static string GetProfileShapeCode(CircleProfile sectionProfile)
        {
            return "SR";
        }

        /***************************************************/

        private static string GetProfileShapeCode(TubeProfile sectionProfile)
        {
            return "P";
        }

        /***************************************************/

        private static string GetProfileShapeCode(GeneralisedFabricatedBoxProfile sectionProfile)
        {
            return "B";
        }

        /***************************************************/

        private static string GetProfileShapeCode(FabricatedBoxProfile sectionProfile)
        {
            return "B";
        }

        /***************************************************/

        private static string GetProfileShapeCode(BoxProfile sectionProfile)
        {
            return "B";
        }

        /***************************************************/

        private static string GetProfileShapeCode(ISectionProfile sectionProfile)
        {
            return "H";
        }

        /***************************************************/

        private static string GetProfileShapeCode(FabricatedISectionProfile sectionProfile)
        {
            return "H";
        }

        /***************************************************/

        private static string GetProfileShapeCode(TSectionProfile sectionProfile)
        {
            return "T";
        }

        /***************************************************/

        private static string GetProfileShapeCode(AngleProfile sectionProfile)
        {
            return "L";
        }

        /***************************************************/

        private static string GetProfileShapeCode(ChannelProfile sectionProfile)
        {
            return "C";
        }

        /***************************************************/

        private static string GetProfileShapeCode(GeneralisedTSectionProfile sectionProfile)
        {
            return null;
        }

        /***************************************************/

        private static string GetProfileShapeCode(TaperedProfile sectionProfile)
        {
            TaperedProfile taperedProfile = (TaperedProfile)sectionProfile;
            return GetProfileShapeCode(taperedProfile.Profiles.Values.First() as dynamic);
        }

        /***************************************************/

    }
}
