using System;
namespace LocalizationCore3
{
    public class JsonLocalizationOptions
    {
        public string ResourcesPath { get; set; }
        public ResourcesType ResourcesType { get; set; } = ResourcesType.TypeBased;
    }
}
