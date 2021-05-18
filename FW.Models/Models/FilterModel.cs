using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FreewheelAssessment.Models
{
    public class FilterModel
    {
        public string title { get; set; }
        public int? year { get; set; }
        public string genre { get; set; }

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(title) && year == null && string.IsNullOrWhiteSpace(genre))
                    return false;
                return true;
            }
        }
    }
}
