// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public class ServiceModel
    {
        public int ServiceId { get; set; }

        public string Group { get; set; }

        public Type ServiceType { get; set; }
    }
}
