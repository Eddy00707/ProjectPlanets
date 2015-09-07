try{
if (cookiestate == 0 && document.cookie.indexOf('ar_g2') == -1){
	sC('ar_g2', 1, 1000*60*60*24*7);
	document.write('<img src="//cm.g.d'+'oubleclick.net/pixel?google_nid=ADR&google_cm&google_sc">');
}
}catch(e){}