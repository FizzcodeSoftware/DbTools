﻿namespace FizzCode.DbTools.Common
{
    using System.Collections.Generic;

    public class FeatureSupport
    {
        public FeatureSupport(Support support, string description)
        {
            Support = support;
            Description = description;
        }

        public Support Support { get; set; }
        public string Description { get; set; }
    }
}
