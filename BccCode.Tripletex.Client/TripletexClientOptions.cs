using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace BccCode.Tripletex.Client
{
    public class TripletexClientOptions
    {
        public string? EmployeeToken { get; set; }

        public string? ConsumerToken { get; set; }

        public string ApiBasePath { get; set; } = "https://tripletex.no/v2";

    }
}
