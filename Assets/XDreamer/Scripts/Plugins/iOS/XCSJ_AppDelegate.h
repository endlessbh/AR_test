//
//  XCSJ_AppDelegate.h
//  XCVRPlayerDemo
//
//  Created by xcsj on 2017/12/1.
//  Copyright © 2017年 吴庆宏. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "UnityAppController.h"
#import "XCSJ_UnityToIOSDelegate.h"

@interface XCSJ_AppDelegate : UIResponder <UIApplicationDelegate, XCSJ_UnityToIOSDelegate>

@property (strong, nonatomic) UIWindow *window;

@property (strong, nonatomic) UIWindow *unityWindow;

@property (strong, nonatomic) UnityAppController *unityAppController;

- (void)showUnityWindow;

- (void)hideUnityWindow;

- (void)showMainPage;

@end
