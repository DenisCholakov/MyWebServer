using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShop.Data
{
    public static class DataConstants
    {
        public const int IdMaxLength = 40;
        public const int DefaultMaxLength = 20;
        public const int PlateNumberMaxLength = 8;
        public const int UsernameMinLength = 4;
        public const int MinPasswordLength = 5;
        public const int CarModelMinLength = 5;
        public const int CarYearMinValue = 1900;
        public const int CarYearMaxValue = 2100;

        public const string Mechanic = "Mechanic";
        public const string Client = "Client";
    }
}
