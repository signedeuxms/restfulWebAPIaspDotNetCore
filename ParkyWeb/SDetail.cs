using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb
{
    public static class SDetail
    {
        public static string APIbaseUrl = "https://localhost:44315/";
        public static string NationalParkAPIpath = APIbaseUrl + "api/v1/nationalparks/";
        public static string TrailAPIpath = APIbaseUrl + "api/v1/trails/";
        public static string AccountAPIpath = APIbaseUrl + "api/v1/Users/";
    }
}
