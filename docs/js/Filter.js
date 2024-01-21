var MYCONT=document.getElementById("DATA");
var isListDev = GetQueryValue("ListDev");
var DataQr = GetQueryValue("SearchIT");
var DataDwQr = GetQueryValue("Download");
function GetQueryValue(name) {
	name = name.replace(/[\[\]]/g, '\\$&');
	results = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)').exec(window.location.href);
	if (!results) return null;
	if (!results[2]) return '';
	return decodeURIComponent(results[2].replace(/\+/g, ' '));
}
function SearchIT(data) {
	filter = data.toUpperCase();
	var ARRA = document.getElementsByClassName("card");
	for (i=0; i < ARRA.length; i++)
	{
		if(ARRA[i].innerHTML.toUpperCase().indexOf(filter) > -1) 
			ARRA[i].style.display = "";
		else
			ARRA[i].style.display = "none";
	}
	console.log("Len: " + ARRA.length);
}
	
function ClientDataOut()
{
	for (i=0; i < Devices.length; i++)
	{
		var part = Devices[i].split("'");
		MYCONT.innerHTML += GetTemplate("UBootloaderTAG", "img/DefULBL.png", part[0], part[1]);
	}
}
function BrowserLinksOut()
{
	for (i=0; i < Tutorials.length; i++)
	{
		var part = Tutorials[i].split("'");
		MYCONT.innerHTML += GetTemplate("", "img/DefULBL.png", part[0], part[1]).replace(">Download<", ">Read<");
	}
	for (i=0; i < TutorialsV.length; i++)
	{
		var part = TutorialsV[i].split("'");
		MYCONT.innerHTML += GetTemplate("", "img/YTIMG.png", part[0], part[1]).replace(">Download<", ">Watch<");
	}
}	
function BrowserDownloadableDataOut()
{
	for (i=0; i < Devices.length; i++)
	{
		var part = Devices[i].split("'");
		MYCONT.innerHTML += GetTemplate("UBootloaderTAG", "img/DefULBL.png", part[0], part[1]);
	}
	for (i=0; i < Tools.length; i++)
	{
		var part = Tools[i].split("'")
		MYCONT.innerHTML += GetTemplate("UToolTAG", "img/DefTool.png", part[0], part[1]);
	}
	for (i=0; i < Drivers.length; i++)
	{
		var part = Drivers[i].split("'");
		MYCONT.innerHTML += GetTemplate("UDriverTAG", "img/DefDriver.png", part[0], part[1]);
	}
	for (i=0; i < Loaders.length; i++)
	{
		var part = Loaders[i].split("'");
		MYCONT.innerHTML += GetTemplate("ULoaderTAG", "img/DefLoader.png", part[0], part[1]);
	}
	for (i=0; i < Firmwares.length; i++)
	{
		var part = Firmwares[i].split("'");
		MYCONT.innerHTML += GetTemplate("UFirmwareTAG", "img/DefFirm.png", part[0], part[1]);
	}
	for (i=0; i < MISC.length; i++)
	{
		var part = MISC[i].split("'");
		MYCONT.innerHTML += GetTemplate("UMiscTAG", "img/DefULFile.png", part[0], part[1]);
	}
	for (i=0; i < GPTS.length; i++)
	{
		var part = GPTS[i].split("'");
		MYCONT.innerHTML += GetTemplate("UGptTAG", "img/DefGPT.png", part[0], part[1]);
	}
	for (i=0; i < MTKDAS.length; i++)
	{
		var part = MTKDAS[i].split("'");
		MYCONT.innerHTML += GetTemplate("UMtkTAG", "img/DefFWMTK.png", part[0], part[1]);
	}
	if(DataQr != null & DataQr.length > 0) {
		SearchIT(DataQr);
		if(DataDwQr != null) {
			var get = document.getElementById(DataQr);
			location.replace(get.href);
		}
	}
}
function GetTemplate(tag, urlimg, name, url)
{
	var data = 
	`
	<div class=\"card\" style=\"width: 18rem;\">
		<div class=\"card-body\">
			<img class="card-img" src={3}></img>
			<br>
			<h5 class=\"card-title\">{1}</h5>
			<a id='{4}' tag='{0}' href='{2}' class='btn2'>Download</div></a>
		</div>
	</div>
	`
	return data.replace("{0}", tag).replace("{1}", name).replace("{2}", url).replace("{3}", urlimg).replace("{4}", name);
}