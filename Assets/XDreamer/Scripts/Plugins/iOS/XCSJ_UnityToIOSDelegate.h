//
//  XCSJ_UnityToIOS.h
//  XCVRPlayerDemo
//
//  Created by xcsj on 2017/12/1.
//  Copyright © 2017年 吴庆宏. All rights reserved.
//

#ifndef XCSJ_UnityToIOSDelegate_h
#define XCSJ_UnityToIOSDelegate_h

/*
enum EUnityToIOSMsgCmd
{
	None,

	UserDefine,

	BackOS,

	UnityEngineLoadedFinish,

	ImportSceneFinish,

	LoadSceneFinish,

	TimedMessage,

	TimedMessageInImportAndLoad,

	TimedMessageInImport,

	TimedMessageInLoad,

	ImportAndLoadSceneFailed,

	ImportSceneFailed,

	LoadSceneFailed,

	SceneNameList,

	SwitchSceneByOS,

	QRCodeScanResult,

	SingleXCSJScriptRunResult,

	UnloadSubSceneFinish,

	UnloadAllSubSceneFinish,
};
 */

@protocol XCSJ_UnityToIOSDelegate <NSObject>

- (void)UnityToIOS_None:(NSString *)activeSceneName andParam1:(NSString *)param1 andParam2:(NSString *)param2 andParam3:(NSString *)param3 andParam4:(NSString *)param4;

- (void)UnityToIOS_UserDefine:(NSString *)activeSceneName andParam1:(NSString *)param1 andParam2:(NSString *)param2 andParam3:(NSString *)param3 andParam4:(NSString *)param4;

- (void)UnityToIOS_BackOS:(NSString *)activeSceneName andParam1:(NSString *)param1 andParam2:(NSString *)param2;

- (void)UnityToIOS_UnityEngineLoadedFinish:(NSString *)activeSceneName;

- (void)UnityToIOS_ImportSceneFinish:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andScenePath:(NSString *)scenePath;

- (void)UnityToIOS_LoadSceneFinish:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andScenePath:(NSString *)scenePath;

- (void)UnityToIOS_TimedMessage:(NSString *)activeSceneName andRealtimeSinceStartup:(NSString *)realtimeSinceStartup andDeltaTimeOfTimedMessage:(NSString *)deltaTimeOfTimedMessage;

- (void)UnityToIOS_TimedMessageInImportAndLoad:(NSString *)activeSceneName andProgress:(NSString *)progress andSceneName:(NSString *)sceneName;

- (void)UnityToIOS_TimedMessageInImport:(NSString *)activeSceneName andProgress:(NSString *)progress andSceneName:(NSString *)sceneName;

- (void)UnityToIOS_TimedMessageInLoad:(NSString *)activeSceneName andProgress:(NSString *)progress andSceneName:(NSString *)sceneName;

- (void)UnityToIOS_ImportAndLoadSceneFailed:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andScenePath:(NSString *)scenePath;

- (void)UnityToIOS_ImportSceneFailed:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andScenePath:(NSString *)scenePath;

- (void)UnityToIOS_LoadSceneFailed:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andScenePath:(NSString *)scenePath;

- (void)UnityToIOS_SceneNameList:(NSString *)activeSceneName andSceneNameList:(NSArray *)sceneNameList;

- (void)UnityToIOS_SwitchSceneByOS:(NSString *)activeSceneName andScenePath:(NSString *)scenePath andSceneName:(NSString *)sceneName andOtherInfo:(NSString *)otherInfo;

- (void)UnityToIOS_QRCodeScanResult:(NSString *)activeSceneName andParam1:(NSString *)param1 andParam2:(NSString *)param2 andParam3:(NSString *)param3 andParam4:(NSString *)param4;

- (void)UnityToIOS_SingleXCSJScriptRunResult:(NSString *)activeSceneName andXCSJScript:(NSString *)xcsjScript andResult:(NSString *)result;

- (void)UnityToIOS_UnloadSubSceneFinish:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andResult:(NSString *)result;

- (void)UnityToIOS_UnloadAllSubSceneFinish:(NSString *)activeSceneName andResult:(NSString *)result;

- (void)UnityToIOS_Default: (NSString *)jsonString;

@end

#endif /* XCSJ_UnityToIOSDelegate_h */
