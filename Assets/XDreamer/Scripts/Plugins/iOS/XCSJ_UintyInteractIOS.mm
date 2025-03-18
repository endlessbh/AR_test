//
//  XCSJ_UintyInteractIOS.m
//  XCVRPlayerDemo
//
//  Created by xcsj on 2017/12/1.
//  Copyright © 2017年 吴庆宏. All rights reserved.
//

#import "UnityInterface.h"
#import "XCSJ_UintyInteractIOS.h"

extern "C"
{
    void UnityToIOS(const char* str)
    {
        if (str==NULL) return;
        
        NSString * jsonString = [NSString stringWithFormat:@"%s",str] ;
        
        [XCSJ_UintyInteractIOS.Instance UnityToIOS:jsonString];
    }
}

@implementation XCSJ_UintyInteractIOS

static XCSJ_UintyInteractIOS *instance = nil;
+(XCSJ_UintyInteractIOS *) Instance
{
    @synchronized(self)
    {
        if(nil == instance)
        {
            instance = [[self alloc] init];
        }
    }
    return instance;
}

//**************** Unity --> IOS *******************//
#pragma mark UnityToIOS

-(void)UnityToIOS:(NSString*)jsonString{
    if(jsonString==nil)return;
    
    NSLog(@"XCSJ_UintyInteractIOS.UnityToIOS--->>%@",jsonString);
    NSDictionary * dic = [[self class] jsonStr2DicOrArray:jsonString];
    if (dic==nil) return;
    
    //@try
    try
    {
        NSString * msgCmd = [dic objectForKey:@"MsgCmd"];
        
        NSDictionary * msg = [dic objectForKey:@"Msg"];
        
        [self UnityToIOSInternal:msgCmd andMsg:msg];
    }
    //@catch (NSException * ex)
    catch (NSException * ex)
    {
        NSLog(@"XCSJ_UintyInteractIOS.UnityToIOS NSException--->>%@",ex);
        
        [self.delegate UnityToIOS_Default:jsonString];
    }
}

-(void)UnityToIOSInternal:(NSString*)msgCmd andMsg:(NSDictionary *)msg
{
	NSString * activeSceneName = [msg objectForKey:@"activeSceneName"];
    if ([msgCmd isEqualToString:@"None"])
    {
        [self.delegate UnityToIOS_None:activeSceneName andParam1:[msg objectForKey:@"param1"] andParam2:[msg objectForKey:@"param2"] andParam3:[msg objectForKey:@"param3"] andParam4:[msg objectForKey:@"param4"]];
    }
	else if ([msgCmd isEqualToString:@"UserDefine"])
    {
        [self.delegate UnityToIOS_UserDefine:activeSceneName andParam1:[msg objectForKey:@"param1"] andParam2:[msg objectForKey:@"param2"] andParam3:[msg objectForKey:@"param3"] andParam4:[msg objectForKey:@"param4"]];
    }
	else if ([msgCmd isEqualToString:@"BackOS"])
    {
        [self.delegate UnityToIOS_BackOS:activeSceneName andParam1:[msg objectForKey:@"param1"] andParam2:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"UnityEngineLoadedFinish"])
    {
        [self.delegate UnityToIOS_UnityEngineLoadedFinish:activeSceneName];
    }
	else if ([msgCmd isEqualToString:@"ImportSceneFinish"])
    {
        [self.delegate UnityToIOS_ImportSceneFinish:activeSceneName andSceneName:[msg objectForKey:@"param1"] andScenePath:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"LoadSceneFinish"])
    {
        [self.delegate UnityToIOS_LoadSceneFinish:activeSceneName andSceneName:[msg objectForKey:@"param1"] andScenePath:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"TimedMessage"])
    {
        [self.delegate UnityToIOS_TimedMessage:activeSceneName andRealtimeSinceStartup:[msg objectForKey:@"param1"] andDeltaTimeOfTimedMessage:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"TimedMessageInImportAndLoad"])
    {
        [self.delegate UnityToIOS_TimedMessageInImportAndLoad:activeSceneName andProgress:[msg objectForKey:@"param1"] andSceneName:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"TimedMessageInImport"])
    {
        [self.delegate UnityToIOS_TimedMessageInImport:activeSceneName andProgress:[msg objectForKey:@"param1"] andSceneName:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"TimedMessageInLoad"])
    {
        [self.delegate UnityToIOS_TimedMessageInLoad:activeSceneName andProgress:[msg objectForKey:@"param1"] andSceneName:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"ImportAndLoadSceneFailed"])
    {
        [self.delegate UnityToIOS_ImportAndLoadSceneFailed:activeSceneName andSceneName:[msg objectForKey:@"param1"] andScenePath:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"ImportSceneFailed"])
    {
        [self.delegate UnityToIOS_ImportSceneFailed:activeSceneName andSceneName:[msg objectForKey:@"param1"] andScenePath:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"LoadSceneFailed"])
    {
        [self.delegate UnityToIOS_LoadSceneFailed:activeSceneName andSceneName:[msg objectForKey:@"param1"] andScenePath:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"SceneNameList"])
    {
		int paramCount =  [[msg objectForKey:@"paramCount"] intValue];
		NSMutableArray * sceneNameList = [[NSMutableArray alloc] init];
		for(int i=1;i<=paramCount;i++)
		{
			[sceneNameList addObject:[msg objectForKey:[NSString stringWithFormat:@"param%d",i]]];
		}
        [self.delegate UnityToIOS_SceneNameList:activeSceneName andSceneNameList:sceneNameList];
    }
	else if ([msgCmd isEqualToString:@"SwitchSceneByOS"])
    {
        [self.delegate UnityToIOS_SwitchSceneByOS:activeSceneName andScenePath:[msg objectForKey:@"param1"] andSceneName:[msg objectForKey:@"param2"] andOtherInfo:[msg objectForKey:@"param3"]];
    }
	else if ([msgCmd isEqualToString:@"QRCodeScanResult"])
    {
        [self.delegate UnityToIOS_QRCodeScanResult:activeSceneName andParam1:[msg objectForKey:@"param1"] andParam2:[msg objectForKey:@"param2"] andParam3:[msg objectForKey:@"param3"] andParam4:[msg objectForKey:@"param4"]];
    }
	else if ([msgCmd isEqualToString:@"SingleXCSJScriptRunResult"])
    {
        [self.delegate UnityToIOS_SingleXCSJScriptRunResult:activeSceneName andXCSJScript:[msg objectForKey:@"param1"] andResult:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"UnloadSubSceneFinish"])
    {
        [self.delegate UnityToIOS_UnloadSubSceneFinish:activeSceneName andSceneName:[msg objectForKey:@"param1"] andResult:[msg objectForKey:@"param2"]];
    }
	else if ([msgCmd isEqualToString:@"UnloadAllSubSceneFinish"])
    {
        [self.delegate UnityToIOS_UnloadAllSubSceneFinish:activeSceneName andResult:[msg objectForKey:@"param1"]];
    }
    else
    {
        //NSLog(@"消息命令未定义--->>%@",msgCmd);
        throw [[NSException alloc] initWithName:@"msgCmd" reason:@"消息命令未定义" userInfo:nil];
    }
}

//**************** IOS --> Unity *******************//
#pragma mark IOSToUnity

-(void)IOSToUnity_ImportAndLoadScene:(NSString*)scenePath andSceneName:(NSString*)sceneName
{
	[self IOSToUnityInternal2:@"ImportAndLoadScene" andKey1:@"scenePath" andValue1:scenePath andKey2:@"sceneName" andValue2:sceneName];
}

-(void)IOSToUnity_ImportScene:(NSString*)scenePath andSceneName:(NSString*)sceneName
{
	[self IOSToUnityInternal2:@"ImportScene" andKey1:@"scenePath" andValue1:scenePath andKey2:@"sceneName" andValue2:sceneName];
}

-(void)IOSToUnity_LoadScene:(NSString*)scenePath andSceneName:(NSString*)sceneName
{
	[self IOSToUnityInternal2:@"LoadScene" andKey1:@"scenePath" andValue1:scenePath andKey2:@"sceneName" andValue2:sceneName];
}

-(void)IOSToUnity_LoadOrImportAndLoadScene:(NSString*)scenePath andSceneName:(NSString*)sceneName
{
	[self IOSToUnityInternal2:@"LoadOrImportAndLoadScene" andKey1:@"scenePath" andValue1:scenePath andKey2:@"sceneName" andValue2:sceneName];
}

-(void)IOSToUnity_UnloadSubScene:(NSString*)sceneName
{
	[self IOSToUnityInternal1:@"UnloadSubScene" andKey1:@"sceneName" andValue1:sceneName];
}

-(void)IOSToUnity_UnloadSubSceneByIndex:(int)sceneIndex
{
	[self IOSToUnityInternal1:@"UnloadSubSceneByIndex" andKey1:@"sceneIndex" andValue1:[NSString stringWithFormat:@"%d",sceneIndex]];
}

-(void)IOSToUnity_UnloadAllSubScene
{
	[self IOSToUnityInternal0:@"UnloadAllSubScene"];
}

-(void)IOSToUnity_RequestSceneNameList
{
	[self IOSToUnityInternal0:@"RequestSceneNameList"];
}

-(void)IOSToUnity_UserDefine:(NSString*)userDefine
{
	[self IOSToUnityInternal1:@"UserDefine" andKey1:@"userDefine" andValue1:userDefine];
}

-(void)IOSToUnity_CallUserDefineFun:(NSString*)userDefineFunName andParam:(NSString*)param
{
	[self IOSToUnityInternal2:@"CallUserDefineFun" andKey1:@"userDefineFunName" andValue1:userDefineFunName andKey2:@"param" andValue2:param];
}


-(void)IOSToUnity_RunXCSJScript:(NSString*)xcsjScript
{
	[self IOSToUnityInternal1:@"RunXCSJScript" andKey1:@"xcsjScript" andValue1:xcsjScript];
}

-(void)IOSToUnity_RunSingleXCSJScriptAndReturnResult:(NSString*)xcsjScript
{
	[self IOSToUnityInternal1:@"RunSingleXCSJScriptAndReturnResult" andKey1:@"xcsjScript" andValue1:xcsjScript];
}

-(void)IOSToUnity_RequestImageQRCodeScan:(NSString*)imagePath andOtherInfo:(NSString*)otherInfo
{
	[self IOSToUnityInternal2:@"RequestImageQRCodeScan" andKey1:@"imagePath" andValue1:imagePath andKey2:@"otherInfo" andValue2:otherInfo];
}


-(void)IOSToUnityInternal0:(NSString*)msgCmd
{
	[self IOSToUnityInternal1:msgCmd andKey1:nil andValue1:nil];
}

-(void)IOSToUnityInternal1:(NSString*)msgCmd andKey1:(NSString *)key1 andValue1:(NSString *)value1
{
	[self IOSToUnityInternal2:msgCmd andKey1:key1 andValue1:value1 andKey2:nil andValue2:nil];
}

-(void)IOSToUnityInternal2:(NSString*)msgCmd andKey1:(NSString *)key1 andValue1:(NSString *)value1 andKey2:(NSString *)key2 andValue2:(NSString *)value2
{
    NSMutableDictionary * msg = [[NSMutableDictionary alloc] init];
    if(key1!=nil && key1.length>0) [msg setObject:value1 forKey:key1];
    if(key2!=nil && key1.length>0) [msg setObject:value2 forKey:key2];
    
    [self IOSToUnityInternal:msgCmd andMsg:msg];
}

-(void)IOSToUnityInternal:(NSString*)msgCmd andMsg:(NSDictionary *)msg
{
    NSMutableDictionary * dic = [[NSMutableDictionary alloc] init];
    
    [dic setObject:msgCmd forKey:@"MsgCmd"];
    [dic setObject:msg forKey:@"Msg"];
    
    NSString * jsonString = [[self class] dicOrArray2JsonStr:dic];
    NSLog(@"发送消息JSON: %@", jsonString);
    
    [self UnityToIOS:jsonString];
    
    NSLog(@"发送消息完成！");
}

-(void)IOSToUnity:(NSString*)jsonString
{
	//UnitySendMessage("OSInteractManager", "OSCallUnityFun", [jsonString UTF8String]);
	UnitySendMessage("XDreamer_OSInteract", "OSToUnity", [jsonString UTF8String]);
}

//**************** 部分静态函数 *******************//
#pragma mark 部分静态函数

//字典或数组转字符串(json 格式）
+ (NSString *)dicOrArray2JsonStr:(id)dic
{
    NSData *data = [[self class] dicOrArray2JsonData:dic];
    NSString *str = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    return str;
}

//字符串（json格式）转字典或数组
+ (id)jsonStr2DicOrArray:(NSString *)str
{
    if(str == nil || [str isEqualToString:@""])
    {
        return nil;
    }
    
    NSData *data = [str dataUsingEncoding:NSUTF8StringEncoding];
    return [[self class] jsonData2DicOrArray:data];
}

//Json值转data（json 格式）
+ (NSData *)dicOrArray2JsonData:(id)jsonValue
{
    if([NSJSONSerialization isValidJSONObject:jsonValue])
    {
        NSError *error;
        NSData *data = [NSJSONSerialization dataWithJSONObject:jsonValue options:NSJSONWritingPrettyPrinted error:&error];
        return data;
    }
    return nil;
}

//data（json格式）转字典或数组
+ (id)jsonData2DicOrArray:(NSData *)data
{
    NSError *error;
    id dic = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingMutableContainers error:&error];
    if(error)
    {
        NSLog(@"JSON解析错误: %ld, 错误信息:%@", (long)error.code, error.description);
    }
    return dic;
}

@end
