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
using BH.oM.Adapters.MidasCivil;
using MCEngine = BH.Engine.Adapters.MidasCivil;

namespace BH.Adapter.MidasCivil
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IEnumerable<Metadata> ToMetadata(List<string> data)
        {
            Metadata metadata = new Metadata();
            List<Review> reviews = new List<Review>();
            List<string> comments = new List<string>();
            Review review1 = new Review();
            Review review2 = new Review();
            Review review3 = new Review();
            Review review4 = new Review();

            foreach (string dataItem in data)
            {

                if (dataItem.Contains("PROJECT=")) { metadata.ProjectNumber = dataItem.Split('=')[1]; }
                else if (dataItem.Contains("REVISION=")) { metadata.Revision = dataItem.Split('=')[1]; }
                else if (dataItem.Contains("USER=")) { metadata.Author = dataItem.Split('=')[1]; }
                else if (dataItem.Contains("EMAIL=")) { metadata.Email = dataItem.Split('=')[1]; }
                else if (dataItem.Contains("ADDRESS=")) { metadata.Location = dataItem.Split('=')[1]; }
                else if (dataItem.Contains("CLIENT=")) { metadata.Client = dataItem.Split('=')[1]; }
                else if (dataItem.Contains("TITLE=")) { metadata.ProjectName = dataItem.Split('=')[1]; }
                else if (dataItem.Contains("EDATE="))
                {
                    metadata.CreationDate = MCEngine.Convert.Date(dataItem.Split('=')[1]);
                }
                else if (dataItem.Contains(";DESIGNSTAGE=")) { metadata.DesignStage = dataItem.Split('=')[1]; }
                else if (dataItem.Contains(";PROJECTLEAD=")) { metadata.ProjectLead = dataItem.Split('=')[1]; }
                else if (dataItem.Contains(";DESCRIPTION=")) { metadata.Description = dataItem.Split('=')[1]; }
                else if (dataItem.Contains(";DISCIPLINE=")) { metadata.Discipline = dataItem.Split('=')[1]; }

                if (dataItem.Contains("CHECK1="))
                {
                    review1.Reviewer = (dataItem.Split('=')[1]);
                    reviews.Add(review1);
                }
                else if (dataItem.Contains("CHECK2="))
                {
                    review2.Reviewer = (dataItem.Split('=')[1]);
                    reviews.Add(review2);
                }
                else if (dataItem.Contains("CHECK3="))
                {
                    review3.Reviewer = (dataItem.Split('=')[1]);
                    reviews.Add(review3);
                }
                else if (dataItem.Contains("APPROVE="))
                {
                    review4.Reviewer = (dataItem.Split('=')[1]);
                    reviews.Add(review4);
                    review4.Approved = true;
                }
                if (dataItem.Contains("CDATE1=")) { review1.ReviewDate = MCEngine.Convert.Date(dataItem.Split('=')[1]); }
                else if (dataItem.Contains("CDATE2=")) { review2.ReviewDate = MCEngine.Convert.Date(dataItem.Split('=')[1]); }
                else if (dataItem.Contains("CDATE3=")) { review3.ReviewDate = MCEngine.Convert.Date(dataItem.Split('=')[1]); }
                else if (dataItem.Contains("ADATE=")) { review4.ReviewDate = MCEngine.Convert.Date(dataItem.Split('=')[1]); }
                else if (dataItem.Contains("COMMENT="))
                {
                    if (!dataItem.Contains("COMMENT=This Model Was Created Using BHoM Version:"))
                    {
                        comments.Add(dataItem.Split('=')[1]);
                        if (!string.IsNullOrEmpty(review4.Reviewer))
                        {
                            review4.Comments = comments;
                        }
                        else if (!string.IsNullOrEmpty(review3.Reviewer))
                        {
                            review3.Comments = comments;
                        }
                        else if (!string.IsNullOrEmpty(review2.Reviewer))
                        {
                            review2.Comments = comments;
                        }
                        else if (!string.IsNullOrEmpty(review1.Reviewer))
                        {
                            review1.Comments = comments;
                        }
                    }
                }
            }
            reviews.Reverse();
            metadata.Reviews = reviews;

            List<Metadata> returnMetadata = new List<Metadata>(1);
            returnMetadata.Add(metadata);

            return returnMetadata;
        }

        /***************************************************/
    }
}
