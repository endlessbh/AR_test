//
//  XCSJ_UnityAppController.m
//  XCVRPlayerDemo
//
//  Created by xcsj on 2017/12/1.
//  Copyright © 2017年 吴庆宏. All rights reserved.
//

#import "UnityAppController.h"
#import "XCSJ_UnityAppController.h"

#ifdef USE_EASYAR

extern "C" void ezarUnitySetGraphicsDevice(void* device, int deviceType, int eventType);
extern "C" void ezarUnityRenderEvent(int marker);

#endif

@implementation XCSJ_UnityAppController

- (void)shouldAttachRenderDelegate;
{
#ifdef USE_EASYAR
    
    UnityRegisterRenderingPlugin(&ezarUnitySetGraphicsDevice, &ezarUnityRenderEvent);
    
#endif
}

@end
