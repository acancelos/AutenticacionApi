using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacionApiSinIdentity.Modelos
{
    public class ODataQuery
    {
        [FromQuery(Name = "$expand")]
        public string Expand { get; set; }

        [FromQuery(Name = "$filter")]
        public string Filter { get; set; }

        [FromQuery(Name = "$count")]
        public bool? Count { get; set; }

        [FromQuery(Name = "$skip")]
        public int? Skip { get; set; }

        [FromQuery(Name = "$top")]
        public int? Top { get; set; }
    }
}
