//
//  XCSJ_IOSToUnityDelegate.h
//  XCVRPlayerDemo
//
//  Created by xcsj on 2017/12/1.
//  Copyright © 2017年 吴庆宏. All rights reserved.
//

#ifndef XCSJ_IOSToUnityDelegate_h
#define XCSJ_IOSToUnityDelegate_h

/*
 enum EIOSToUnityMsgCmd
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
 };
 */

@protocol XCSJ_IOSToUnityDelegate <NSObject>

-(void)IOSToUnity_ImportAndLoadScene:(NSString*)scenePath andSceneName:(NSString*)sceneName;

-(void)IOSToUnity_ImportScene:(NSString*)scenePath andSceneName:(NSString*)sceneName;

-(void)IOSToUnity_LoadScene:(NSString*)scenePath andSceneName:(NSString*)sceneName;

-(void)IOSToUnity_LoadOrImportAndLoadScene:(NSString*)scenePath andSceneName:(NSString*)sceneName;

-(void)IOSToUnity_UnloadSubScene:(NSString*)sceneName;

-(void)IOSToUnity_UnloadSubSceneByIndex:(int)sceneIndex;

-(void)IOSToUnity_UnloadAllSubScene;

-(void)IOSToUnity_RequestSceneNameList;

-(void)IOSToUnity_UserDefine:(NSString*)userDefine;

-(void)IOSToUnity_CallUserDefineFun:(NSString*)userDefineFunName andParam:(NSString*)param;

-(void)IOSToUnity_RunXCSJScript:(NSString*)xcsjScript;

-(void)IOSToUnity_RunSingleXCSJScriptAndReturnResult:(NSString*)xcsjScript;

-(void)IOSToUnity_RequestImageQRCodeScan:(NSString*)imagePath andOtherInfo:(NSString*)otherInfo;

@end

#endif /* XCSJ_IOSToUnityDelegate_h */
