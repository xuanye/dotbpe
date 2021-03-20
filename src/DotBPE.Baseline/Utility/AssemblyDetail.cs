using DotBPE.Baseline.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace DotBPE.Baseline.Utility
{
    public class AssemblyDetail
    {
        public string AssemblyName { get; private set; }

        public string AssemblyTitle { get; private set; }

        public string AssemblyDescription { get; private set; }

        public string AssemblyProduct { get; private set; }

        public string AssemblyCompany { get; private set; }

        public string AssemblyCopyright { get; private set; }

        public string AssemblyConfiguration { get; private set; }

        public string AssemblyVersion { get; private set; }

        public string AssemblyFileVersion { get; private set; }

        public string AssemblyInformationalVersion { get; private set; }

        private static readonly ConcurrentDictionary<Assembly, AssemblyDetail> _detailCache = new ConcurrentDictionary<Assembly, AssemblyDetail>();

        public static AssemblyDetail Extract(Assembly assembly)
        {
            return _detailCache.GetOrAdd(assembly, a =>
            {
                var assemblyDetail = new AssemblyDetail();
                var assemblyName = a.GetName();

                assemblyDetail.AssemblyName = assemblyName.Name;
                assemblyDetail.AssemblyVersion = assemblyName.Version?.ToString();

                assemblyDetail.AssemblyTitle = a.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
                assemblyDetail.AssemblyDescription = a.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
                assemblyDetail.AssemblyProduct = a.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
                assemblyDetail.AssemblyCompany = a.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
                assemblyDetail.AssemblyCopyright = a.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
                assemblyDetail.AssemblyConfiguration = a.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;
                assemblyDetail.AssemblyFileVersion = a.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
                assemblyDetail.AssemblyInformationalVersion = a.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

                return assemblyDetail;
            });
        }

        public static IEnumerable<AssemblyDetail> ExtractAll(string filter = "Foundatio*")
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.FullName.Like(filter))
                    continue;

                yield return Extract(assembly);
            }
        }
    }
}
