本项目基于unity平台进行开发，主要使用了vuforia、xdreamer等工具辅助开发。Vuforia是一个专业的增强现实开发平台，提供了强大的图像识别和跟踪功能。Vuforia可以通过图像识别算法，识别和跟踪预先定义的目标图像，并将虚拟内容与之关联起来。
X-Dreamer是一种创新的工具，通过利用先进的机器学习和计算机图形技术，有效地弥合了文本到2D和文本到3D生成之间的领域差距。该工具利用了预训练的视觉和语言模型，如CLIP，并采用扩散模型（如Stable Diffusion）来优化3D表示，使其与文本描述一致。X-Dreamer在需要从文本描述生成详细且准确的3D内容的领域中有广泛应用，如数字内容创建、虚拟现实和增强现实应用。

预期功能：
1、唐代街道图像扫描识别：将手机相机对准要识别的图像，应用便能够准确识别图像，显示出相应的唐代历史街道模型。
2、唐代历史街道模型生成：唐代街道图像被成功识别后，应用会生成街道模型，用户可以通过移动设备的屏幕观察和探索这些模型，欣赏其细节和设计。
3、虚拟按钮交互：为了提供更多的历史文化背景和相关信息，在应用中添加了一个虚拟按钮。当用户触摸虚拟按钮时，会出现该历史图像的文字介绍或宣传视频，介绍这条街道在唐代时发生过的趣闻轶事。
4、语音介绍功能：为了增强用户体验，应用中还包含语音介绍的选项。当用户触摸虚拟按钮，打开文字介绍后，还可以点击打开语音介绍功能，播放唐代历史相关的语音介绍，使用户能够通过听觉感受更加丰富的文化体验。
5、碰撞体交互探索：为一部分模型添加了碰撞体属性，屏幕点击关键物体后出现场景灯光变幻、街道行人出现的效果，点击某些人物还可以触发相关语音，听到历史人物对话的声音，拉近了用户和历史的距离。
6、文物交互：支持对文物模型进行放大、缩小和旋转的操作。用户可以通过触摸屏幕，与模型进行互动，从不同角度欣赏文物，探索其细节和特征。

我使用的是Unity 2021.3.38f1c1、X-Dreamer23.730，二者的版本需要兼容，在配置环境时一定要耐心查看各种说明，下载能互相兼容的版本。

不成熟的大三作业项目，个人还在对XR技术探索中...
