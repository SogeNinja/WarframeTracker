using System;
using System.Collections.Generic;
using System.Text;
namespace WarframeMasteryTracker.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string Type { get; set; } = "";
        public int? MasteryReq { get; set; }
        public bool IsMastered { get; set; }
    }
}
