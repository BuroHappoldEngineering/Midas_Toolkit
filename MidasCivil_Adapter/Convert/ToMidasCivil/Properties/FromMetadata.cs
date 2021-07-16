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

using System.Text;
using BH.oM.Adapters.MidasCivil;
using System.Collections.Generic;
using BH.Engine.Base;

namespace BH.Adapter.Adapters.MidasCivil
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static StringBuilder FromMetadata(Metadata metadata)
        {
            StringBuilder midasMetadata = new StringBuilder();
            List<Review> reviews = new List<Review>(metadata.Reviews);

            midasMetadata.AppendLine("*PROJINFO");
            midasMetadata.AppendLine($"PROJECT={metadata.ProjectNumber}");
            midasMetadata.AppendLine($";DESIGNSTAGE={metadata.DesignStage}");
            midasMetadata.AppendLine($";PROJECTLEAD={metadata.ProjectLead}");
            midasMetadata.AppendLine($"REVISION={metadata.Revision}");
            midasMetadata.AppendLine($"USER={metadata.Author}");
            midasMetadata.AppendLine($"EMAIL={metadata.Email}");
            midasMetadata.AppendLine($"ADDRESS={metadata.Location.Replace(System.Environment.NewLine, ", ")}");
            midasMetadata.AppendLine($"CLIENT={metadata.Client}");
            midasMetadata.AppendLine($"TITLE={metadata.ProjectName}");
            midasMetadata.AppendLine($"ENGINEER={metadata.Author}");
            midasMetadata.AppendLine($"EDATE={metadata.CreationDate.ToString("yyyy-MM-dd")}");
            midasMetadata.AppendLine($";DESCRIPTION={metadata.Description.Replace('*', '†').Replace("=", " equals ").Replace($"{System.Environment.NewLine}"," ")}");
            midasMetadata.AppendLine($";DISCIPLINE={metadata.Discipline}");

            if(reviews.Count > 4)
            {
                reviews.RemoveRange(1, reviews.Count - 3);
                Engine.Reflection.Compute.RecordWarning("The number of reviews exceeds the limit of MidasCivil. Only the first, last and second to last reviews will be recorded.");
            }
            if(reviews.Count >= 1)
            {
                midasMetadata.AppendLine($"CHECK1={reviews[0].Reviewer}");
                midasMetadata.AppendLine($"CDATE1={reviews[0].ReviewDate.ToString("yyyy-MM-dd")}");
            }
            if (reviews.Count >= 2)
            {
                midasMetadata.AppendLine($"CHECK2={reviews[1].Reviewer}");
                midasMetadata.AppendLine($"CDATE2={reviews[1].ReviewDate.ToString("yyyy-MM-dd")}");
                Engine.Reflection.Compute.RecordWarning("Only comments from the last review will be pushed to MidasCivil.");
            }
            if (reviews.Count >= 3)
            {
                midasMetadata.AppendLine($"CHECK3={reviews[2].Reviewer}");
                midasMetadata.AppendLine($"CDATE3={reviews[2].ReviewDate.ToString("yyyy-MM-dd")}");
            }
            if(reviews[reviews.Count - 1].Approved)
            {
                midasMetadata.AppendLine($"APPROVE={reviews[reviews.Count - 1].Reviewer}");
                midasMetadata.AppendLine($"ADATE={reviews[reviews.Count - 1].ReviewDate.ToString("yyyy-MM-dd")}");
            }

            foreach(string comment in reviews[reviews.Count - 1].Comments)
            {
                midasMetadata.AppendLine($"COMMENT={comment.Replace('*', '†').Replace("=", " equals ").Replace($"{System.Environment.NewLine}", $"{System.Environment.NewLine}COMMENT=")}");
            }

            return midasMetadata;
        }

        /***************************************************/

    }
}
