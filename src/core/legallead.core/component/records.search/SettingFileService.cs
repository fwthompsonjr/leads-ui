using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.records.search
{
    internal static class SettingFileService
    {
        public static bool Exists(string? path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            var keys = fileList.Keys.ToList();
            var shortname = Path.GetFileNameWithoutExtension(path);
            var exists = keys.Exists(k => k.Equals(shortname, StringComparison.OrdinalIgnoreCase));
            return exists;
        }
        public static string GetContentOrDefault(string? path, string fallback)
        {
            if (string.IsNullOrEmpty(path)) return fallback;
            var keys = fileList.Keys.ToList();
            var shortname = Path.GetFileNameWithoutExtension(path);
            var exists = keys.Exists(k => k.Equals(shortname, StringComparison.OrdinalIgnoreCase));
            if (!exists) return fallback;
            var keyname = keys.Find(k => k.Equals(shortname, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(keyname)) return fallback;
            exists = fileList.TryGetValue(keyname, out var found);
            if (!exists || string.IsNullOrEmpty(found)) return fallback;
            return found;
        }

        private static readonly Dictionary<string, string> fileList = new()
        {
            { "caselayout", Properties.Resources.xml_caselayout_xml },
            { "collinCountyCaseType", Properties.Resources.xml_collinCountyCaseType_json },
            { "collinCountyMapping_1", Properties.Resources.xml_collinCountyMapping_1_json },
            { "collinCountyUserMap", Properties.Resources.xml_collinCountyUserMap_json },
            { "courtAddress", Properties.Resources.xml_courtAddress_json },
            { "dentonCaseCustomInstruction_1", Properties.Resources.xml_dentonCaseCustomInstruction_1_json },
            { "dentonCaseCustomInstruction", Properties.Resources.xml_dentonCaseCustomInstruction_json },
            { "dentonCountyCaseType", Properties.Resources.xml_dentonCountyCaseType_json },
            { "dentonDistrictCaseType", Properties.Resources.xml_dentonDistrictCaseType_json },
            { "denton-settings", Properties.Resources.xml_denton_settings_json },
            { "harrisCivilCaseType", Properties.Resources.xml_harrisCivilCaseType_json },
            { "harrisCivilMapping", Properties.Resources.xml_harrisCivilMapping_json },
            { "harris-civil-settings", Properties.Resources.xml_harris_civil_settings_json },
            { "settings", Properties.Resources.xml_settings_xml },
            { "tarrantCountyCaseType", Properties.Resources.xml_tarrantCountyCaseType_json },
            { "tarrantCountyCustomType", Properties.Resources.xml_tarrantCountyCustomType_json },
            { "tarrantCountyDataPoint", Properties.Resources.tarrantCountyDataPoint_json },
            { "tarrantCountyMapping_1", Properties.Resources.xml_tarrantCountyMapping_1_json },
            { "tarrantCountyMapping_2", Properties.Resources.xml_tarrantCountyMapping_2_json },
            { "tarrantCourtSearchDropDown", Properties.Resources.xml_tarrantCourtSearchDropDown_json },
            { "webDrivers", Properties.Resources.xml_webDrivers_json },
        };
    }
}
