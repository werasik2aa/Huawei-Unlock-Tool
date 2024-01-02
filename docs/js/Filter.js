var MYCONT=document.getElementById("DATA");
function GetQueryValue(name) {
	name = name.replace(/[\[\]]/g, '\\$&');
	results = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)').exec(window.location.href);
	if (!results || !results[2]) return '';
	return decodeURIComponent(results[2].replace(/\+/g, ' '));
}
function SearchIt(data) {
	filter = data.toUpperCase();
	var ARRA = document.getElementsByClassName('ItemCont');
	for (i=0; i < ARRA.length; i++)
	{
		if(ARRA[i].innerHTML.toUpperCase().indexOf(filter) > -1) 
			ARRA[i].style.display = "";
		else
			ARRA[i].style.display = "none";
	}
}
window.onload = function()
{
	var isListDev = GetQueryValue("ListDev");
	if(isListDev == 'true')
		ClientDataOut();
	else
		BrowserDataOut();
};
	
function ClientDataOut()
{
	document.documentElement.innerHTML = '';
	for (i=0; i < Devices.length; i++)
	{
		var part = Devices[i].split("'");
		document.documentElement.innerHTML += '\n' + part[0] + "'" + part[1];
	}
}
	
function BrowserDataOut()
{
	for (i=0; i < Devices.length; i++)
	{
		var part = Devices[i].split("'");
		MYCONT.innerHTML += "<div id='Bootloader' class='ItemCont' href='sg'>{0}<a tag='UBootloaderTAG' href='{1}' class='btn2'>Download</div></a>".replace("{0}", part[0]).replace("{1}", part[1]);
	}
	for (i=0; i < Tools.length; i++)
	{
		var part = Tools[i].split("'");
		MYCONT.innerHTML += "<div id='Tool' class='ItemCont' href='sg'>{0}<a tag='UToolTAG' href='{1}' class='btn2'>Download</div></a>".replace("{0}", part[0]).replace("{1}", part[1]);
	}
	for (i=0; i < Drivers.length; i++)
	{
		var part = Drivers[i].split("'");
		MYCONT.innerHTML += "<div id='Tool' class='ItemCont' href='sg'>{0}<a tag='UDriverTAG' href='{1}' class='btn2'>Download</div></a>".replace("{0}", part[0]).replace("{1}", part[1]);
	}
	for (i=0; i < Loaders.length; i++)
	{
		var part = Loaders[i].split("'");
		MYCONT.innerHTML += "<div id='Tool' class='ItemCont' href='sg'>{0}<a tag='ULoaderTAG' href='{1}' class='btn2'>Download</div></a>".replace("{0}", part[0]).replace("{1}", part[1]);
	}
	for (i=0; i < Firmwares.length; i++)
	{
		var part = Firmwares[i].split("'");
		MYCONT.innerHTML += "<div id='Tool' class='ItemCont' href='sg'>{0}<a tag='UFirmwareTAG' href='{1}' class='btn2'>Download</div></a>".replace("{0}", part[0]).replace("{1}", part[1]);
	}
	for (i=0; i < GPTS.length; i++)
	{
		var part = GPTS[i].split("'");
		MYCONT.innerHTML += "<div id='Tool' class='ItemCont' href='sg'>{0}<a tag='UGptTAG' href='{1}' class='btn2'>Download</div></a>".replace("{0}", part[0]).replace("{1}", part[1]);
	}
	for (i=0; i < MTKDAS.length; i++)
	{
		var part = MTKDAS[i].split("'");
		MYCONT.innerHTML += "<div id='Tool' class='ItemCont' href='sg'>{0}<a tag='UMtkTAG' href='{1}' class='btn2'>Download</div></a>".replace("{0}", part[0]).replace("{1}", part[1]);
	}
}