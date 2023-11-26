﻿using System.Globalization;

namespace legallead.resources.Models
{
    public class Resource
    {
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int Id { get; set; }

        public int KeyIndex { get; set; }

        public override string ToString()
        {
            var nbrFormat = CultureInfo.CurrentCulture.NumberFormat;
            var type = Type ?? string.Empty;
            var name = Name ?? string.Empty;
            var itemValue = Value ?? string.Empty;
            return $"{Id.ToString("0", nbrFormat)}, {KeyIndex.ToString(nbrFormat)} - {type} - {name} - {itemValue}";
        }
    }
}