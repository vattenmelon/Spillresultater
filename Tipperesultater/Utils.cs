using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tipperesultater
{
    class Utils
    {
        public static bool isEnglish()
        {
            return !CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("nb");
        }
    }
}
