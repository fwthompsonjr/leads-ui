const emptyGuid = '00000000-0000-0000-0000-000000000000';
const defaultApplicationName = 'legallead.permissions.api';

let settingNames {
	"app_url": "app_url",
	"use_local":  "UseLocal",
	"use_local_instance": "UseLocalInstance",
	"ll_test": "LL-TEST",
	"local_url": "LocalUrl",
	"test_url": "TestUrl",
	"base_url": "BaseUrl"
}
let utilities =
{
	"getLocalUrl": function() {
		let environmentName = pm.environment.name;
		if (environmentName == settingNames.ll_test) { return pm.collectionVariables.get(settingNames.test_url); }
		return pm.collectionVariables.get(settingNames.local_url);
	},
	"getBaseUrl": function(islocal) {
		if (islocal) { return utilities.getLocalUrl(); }
		pm.collectionVariables.get(settingNames.base_url);
	},
	"getEnviromentSetting": function( name, fallback ) {
		try {
			var envalue = pm.environment.get(name);
			if (undefined === envalue || null === envalue || String(envalue).length === 0) { 
				console.log("Environment: " + name +  " := ");
				return fallback; 
			}
			console.log("Environment: " + name +  " := " + envalue);
			return String(envalue);
		} catch {
			return fallback;
		}
	},
	"getAppId": function(hostname, appname){
		try {            
			var target = "".concat(hostname, "/api/application/apps");
			pm.sendRequest({
			url: target,
			method: 'GET',
			header: { 
				"Content-Type": "application/json"
				}
			},
			function (err, response) {  
				var jstring = response.json();
				var jsfirst = jstring[0];
				var indx = jsfirst["id"];
				var name = jsfirst["name"];
				var payload = JSON.stringify({ "id": indx, "name": name });
				pm.environment.set("app_identity", payload);
				});
		} catch {
			var dfpayload = JSON.stringify({ "id": emptyGuid, "name": appname });
			pm.environment.set("app_identity", dfpayload);
		}
	},
	"getApplicationCredential": function (index)
	{
		let variableset = pm.collectionVariables;
		let names = [
			{ "uid": variableset.get('LeadAccount00'), "pwd": variableset.get('LeadPassword00') },
			{ "uid": variableset.get('LeadAccount01'), "pwd": variableset.get('LeadPassword01') },
			{ "uid": variableset.get('UserAccountName'), "pwd": variableset.get('UserPassword') },
		]
		if (null != index && index == '0') { return JSON.stringify(names[0]) }
		if (null != index && index == '1') { return JSON.stringify(names[1]) }
		return JSON.stringify(names[2]);
	}
	
}


var fallbackEnvironment = pm.collectionVariables.get(settingNames.use_local);
var useLocal = utilities.getEnviromentSetting( settingNames.use_local_instance, fallbackEnvironment);
var isLocal = useLocal == 'True';
var basurl = isLocal ? utilities.getLocalUrl() : pm.collectionVariables.get(settingNames.base_url);
pm.environment.set(settingNames.app_url, basurl);
console.log(pm.environment.name);

var locationNames = pm.execution.location == null ? 'unknown' : pm.execution.location;
locationNames.push(pm.environment.name);

var folderName = locationNames[1];
var methodName = locationNames[2];
var isAppRequest = folderName == 'app';

var systemUnderTest = locationNames.join(' | ');
console.log(systemUnderTest);

function doLogin(hostname) {
    try {
        var target = "".concat(hostname, "/api/signon/login");
        var uid = pm.collectionVariables.get('DefaultUserName');
        var pwd = pm.collectionVariables.get('DefaultPassword');
        var appid = pm.environment.get("app_identity");
        var payload = JSON.stringify({ "UserName": uid, "Password": pwd });
            
        pm.sendRequest({
        url: target,
        method: 'POST',
        header: { 
            "Content-Type": "application/json",
            "APP_IDENTITY": appid},
        body: { 
            mode: 'raw',
            raw: payload
            }
        },
        function (err, response) {  
            var jstring = response.json();
            var atoken = jstring["accessToken"];
            var rtoken = jstring["refreshToken"];
            pm.environment.set("access_token", atoken);
            pm.environment.set("refresh_token", rtoken);
            });
    } catch (error) {
        console.debug(error);
    }
}
function doApplicationLogin(index) {
    try {
        var payload = utilities.getApplicationCredential(index);
            
        pm.sendRequest({
        url: target,
        method: 'POST',
        header: {
            "Content-Type": "application/json"
        },
        body: { 
            mode: 'raw',
            raw: payload
            }
        },
        function (err, response) {  
            var jstring = response.json();
            var atoken = jstring["token"];
            pm.environment.set("user_access_token", atoken);
            });
    } catch (error) {
        console.debug(error);
    }
}

function isAppAccountRequest(names)
{
    if (undefined === names || null == names || !names) return false;
    var keywords = [ 'Change', 'Set', 'Fetch', 'Append' ]
    var isKeyFound = false;
    names.forEach(n =>{
        for(let k = 0; k < keywords.length; k++) {
            if (isKeyFound) continue;
            isKeyFound = n.indexOf(keywords[k]) >= 0;
        }
    });
    return isKeyFound;
}

function execPreRequest() {
    try {
        console.log("folder:= " + folderName);
        console.log("method:= " + methodName);
        if (folderName == "search" && methodName.indexOf("RemoteSearch") >= 0) { return; }
        if (folderName == "application") { return; }
        if (folderName == "home") { return; }
		if (folderName == "signon" && methodName.indexOf("Login") >= 0) { return; }
        if (folderName == "app" || folderName.indexOf('onboarding') >= 0) { 
            var isAccountRequest = isAppAccountRequest(locationNames);
            var isPrimaryRequest = locationNames.find(x => x.indexOf('00') >= 0) != null;
            var isSecondayRequest = locationNames.find(x => x.indexOf('01') >= 0) != null;
            if (isAccountRequest){
                if (isPrimaryRequest)
                {
                    doApplicationLogin('0');
                    return; 
                }
                if (isSecondayRequest)
                {
                    doApplicationLogin('1');
                    return; 
                }
                doApplicationLogin();
            }
			return; 
		}
        getAppId();
        
        
        doLogin();
    } catch {

    }
}

execPreRequest();