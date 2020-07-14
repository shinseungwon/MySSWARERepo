using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSWARE_CORE_PROTOTYPE.Models.Shared
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
