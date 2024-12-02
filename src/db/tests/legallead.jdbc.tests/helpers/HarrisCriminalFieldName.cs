using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.jdbc.tests.helpers
{
    internal static class HarrisCriminalFieldName
    {
        public static List<FieldNameDto> Fields
        {
            get
            {
                const char sq = (char)39;
                const char dq = '"';
                if (fields.Count > 0) return fields;
                var js = json.Replace(sq, dq);
                var tmp= JsonConvert.DeserializeObject<List<FieldNameDto>>(js) ?? new();
                fields.AddRange(tmp);
                return fields;
            }
        }

        private static readonly List<FieldNameDto> fields = [];
        private static readonly string NL = Environment.NewLine;
        private static readonly string json = "[" + NL +
        "	{ 'key': 'cdi', 'value': 'court division indicator' }," + NL +
        "	{ 'key': 'cas', 'value': 'case number' }," + NL +
        "	{ 'key': 'fda', 'value': 'case file date' }," + NL +
        "	{ 'key': 'ins', 'value': 'instrument type' }," + NL +
        "	{ 'key': 'cad', 'value': 'setting results' }," + NL +
        "	{ 'key': 'crt', 'value': 'court number' }," + NL +
        "	{ 'key': 'cst', 'value': 'case status' }," + NL +
        "	{ 'key': 'dst', 'value': 'defendant status' }," + NL +
        "	{ 'key': 'bam', 'value': 'bond amount' }," + NL +
        "	{ 'key': 'curr_off', 'value': 'current offense code' }," + NL +
        "	{ 'key': 'curr_off_lit', 'value': 'current offense literal' }," + NL +
        "	{ 'key': 'curr_l_d', 'value': 'current offense level and degree' }," + NL +
        "	{ 'key': 'com_off', 'value': 'complaint offense code' }," + NL +
        "	{ 'key': 'com_off_lit', 'value': 'complaint offense literal' }," + NL +
        "	{ 'key': 'com_l_d', 'value': 'complaint offense level and degree' }," + NL +
        "	{ 'key': 'gj_off', 'value': 'grand jury offense code' }," + NL +
        "	{ 'key': 'gj_off_lit', 'value': 'grand jury offense literal' }," + NL +
        "	{ 'key': 'gj_l_d', 'value': 'grand jury offense level and degree' }," + NL +
        "	{ 'key': 'nda', 'value': 'next appearance date' }," + NL +
        "	{ 'key': 'cnc', 'value': 'docket type' }," + NL +
        "	{ 'key': 'rea', 'value': 'next appearance reason' }," + NL +
        "	{ 'key': 'def_nam', 'value': 'defendant name' }," + NL +
        "	{ 'key': 'def_spn', 'value': 'defendant spn' }," + NL +
        "	{ 'key': 'def_rac', 'value': 'defendant race' }," + NL +
        "	{ 'key': 'def_sex', 'value': 'defendant sex' }," + NL +
        "	{ 'key': 'def_dob', 'value': 'defendant date of birth' }," + NL +
        "	{ 'key': 'def_stnum', 'value': 'defendant street number' }," + NL +
        "	{ 'key': 'def_stnam', 'value': 'defendant street name' }," + NL +
        "	{ 'key': 'def_apt', 'value': 'defendant apartment number' }," + NL +
        "	{ 'key': 'def_cty', 'value': 'defendant city' }," + NL +
        "	{ 'key': 'def_st', 'value': 'defendant state' }," + NL +
        "	{ 'key': 'def_zip', 'value': 'defendant zip' }," + NL +
        "	{ 'key': 'aty_nam', 'value': 'attorney name' }," + NL +
        "	{ 'key': 'aty_spn', 'value': 'attorney spn' }," + NL +
        "	{ 'key': 'aty_coc', 'value': 'attorney connection code' }," + NL +
        "	{ 'key': 'aty_coc_lit', 'value': 'attorney connection literal' }," + NL +
        "	{ 'key': 'comp_nam', 'value': 'complainant name' }," + NL +
        "	{ 'key': 'comp_agency', 'value': 'complainant agency' }," + NL +
        "	{ 'key': 'off_rpt_num', 'value': 'offense report number' }," + NL +
        "	{ 'key': 'dispdt', 'value': 'disposition date' }," + NL +
        "	{ 'key': 'disposition', 'value': 'disposition' }," + NL +
        "	{ 'key': 'sentence', 'value': 'sentence' }," + NL +
        "	{ 'key': 'def_citizen', 'value': 'def citizenship status' }," + NL +
        "	{ 'key': 'bamexp', 'value': 'bond exception' }," + NL +
        "	{ 'key': 'gj_dt', 'value': 'grand jury date' }," + NL +
        "	{ 'key': 'gj_crt', 'value': 'grand jury court' }," + NL +
        "	{ 'key': 'gj_cdp', 'value': 'grand jury action' }," + NL +
        "	{ 'key': 'def_pob', 'value': 'defendant place of birth' }" + NL +
        "]";
    }
}
