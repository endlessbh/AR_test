//
//  XCSJ_AppDelegate.m
//  XCVRPlayerDemo
//
//  Created by xcsj on 2017/12/1.
//  Copyright © 2017年 吴庆宏. All rights reserved.
//

#import "XCSJ_AppDelegate.h"
#import "XCSJ_UintyInteractIOS.h"
#import "XCSJ_UnityAppController.h"

@implementation XCSJ_AppDelegate

- (UIWindow *)unityWindow
{
    return UnityGetMainWindow();
}

- (void)showUnityWindow
{
    [self.unityWindow makeKeyAndVisible];
}

- (void)hideUnityWindow
{
    [self.window makeKeyAndVisible];
}

- (void)showMainPage
{
    //
}

//**************** UIApplicationDelegate *******************//
#pragma mark UIApplicationDelegate

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    // Override point for customization after application launch.
    
    CGRect rect =[UIScreen mainScreen].bounds ;
    self.window = [[UIWindow alloc] initWithFrame:rect];
    //self.window.backgroundColor = [SkinManager Instance].content_background_color;
    
    //self.unityController = [[UnityAppController alloc] init];
    self.unityAppController = [[XCSJ_UnityAppController alloc] init];
    [self.unityAppController application:application didFinishLaunchingWithOptions:launchOptions];
    
    XCSJ_UintyInteractIOS.Instance.delegate = self;
    
    [self showMainPage];
    return YES;
}

- (void)applicationWillResignActive:(UIApplication *)application {
    // Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
    // Use this method to pause ongoing tasks, disable timers, and throttle down OpenGL ES frame rates. Games should use this method to pause the game.
    
    //    return ;
    [self.unityAppController applicationWillResignActive:application];
}

- (void)applicationDidEnterBackground:(UIApplication *)application {
    // Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later.
    // If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.
    
    //    return ;
    [self.unityAppController applicationDidEnterBackground:application];
}

- (void)applicationWillEnterForeground:(UIApplication *)application {
    // Called as part of the transition from the background to the inactive state; here you can undo many of the changes made on entering the background.
    
    //    [self.unityController applicationWillEnterForeground:application];
}

- (void)applicationDidBecomeActive:(UIApplication *)application {
    // Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
    
    //    return ;
    [self.unityAppController applicationDidBecomeActive:application];
}

- (void)applicationWillTerminate:(UIApplication *)application {
    // Called when the application is about to terminate. Save data if appropriate. See also applicationDidEnterBackground:.
    
    //    return ;
    [self.unityAppController applicationWillTerminate:application];
}

//**************** UnityToIOS *******************//
#pragma mark UnityToIOS --> 回调函数


- (void)UnityToIOS_None:(NSString *)activeSceneName andParam1:(NSString *)param1 andParam2:(NSString *)param2 andParam3:(NSString *)param3 andParam4:(NSString *)param4
{
	//
}

- (void)UnityToIOS_UserDefine:(NSString *)activeSceneName andParam1:(NSString *)param1 andParam2:(NSString *)param2 andParam3:(NSString *)param3 andParam4:(NSString *)param4
{
	//
}

- (void)UnityToIOS_BackOS:(NSString *)activeSceneName andParam1:(NSString *)param1 andParam2:(NSString *)param2
{
	//
}

- (void)UnityToIOS_UnityEngineLoadedFinish:(NSString *)activeSceneName
{
	//
}

- (void)UnityToIOS_ImportSceneFinish:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andScenePath:(NSString *)scenePath
{
	//
}

- (void)UnityToIOS_LoadSceneFinish:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andScenePath:(NSString *)scenePath
{
	//
}

- (void)UnityToIOS_TimedMessage:(NSString *)activeSceneName andRealtimeSinceStartup:(NSString *)realtimeSinceStartup andDeltaTimeOfTimedMessage:(NSString *)deltaTimeOfTimedMessage
{
	//
}

- (void)UnityToIOS_TimedMessageInImportAndLoad:(NSString *)activeSceneName andProgress:(NSString *)progress andSceneName:(NSString *)sceneName
{
	//
}

- (void)UnityToIOS_TimedMessageInImport:(NSString *)activeSceneName andProgress:(NSString *)progress andSceneName:(NSString *)sceneName
{
	//
}

- (void)UnityToIOS_TimedMessageInLoad:(NSString *)activeSceneName andProgress:(NSString *)progress andSceneName:(NSString *)sceneName
{
	//
}

- (void)UnityToIOS_ImportAndLoadSceneFailed:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andScenePath:(NSString *)scenePath
{
	//
}

- (void)UnityToIOS_ImportSceneFailed:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andScenePath:(NSString *)scenePath
{
	//
}

- (void)UnityToIOS_LoadSceneFailed:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andScenePath:(NSString *)scenePath
{
	//
}

- (void)UnityToIOS_SceneNameList:(NSString *)activeSceneName andSceneNameList:(NSArray *)sceneNameList
{
	//
}

- (void)UnityToIOS_SwitchSceneByOS:(NSString *)activeSceneName andScenePath:(NSString *)scenePath andSceneName:(NSString *)sceneName andOtherInfo:(NSString *)otherInfo
{
	//
}

- (void)UnityToIOS_QRCodeScanResult:(NSString *)activeSceneName andParam1:(NSString *)param1 andParam2:(NSString *)param2 andParam3:(NSString *)param3 andParam4:(NSString *)param4
{
	//
}

- (void)UnityToIOS_SingleXCSJScriptRunResult:(NSString *)activeSceneName andXCSJScript:(NSString *)xcsjScript andResult:(NSString *)result
{
	//
}

- (void)UnityToIOS_UnloadSubSceneFinish:(NSString *)activeSceneName andSceneName:(NSString *)sceneName andResult:(NSString *)result
{
	//
}

- (void)UnityToIOS_UnloadAllSubSceneFinish:(NSString *)activeSceneName andResult:(NSString *)result
{
	//
}

- (void)UnityToIOS_Default: (NSString *)jsonString
{
    //
}

@end

//IMPL_APP_CONTROLLER_SUBCLASS(XCSJ_AppDelegate)
