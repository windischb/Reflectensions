﻿using System;
using System.Collections.Generic;
using System.Text;
using Reflectensions.HelperClasses;

namespace Reflectensions.Tests.Classes {
    public class Expandable1: ExpandableObject {

        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class Expandable2 : Expandable1 {

        public bool Ok { get; set; }

        public DateTime Now { get; set; }

        public List<DateTime> Dates { get; set; }
    }
}
