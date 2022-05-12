using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LocalizationCore3
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly ConcurrentDictionary<string, IEnumerable<KeyValuePair<string, string>>> _resourcesCache = new ConcurrentDictionary<string, IEnumerable<KeyValuePair<string, string>>>();
        private readonly string _resourcesPath;
        private readonly string _resourceName;
        private readonly ILogger _logger;

        private string _searchedLocation;

        public JsonStringLocalizer(
            string resourcesPath,
            string resourceName, // TODO: Use optional parameter in upcoming major release
            ILogger logger)
        {
            _resourcesPath = resourcesPath ?? throw new ArgumentNullException(nameof(resourcesPath));
            _resourceName = resourceName;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public LocalizedString this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name)); 

                var value = GetStringSafely(name);

                return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _searchedLocation);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));

                var format = GetStringSafely(name);
                var value = string.Format(format ?? name, arguments);

                return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: _searchedLocation);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            GetAllStrings(includeParentCultures, CultureInfo.CurrentCulture);

        public IStringLocalizer WithCulture(CultureInfo culture) => this;

        protected IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            var resourceNames = includeParentCultures
                ? GetAllStringsFromCultureHierarchy(culture)
                : GetAllResourceStrings(culture);

            foreach (var name in resourceNames)
            {
                var value = GetStringSafely(name);
                yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _searchedLocation);
            }
        }

        protected virtual string GetStringSafely(string name)
        {
            if (name == null)  throw new ArgumentNullException(nameof(name)); 

            var culture = CultureInfo.CurrentCulture;
            string value = null;

            while (culture != culture.Parent)
            {
                BuildResourcesCache(culture.ToString());

                if (_resourcesCache.TryGetValue(culture.Name, out IEnumerable<KeyValuePair<string, string>> resources))
                {
                    var resource = resources?.SingleOrDefault(s => s.Key == name);

                    value = resource?.Value ?? null;
                    // _logger.SearchedLocation(name, _searchedLocation, culture);

                    if (value != null)
                    {
                        break;
                    }

                    culture = culture.Parent;
                }
            }

            return value;
        }

        private IEnumerable<string> GetAllStringsFromCultureHierarchy(CultureInfo startingCulture)
        {
            var currentCulture = startingCulture;
            var resourceNames = new HashSet<string>();

            while (currentCulture != currentCulture.Parent)
            {
                var cultureResourceNames = GetAllResourceStrings(currentCulture);

                if (cultureResourceNames != null)
                {
                    foreach (var resourceName in cultureResourceNames)
                    {
                        resourceNames.Add(resourceName);
                    }
                }

                currentCulture = currentCulture.Parent;
            }

            return resourceNames;
        }

        private IEnumerable<string> GetAllResourceStrings(CultureInfo culture)
        {
            BuildResourcesCache(culture.ToString());

            if (_resourcesCache.TryGetValue(culture.Name, out IEnumerable<KeyValuePair<string, string>> resources))
            {
                foreach (var resource in resources)
                {
                    yield return resource.Key;
                }
            }
            else
            {
                yield return null;
            }
        }

        private void BuildResourcesCache(string culture)
        {  
             
            _resourcesCache.GetOrAdd(culture, _ =>
            {
                // var resourceFile = string.IsNullOrEmpty(_resourceName) ? $"{culture}.json" : $"{_resourceName}.{culture}.json";
                var resourceFile = $"{culture}.json";
                 
                _searchedLocation = Path.Combine(_resourcesPath, resourceFile);

                if (!File.Exists(_searchedLocation))
                {
                    if (resourceFile.Count(r => r == '.') > 1)
                    {
                        var resourceFileWithoutExtension = Path.GetFileNameWithoutExtension(resourceFile);
                        var resourceFileWithoutCulture = resourceFileWithoutExtension.Substring(0, resourceFileWithoutExtension.LastIndexOf('.'));
                        resourceFile = $"{resourceFileWithoutCulture.Replace('.', Path.DirectorySeparatorChar)}.{culture}.json";
                        _searchedLocation = Path.Combine(_resourcesPath, resourceFile);
                    }
                }

                IEnumerable<KeyValuePair<string, string>> value = null;

                if (File.Exists(_searchedLocation))
                {
                    /*
                    var builder = new ConfigurationBuilder()
                                    .SetBasePath(_resourcesPath)
                                    .AddJsonFile(resourceFile, optional: false, reloadOnChange: true);

                    var config = builder.Build();
                    value = config.AsEnumerable();
                    */
                    var resourceFileStream = new FileStream(_searchedLocation, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                    using (resourceFileStream)
                    {
                        var resourceReader = new JsonTextReader(new StreamReader(resourceFileStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true));
                        using (resourceReader)
                        {
                            var jsonData = JObject.Load(resourceReader);
                            value = jsonData.ToObject<Dictionary<string, string>>();
                        }
                    }
                }

                return value;
            });
        }
    }
}
