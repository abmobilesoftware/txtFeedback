using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    [Serializable]
    public class Voucher
    {
        public String code;
        public String description;

        public Voucher(String iCode, String iDescription)
        {
            code = iCode;
            description = iDescription;
        }
    }
}