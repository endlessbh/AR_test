//
//  XCSJ_ReadMe.h
//  XCVRPlayerDemo
//
//  Created by xcsj on 2017/12/5.
//  Copyright © 2017年 吴庆宏. All rights reserved.
//

#ifndef XCSJ_ReadMe_h
#define XCSJ_ReadMe_h

/*
 本文档由 北京讯驰视界科技有限公司 (www.smart-mvp.com) 编写并拥有最终解释权！
 任何单位或个人不可私自拷贝或传播！
 
 本文件用于指导Unity嵌入iOS工程的代码部分，工程设置查看视频！
 基于 Unity5.0+ 版本，低于(如 4.0)或高于(如2017+)的版本，方法可能不同！！
 
 **特别说明:本文档用于指导用于使用XCSJ_Unity播放器的情况;如果单独发布App时，请勿根据本文档对工程代码做任何调整或修改！**
 
 嵌入(替换)方法:
 1、将主场景发布iOS工程后，工程文件夹中的Classes、Libraries、Data文件夹完全替换；
 
 2、导入1步骤中的3个文件夹，其中Data文件夹以引用方式导入，其余Classes、Libraries文件夹由非引用方式导入；导入时间较...需耐心等待！
 
 3、在2步骤完成后，将 UnityAppController.h 文件24行代码的 UnityViewControllerBase 替换为 UIViewController，该行其他字符不做修改；修改完成后该行代码为: UIViewController* _viewControllerForOrientation[5];
 
 4、在3步骤完成后，将 UnityAppController.h 文件的第82 到85行代码屏蔽，并修改为如下的代码:
     //inline UnityAppController*    GetAppController()
     //{
     //    return (UnityAppController*)[UIApplication sharedApplication].delegate;
     //}
     
     #import "XCSJ_AppDelegate.h"
     inline UnityAppController*    GetAppController()
     {
     XCSJ_AppDelegate *delegate = (XCSJ_AppDelegate *)[UIApplication sharedApplication].delegate;
     
     return delegate.unityAppController;
     }
 
 5、EasyAR说明:
 5.1 如果启用，则当前工程中包含EasyARAppController.mm文件，需将该文件的最后一行屏蔽，如下:
    //IMPL_APP_CONTROLLER_SUBCLASS(EasyARAppController)
 5.2 如果不启用，需将XCSJ_UnityAppController.h文件中 USE_EASYAR 宏定义屏蔽，如下:
    //#define USE_EASYAR //即 EasyARAppController
 
 6、GoogleVR说明:
 5.1 如果启用，则当前工程中包含CardboardAppController.mm文件，需将该文件的最后一行屏蔽，如下:
    //IMPL_APP_CONTROLLER_SUBCLASS(CardboardAppController)
 5.2 如果不启用，需将XCSJ_UnityAppController.h文件中 USE_GVR 宏定义屏蔽，如下:
    //#define USE_GVR //GoogleVR 即 CardboardAppController
 
 7、到此已经完成替换工作！
 
 8、其他说明:
 8.1 如果用户期望使用XCSJ提供的默认启动项，需将XCSJ_AppDelegate.m文件中最后一行取消屏蔽，如下:
    IMPL_APP_CONTROLLER_SUBCLASS(XCSJ_AppDelegate)
 8.2 如果用户使用自己编写的启动项时，编写的启动项类需继承于XCSJ_AppDelegate类，该类已经封装了OC与C#通行机制，需要时重写对应方法即可；
 
 */

#endif /* XCSJ_ReadMe_h */
