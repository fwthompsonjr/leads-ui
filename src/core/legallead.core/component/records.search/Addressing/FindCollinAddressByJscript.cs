using legallead.records.search.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace legallead.records.search.Addressing
{
    internal class FindCollinAddressByJscript : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            var js = CommandJs;
            var jse = (IJavaScriptExecutor)driver;
            var json = Convert.ToString(jse.ExecuteScript(js));
            if (string.IsNullOrEmpty(json)) { CanFind = false; return; }
            var item = TryParse<CollinAddressItem>(json);
            if (item == null) { CanFind = false; return; }
            CanFind = CollinAddressList.Append(json);
            if (!CanFind) return;
            item.Parse();
            linkData.Address = item.Address;
            linkData.Defendant = item.Defendant;

        }

        private static string CommandJs => command ??= GetCommand();
        private static string? command;
        private static string GetCommand()
        {
            var js = string.Join(Environment.NewLine, script).Replace("~", "\\");
            return js;
        }
        private static readonly List<string> script = new()
        {
            "function getDefendantName(targetRw) ",
            "{ ",
            "	try  ",
            "	{ ",
            "		return targetRw.getElementsByTagName('th')[1].innerText; ",
            "	} catch { ",
            "		return ''; ",
            "	} ",
            "} ",
            " ",
            "function getAddressDetails() ",
            "{ ",
            "	let rsp = {  ",
            "	'caseno': '', ",
            "	'address': '' ",
            "	} ",
            "	try { ",
            "		const pipe = '|'; ",
            "		let matches = [  ",
            "			'plaintiff',  ",
            "			'defendant',  ",
            "			'principal',  ",
            "			'petitioner',  ",
            "			'applicant',  ",
            "			'claimant',  ",
            "			'decedent',  ",
            "			'respondent',  ",
            "			'condemnee',  ",
            "			'guardian'  ",
            "			] ",
            "		dvs = document.getElementsByTagName('div') ",
            "		arr = Array.prototype.slice.call( dvs, 0 ); ",
            "		dv = arr.find(x => x.innerText.indexOf('Party Information') >= 0) ",
            "		dvcase = arr.find(x => x.classList.contains('ssCaseDetailCaseNbr')) ",
            "		dvcasenbr = dvcase.innerText.trim().split('.')[1].trim() ",
            "		rsp.caseno = dvcasenbr; ",
            "		trows = dv.closest('table').getElementsByTagName('tbody')[0].getElementsByTagName('tr'); ",
            "		arows = Array.prototype.slice.call( trows, 0 ); ",
            "		targetRw = arows.find(x =>  ",
            "		{ ",
            "			a = x.getElementsByTagName('th'); ",
            "			if ( a.length == 0 ) { return false; } ",
            "			aa = Array.prototype.slice.call(a,0); ",
            "			th = aa.find(h => { ",
            "				if (!h.classList.contains('ssTableHeader')) { return false; }  ",
            "				if (h.innerText.length == 0 ) { return false; }  ",
            "				return matches.includes(h.innerText.trim().toLowerCase()) ",
            "				}); ",
            "			return th != null; ",
            "		}); ",
            "		if (targetRw == null) { return JSON.stringify(rsp); } ",
            "       rsp.defendant = getDefendantName(targetRw);",
            "		linenbr = targetRw.rowIndex + 1; ",
            "		address = trows[linenbr].innerText ",
            "			.replace('~n', pipe) ",
            "			.replace('~n', pipe) ",
            "			.replace('~n', pipe) ",
            "			.replace('~n', pipe) ",
            "			.replace('~n', pipe).trim(); ",
            "		addresses = address.split(pipe).filter(x => x.length > 0 && x.indexOf('DL:') < 0 && x.indexOf('SID:') < 0); ",
            "		for(i = 0; i < addresses.length; i++) { addresses[i] = addresses[i].trim() } ",
            "		rsp['address'] = addresses.join(pipe); ",
            "		return JSON.stringify(rsp); ",
            "	} catch { return JSON.stringify(rsp); } ",
            "} ",
            "dta = getAddressDetails(); ",
            "return dta; "
        };
        private static T? TryParse<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
