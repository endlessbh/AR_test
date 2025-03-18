package com.xcsj.PluginOSInteract;

public enum EOSToUnityMsgCmd
{
	None,

	ImportAndLoadScene,

	ImportScene,

	LoadScene,

	LoadOrImportAndLoadScene,

	UnloadSubScene,

	UnloadSubSceneByIndex,

	UnloadAllSubScene,

	RequestSceneNameList,

	UserDefine,

	CallUserDefineFun,

	RunXCSJScript,

	RunSingleXCSJScriptAndReturnResult,

	RequestImageQRCodeScan,
}