//
//  XCSJ_UintyInteractIOS.h
//  XCVRPlayerDemo
//
//  Created by xcsj on 2017/12/1.
//  Copyright © 2017年 吴庆宏. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "XCSJ_IOSToUnityDelegate.h"
#import "XCSJ_UnityToIOSDelegate.h"

@interface XCSJ_UintyInteractIOS : NSObject<XCSJ_IOSToUnityDelegate>

+(XCSJ_UintyInteractIOS *) Instance;

//**************** Unity --> IOS *******************//
#pragma mark UnityToIOS

@property(weak,nonatomic) id<XCSJ_UnityToIOSDelegate> delegate;

-(void)UnityToIOS:(NSString*)jsonString;

//**************** IOS --> Unity *******************//
#pragma mark IOSToUnity

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

-(void)IOSToUnity:(NSString*)jsonString;

//**************** 部分静态函数 *******************//
#pragma mark 部分静态函数

//字典或数组转字符串(json 格式）
+ (NSString *)dicOrArray2JsonStr:(id)dic;

//字符串（json格式）转字典或数组
+ (id)jsonStr2DicOrArray:(NSString *)str;

//Json值转data（json 格式）
+ (NSData *)dicOrArray2JsonData:(id)jsonValue;

//data（json格式）转字典或数组
+ (id)jsonData2DicOrArray:(NSData *)data;

@end
