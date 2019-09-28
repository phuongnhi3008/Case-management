using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NienLuan2.Helper
{
    public class UUID
    {
        public static string GetUUID(int length)
        {

            Guid guid = Guid.NewGuid();

            return guid.ToString().Substring(0 , length);
        }
    }
}