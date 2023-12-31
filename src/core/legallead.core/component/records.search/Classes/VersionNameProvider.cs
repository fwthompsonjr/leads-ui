﻿using System.Diagnostics;

namespace legallead.records.search.Classes
{
    public class VersionNameProvider
    {
        public static List<string> VersionNames
        {
            get
            {
                return _versionNames ??= GetNames();
            }
        }

        public static string FileVersion
        {
            get
            {
                return _fileVersion ??= GetFileVersion();
            }
        }

        public VersionNameProvider()
        {
            // when the assembly-file-version contains pre-release
            bool isPreRelease = FileVersion.EndsWith($"~{VersionNames[^1]}",
                System.StringComparison.CurrentCultureIgnoreCase);
            Name = isPreRelease ?
                CommonKeyIndexes.FutureKeyWord :
                CommonKeyIndexes.DefaultKeyWord;
        }

        public string Name { get; private set; }

        private static string GetFileVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion ?? string.Empty;
        }

        private static List<string> GetNames()
        {
            return new List<string> {
                CommonKeyIndexes.DefaultKeyWord,
                CommonKeyIndexes.FutureKeyWord };
        }

        private static string? _fileVersion;
        private static List<string>? _versionNames;
    }
}