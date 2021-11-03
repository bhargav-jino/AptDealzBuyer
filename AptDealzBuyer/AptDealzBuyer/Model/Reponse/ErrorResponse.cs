using System;
using System.Collections.Generic;
using System.Text;

namespace AptDealzBuyer.Model.Reponse
{
    public class Errors
    {
        public List<string> orderId { get; set; }
    }

    public class ErrorResponse
    {
        public Errors errors { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int status { get; set; }
        public string traceId { get; set; }
    }

}
