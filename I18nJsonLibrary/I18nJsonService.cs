using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I18nJsonLibrary
{
    public class I18nJsonService
    {
        public IEnumerable<CultureInfo> AvailableCultures()
        {
            var assembly = GetType().Assembly;
            var result = assembly.GetManifestResourceNames()
                .Select(resource => Path.GetFileNameWithoutExtension(resource))
                .Select(resource => resource.Split(".").LastOrDefault())
                .Where(cultureName => cultureName != null)
                .Select(cultureName => new CultureInfo(cultureName!));
            return result;
        }

        public bool ResolveCurrentCulture(string? culture)
        {
            if (string.IsNullOrEmpty(culture)) return false;

            if (!string.IsNullOrEmpty(culture) && CultureExists(culture))
            {
                var cultureInfo = new CultureInfo(culture);
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                return true;
            }

            return false;
        }

        private bool CultureExists(string culture)
        {
            // return CultureInfo.GetCultures(CultureTypes.AllCultures)
            return AvailableCultures() // Explicit cultures based on embedded resources
                .Any(cultureInfo => string.Equals(cultureInfo.Name, culture, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
