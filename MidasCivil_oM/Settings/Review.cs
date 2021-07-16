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

using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.Adapters.MidasCivil
{
    public class Review : BHoMObject
    {
        [Description("The person who has reviewed the model.")]
        public virtual string Reviewer { get; set; }

        [Description("The date when the model was reviewed by the reviewer.In the format yyyy-MM-dd, or DateTime object.")]
        public virtual DateTime ReviewDate { get; set; }

        [Description("A list of comments made by the reviewer.")]
        public virtual List<string> Comments { get; set; } = new List<string>();

        [Description("True if the model is approved for its intended use.")]
        public virtual bool Approved { get; set; }

    }
}
