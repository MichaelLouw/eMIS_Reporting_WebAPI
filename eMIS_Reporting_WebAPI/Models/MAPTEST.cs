//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eMIS_Reporting_WebAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class MAPTEST
    {
        [Key]
        public string GISKEY { get; set; }
        public string TEST { get; set; }
        public string Hello { get; set; }
        public Nullable<int> COMPNO { get; set; }
        public Nullable<int> Column_5 { get; set; }
    }
}
