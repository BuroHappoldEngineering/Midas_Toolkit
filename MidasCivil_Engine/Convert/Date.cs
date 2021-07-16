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
using System.ComponentModel;
using System.Globalization;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Adapters.MidasCivil
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        [Description("Converts a string into a DateTime object.")]
        [Input("date", "Date in the format yyyy-mm-dd.")]
        [Output("date", "A DateTime.")]
        public static DateTime Date(this string date)
        {
            string dateFormatted = date.Replace('\\', '-').Replace('/', '-').Replace('.', '-').Replace(',', '-');
            DateTime resultOut;
            if (DateTime.TryParseExact(dateFormatted, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out resultOut))
            {
                return resultOut;
            }
            else if (DateTime.TryParseExact(dateFormatted, "yy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out resultOut))
            {
                return resultOut;
            }
            else if (DateTime.TryParseExact(dateFormatted, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out resultOut))
            {
                return resultOut;
            }
            else if (DateTime.TryParseExact(dateFormatted, "yy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out resultOut))
            {
                return resultOut;
            }
            else if (DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out resultOut))
            {
                return resultOut;
            }
            else
            {
                Engine.Reflection.Compute.RecordError("Date format not recognised please use yyyy-MM-dd format.");
                return DateTime.MinValue;
            }
        }
    }
}

