﻿
using legallead.permissions.api.Model;

namespace legallead.models
{
    public class StateSearchData : UsState
    {
        public List<CountySearchData>? Counties { get; set; }
    }
}