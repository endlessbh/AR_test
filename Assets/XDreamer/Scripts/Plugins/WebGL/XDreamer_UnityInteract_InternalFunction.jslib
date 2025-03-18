/*
* Copyright @ 北京讯驰视界科技有限公司
* Product: XDreamer
* Author: XCSJ
* Create Date: 2018-11-23 19:44:44
*/

/*
	定义XDreamer与Unity交互内部方法；适用于Unity5.6之后的版本;
	用于处理JS与Unity之间的数据通信；
*/
var XDreamer_UnityInteract_InternalFunction = {

	UnityToJS : function (jsonString)
	{
		try
		{
			if(typeof xdreamer_UnityInteractInstance == "undefined") return;
			xdreamer_UnityInteractInstance.UnityToJS(Pointer_stringify(jsonString));
		}
		catch(ex)
		{
			console.log("XDreamer_UnityInteract_InternalFunction UnityToJS [" + ex.name + " ]: " + ex.message)
		}
	}
};

mergeInto(LibraryManager.library, XDreamer_UnityInteract_InternalFunction);
