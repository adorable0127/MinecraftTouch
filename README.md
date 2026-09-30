# 我的世界java版触屏工具 --by xztx127

版本：**1.0.0**
up是因为在学校的希沃白班上游玩想开了什么挂一样。。。所以做了个这个，欢迎给我点个关注（xztx127）或star，也可以给身边的人推荐，源码开源可查，每次提交都有附带源码，万分感谢

这是从所提供 XCL2 源码中提取的独立触屏工具。启动直接显示触摸板管理主界面，没有登录、下载游戏或启动器页面。可以配合任意启动器已打开的 Minecraft Java 版窗口使用。

## 运行环境

- Windows 7 **SP1** 或更新的 Windows，支持 x86 / x64。
- 安装 **.NET Framework 4.8** 运行时；不依赖 .NET 8 运行时、WebView2 或 XCL。
- 真正的多点操作需要 Windows 能识别的触摸屏和驱动。
- Windows 7 原版（未装 SP1）不在此版本的目标范围内。
- 工具的系统要求不改变 Minecraft、Java、显卡驱动自身的系统要求。
- Microsoft 运行时下载：https://dotnet.microsoft.com/download/dotnet-framework/net48
- Microsoft 系统要求：https://learn.microsoft.com/en-us/dotnet/framework/get-started/system-requirements

保留多点同时按键、WASD、跳跃/潜行/疾跑、攻击/使用、快捷栏、滑动视角、背包点击拖放、菜单状态观察、隐藏/恢复、系统屏幕键盘等功能。按键布局保留上一版深蓝选中行和圆角勾选框修复。

关闭一个触摸板只清理对应悬浮层。关闭工具会释放模拟按键并清理所有悬浮层，不结束游戏进程。程序采用单实例，避免重复工具同时注入输入。
