using Newtonsoft.Json;

namespace legallead.jdbc.helpers
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
                var tmp = JsonConvert.DeserializeObject<List<FieldNameDto>>(js) ?? new();
                fields.AddRange(tmp);
                return fields;
            }
        }

        private static readonly List<FieldNameDto> fields = [];
        private static readonly string NL = Environment.NewLine;
        private static readonly string json = "[" + NL +
        "\t{ 'key': 'cdi', 'value': 'court division indicator' }," + NL +
        "\t{ 'key': 'cas', 'value': 'case number' }," + NL +
        "\t{ 'key': 'fda', 'value': 'case file date' }," + NL +
        "\t{ 'key': 'ins', 'value': 'instrument type' }," + NL +
        "\t{ 'key': 'cad', 'value': 'setting results' }," + NL +
        "\t{ 'key': 'crt', 'value': 'court number' }," + NL +
        "\t{ 'key': 'cst', 'value': 'case status' }," + NL +
        "\t{ 'key': 'dst', 'value': 'defendant status' }," + NL +
        "\t{ 'key': 'bam', 'value': 'bond amount' }," + NL +
        "\t{ 'key': 'curr_off', 'value': 'current offense code' }," + NL +
        "\t{ 'key': 'curr_off_lit', 'value': 'current offense literal' }," + NL +
        "\t{ 'key': 'curr_l_d', 'value': 'current offense level and degree' }," + NL +
        "\t{ 'key': 'com_off', 'value': 'complaint offense code' }," + NL +
        "\t{ 'key': 'com_off_lit', 'value': 'complaint offense literal' }," + NL +
        "\t{ 'key': 'com_l_d', 'value': 'complaint offense level and degree' }," + NL +
        "\t{ 'key': 'gj_off', 'value': 'grand jury offense code' }," + NL +
        "\t{ 'key': 'gj_off_lit', 'value': 'grand jury offense literal' }," + NL +
        "\t{ 'key': 'gj_l_d', 'value': 'grand jury offense level and degree' }," + NL +
        "\t{ 'key': 'nda', 'value': 'next appearance date' }," + NL +
        "\t{ 'key': 'cnc', 'value': 'docket type' }," + NL +
        "\t{ 'key': 'rea', 'value': 'next appearance reason' }," + NL +
        "\t{ 'key': 'def_nam', 'value': 'defendant name' }," + NL +
        "\t{ 'key': 'def_spn', 'value': 'defendant spn' }," + NL +
        "\t{ 'key': 'def_rac', 'value': 'defendant race' }," + NL +
        "\t{ 'key': 'def_sex', 'value': 'defendant sex' }," + NL +
        "\t{ 'key': 'def_dob', 'value': 'defendant date of birth' }," + NL +
        "\t{ 'key': 'def_stnum', 'value': 'defendant street number' }," + NL +
        "\t{ 'key': 'def_stnam', 'value': 'defendant street name' }," + NL +
        "\t{ 'key': 'def_apt', 'value': 'defendant apartment number' }," + NL +
        "\t{ 'key': 'def_cty', 'value': 'defendant city' }," + NL +
        "\t{ 'key': 'def_st', 'value': 'defendant state' }," + NL +
        "\t{ 'key': 'def_zip', 'value': 'defendant zip' }," + NL +
        "\t{ 'key': 'aty_nam', 'value': 'attorney name' }," + NL +
        "\t{ 'key': 'aty_spn', 'value': 'attorney spn' }," + NL +
        "\t{ 'key': 'aty_coc', 'value': 'attorney connection code' }," + NL +
        "\t{ 'key': 'aty_coc_lit', 'value': 'attorney connection literal' }," + NL +
        "\t{ 'key': 'comp_nam', 'value': 'complainant name' }," + NL +
        "\t{ 'key': 'comp_agency', 'value': 'complainant agency' }," + NL +
        "\t{ 'key': 'off_rpt_num', 'value': 'offense report number' }," + NL +
        "\t{ 'key': 'dispdt', 'value': 'disposition date' }," + NL +
        "\t{ 'key': 'disposition', 'value': 'disposition' }," + NL +
        "\t{ 'key': 'sentence', 'value': 'sentence' }," + NL +
        "\t{ 'key': 'def_citizen', 'value': 'def citizenship status' }," + NL +
        "\t{ 'key': 'bamexp', 'value': 'bond exception' }," + NL +
        "\t{ 'key': 'gj_dt', 'value': 'grand jury date' }," + NL +
        "\t{ 'key': 'gj_crt', 'value': 'grand jury court' }," + NL +
        "\t{ 'key': 'gj_cdp', 'value': 'grand jury action' }," + NL +
        "\t{ 'key': 'def_pob', 'value': 'defendant place of birth' }" + NL +
        "]";
    }
}
