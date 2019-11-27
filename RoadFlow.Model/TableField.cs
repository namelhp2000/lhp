using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    public class TableField
    {
        // Properties
        public string Comment { get; set; }

        public string DefaultValue { get; set; }

        public string FieldName { get; set; }

        public bool IsDefault { get; set; }

        public bool IsIdentity { get; set; }

        public bool IsNull { get; set; }

        public int Size { get; set; }

        public string Type { get; set; }
    }



}
