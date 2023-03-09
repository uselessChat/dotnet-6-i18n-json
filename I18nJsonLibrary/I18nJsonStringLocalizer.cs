using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace I18nJsonLibrary
{
    public class I18nJsonStringLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name]
        {
            get
            {
                var value = FindBy(name);
                var notFound = string.IsNullOrEmpty(value);
                return new LocalizedString(name, value ?? name, notFound);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var localized = this[name];
                return localized.ResourceNotFound
                    ? localized
                    : new LocalizedString(name, string.Format(localized.Value, arguments), false);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var @default = new LocalizedString("NA", "NOT_FOUND", true);
            var stream = EmbeddedResource(Thread.CurrentThread.CurrentCulture.Name);
            if (stream == null) { yield return @default; }

            using (stream)
            {
                using var streamReader = new StreamReader(stream!);
                using var reader = new JsonTextReader(streamReader);
                JObject data = JObject.Load(reader);
                var rootToken = data.Root;
                foreach (var pair in SearchJTokenPathsWithValues(rootToken))
                    yield return new LocalizedString(pair.Key, pair.Value, false);
            }
        }

        public IEnumerable<string> SearchJTokenPaths(JToken jtoken)
        {
            var firstChildren = jtoken.Children().FirstOrDefault();
            if (jtoken.GetType() == typeof(JProperty) &&
                jtoken.Count() == 1 &&
                jtoken.HasValues &&
                firstChildren != null && firstChildren.GetType() == typeof(JValue))
            {
                return new List<string> { jtoken.Path };
            }
            return jtoken.SelectMany(token => SearchJTokenPaths(token));
        }

        public IEnumerable<KeyValuePair<string, string>> SearchJTokenPathsWithValues(JToken jtoken)
        {
            var firstChildren = jtoken.Children().FirstOrDefault();
            var child = firstChildren as JValue;
            if (jtoken.GetType() == typeof(JProperty) &&
                jtoken.Count() == 1 &&
                jtoken.HasValues &&
                child != null && child.GetType() == typeof(JValue))
            {
                string _value = Convert.ToString(child.Value) ?? string.Empty;
                var value = new KeyValuePair<string, string>(jtoken.Path, _value);
                return new List<KeyValuePair<string, string>> { value };
            }
            return jtoken.SelectMany(token => SearchJTokenPathsWithValues(token));
        }

        virtual public string? FindBy(string key)
        {
            string? result = FindResourceValueBy(key);
            return result;
        }

        public string? FindResourceValueBy(string propertyName)
        {
            var stream = EmbeddedResource(Thread.CurrentThread.CurrentCulture.Name);
            if (stream == null) return null;
            using (stream)
            {
                using var streamReader = new StreamReader(stream!);
                using var reader = new JsonTextReader(streamReader);
                JObject data = JObject.Load(reader);
                JToken? token = data.SelectToken(propertyName);
                return token?.Value<string>();
            }
        }

        public Stream? EmbeddedResource(string cultureName)
        {
            var assembly = GetType().Assembly;
            var assemblyName = assembly.GetName().Name;
            // Identify expected embedded resource name
            string resourceFormat = "{0}.Resources.{1}.json";
            var resourcesAvailable = assembly.GetManifestResourceNames();
            // Identify what resource can apply based on culture name
            var resourceTargets = new string?[] { cultureName, cultureName.Split("-").FirstOrDefault() }
                .Where(i => !string.IsNullOrEmpty(i))
                .Select(cultureKey => string.Format(resourceFormat, assemblyName, cultureKey));
            var target = resourceTargets.FirstOrDefault(target => resourcesAvailable.Contains(target));
            return target == null ? null : assembly.GetManifestResourceStream(target);
        }
    }
}
