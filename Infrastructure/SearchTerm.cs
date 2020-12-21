﻿namespace DemoApi.Models
{
    /// <summary>
    /// Represents a deconstructed search term.
    /// </summary>
    public class SearchTerm
    {
        public string Name { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public bool ValidSyntax { get; set; }
    }
}