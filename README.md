# MinecraftTouch

### 让 Minecraft Java Edition 真正拥有一套属于触摸屏的操作方式。

> **不用 Mod。**
> **不用修改 Minecraft。**
> **不用改变游戏本身。**
> **只需要一个 Windows 工具，就能把触摸屏变成 Minecraft 的虚拟键鼠。**

如果你曾经尝试过在 Windows 平板、二合一设备或触摸屏电脑上玩 Minecraft Java Edition，你应该知道那种感觉：

**游戏能启动。**

**画面也能正常显示。**

**但是你的手指不知道该放在哪里。**

WASD、空格、Shift、鼠标左键、右键、滚轮、F3、ESC……

这些原本为实体键盘和鼠标设计的操作，在触摸屏上突然全部变成了问题。

MinecraftTouch 就是为了解决这件事而诞生的。

---

## 🎮 这不是一个 Minecraft Mod

MinecraftTouch 不需要安装到 `.minecraft`。

不需要 Forge。

不需要 Fabric。

不需要 NeoForge。

不需要修改 Minecraft 客户端。

它运行在 **Windows 桌面输入层**，通过原生 Windows 输入接口，把你的触摸操作转换成 Minecraft 能理解的键盘和鼠标输入。

所以：

**Minecraft 不需要知道 MinecraftTouch 的存在。**

这也意味着它可以服务于不同版本、不同 Mod 环境，甚至不同的 Minecraft 启动方式。

---

# ✨ 它真正改变的，是“触摸 Minecraft”的方式

打开 MinecraftTouch。

启动 Minecraft。

选择游戏窗口。

**触摸屏上的 Minecraft，就这样出现了。**

左下角：

```text
        W
      A   D
        S
```

右下角：

```text
        跳跃

   使用       攻击

       背包
```

顶部还可以拥有：

```text
ESC   F3   DEL   F5   F11   聊天   TAB   F1
```

快捷栏也可以直接触摸。

而屏幕中央，则保留给 Minecraft 最重要的东西：

**视角。**

---

# 🖐️ 真正的多指操作

MinecraftTouch 并不是简单地把屏幕做成一块“虚拟键盘”。

它支持真正意义上的多触点操作。

例如：

> 一根手指按住 W 前进
>
> 第二根手指按住跳跃
>
> 第三根手指拖动画面控制视角
>
> 第四根手指进行攻击

这些操作可以同时发生。

你不需要在“移动”和“转视角”之间来回切换。

**你的手指可以同时做不同的事情。**

---

# 👁️ 游戏视角与菜单操作，完全不同

这是 MinecraftTouch 输入系统最重要的设计之一。

Minecraft 在正常游戏状态下，会锁定鼠标光标。

这个时候：

**移动鼠标 = 转动视角。**

因此 MinecraftTouch 在游戏状态下不会粗暴地把手指移动转换成屏幕绝对坐标。

它使用：

```text
手指移动
    ↓
计算相对位移
    ↓
Windows 相对鼠标输入
    ↓
Minecraft 视角旋转
```

这样才能让触摸屏上的“拖动视角”真正像鼠标一样工作。

---

## 🖱️ 而进入背包 / 菜单后

情况完全不同。

这时候 Minecraft 需要的是：

```text
手指点哪里
        ↓
鼠标光标移动到哪里
        ↓
执行点击
```

因此 MinecraftTouch 会在不同输入状态之间切换：

### 游戏状态

**相对鼠标输入 → 控制视角**

### 菜单状态

**绝对鼠标定位 → 操作界面**

这也是为什么 MinecraftTouch 并不是简单的“透明窗口 + SendInput”。

它需要持续观察游戏窗口、鼠标约束和光标状态，并判断当前到底应该采用哪一种输入模式。

---

# ⚡ 不抢 Minecraft 的焦点

MinecraftTouch 的悬浮层使用 Windows 原生窗口机制创建。

它可以：

* 保持在游戏上方
* 不显示在任务栏
* 不主动抢走游戏焦点
* 透明显示
* 跟随 Minecraft 游戏窗口
* 游戏关闭后自动结束对应悬浮层

因此你看到的是：

```text
┌─────────────────────────────┐
│                             │
│        Minecraft             │
│                             │
│                         F3  │
│                             │
│                             │
│   A W D              跳跃   │
│       S              攻击   │
└─────────────────────────────┘
```

而不是一个盖在 Minecraft 上面的普通软件窗口。

---

# 🧩 按键不是写死的

MinecraftTouch 的虚拟按键全部可以配置。

你可以修改：

| 项目       | 支持 |
| -------- | -- |
| 按键名称     | ✅  |
| 键盘绑定     | ✅  |
| 点按       | ✅  |
| 按住       | ✅  |
| 切换保持     | ✅  |
| 功能命令     | ✅  |
| 显示范围     | ✅  |
| 锚点       | ✅  |
| X / Y 坐标 | ✅  |
| 宽度 / 高度  | ✅  |
| 层级       | ✅  |
| 是否启用     | ✅  |

甚至可以直接录入实体键盘按键。

---

# 🎛️ 不只是 WASD

MinecraftTouch 内置支持大量输入：

```text
A-Z
0-9

F1-F24

ESC
ENTER
TAB
SHIFT
CTRL
ALT

INSERT
DELETE
HOME
END
PAGEUP
PAGEDOWN

方向键

鼠标左键
鼠标右键
鼠标中键

滚轮上
滚轮下
```

还支持组合键，例如：

```text
CTRL+W
SHIFT+某个按键
ALT+某个按键
```

因此它并不局限于 Minecraft 默认控制。

---

# 🖱️ 鼠标也可以成为触摸按键

例如：

```text
攻击       → MOUSE_L
使用       → MOUSE_R
滚轮       → WHEEL_UP
```

所以你可以构建自己的：

**移动 + 视角 + 攻击 + 使用 + 快捷栏**

完整触摸控制系统。

---

# 🧠 智能菜单控制

MinecraftTouch 还可以把虚拟按键和 Minecraft 菜单状态结合起来。

例如：

**ESC**

可以被定义为菜单切换。

**聊天**

可以打开聊天。

**背包**

可以作为菜单状态切换操作。

这样虚拟按键就不再只是“把键盘贴到屏幕上”。

它开始理解：

> **我现在是在游戏里，还是在菜单里？**

---

# ⌨️ 触摸屏也需要键盘

打开聊天怎么办？

输入服务器地址怎么办？

输入命令怎么办？

MinecraftTouch 可以直接调用 Windows 自带的：

**屏幕键盘。**

不需要额外安装第三方输入法或虚拟键盘程序。

---

# 🛠️ 为真实使用场景设计

MinecraftTouch 不只是做了一个 Demo。

项目内部针对实际使用场景处理了大量边缘情况。

例如：

### 多显示器

支持虚拟桌面坐标。

### Windows DPI 缩放

触摸布局使用 DIP 坐标进行保存。

### 窗口模式

悬浮层跟随 Minecraft 客户区，而不是简单覆盖整个窗口。

这样可以避免把 Minecraft 原生标题栏按钮挡住。

### 游戏退出

游戏关闭后自动释放：

* 虚拟按键
* 鼠标按钮
* 悬浮层
* 输入会话

避免出现：

> “Minecraft 都关了，为什么我的角色还在一直往前走？”

---

# 🔒 不碰你的 Minecraft 文件

MinecraftTouch 的设计目标之一就是：

**尽可能把 Minecraft 与触摸控制工具解耦。**

它不需要把 DLL 塞进 Minecraft。

不需要修改游戏文件。

不需要往 `.minecraft` 里安装组件。

你的 Minecraft 仍然是你的 Minecraft。

MinecraftTouch 只是站在游戏旁边：

> **把你的手指翻译成 Minecraft 听得懂的输入。**

---

# 🏗️ 技术栈

MinecraftTouch 当前基于：

* **C#**
* **WPF**
* **.NET**
* **Win32 API**
* **Windows `SendInput`**
* Windows 窗口 / 光标 / 输入相关 API

核心结构大致可以理解为：

```text
                 ┌────────────────────┐
                 │    MinecraftTouch  │
                 └─────────┬──────────┘
                           │
               ┌───────────┴───────────┐
               │                       │
        游戏窗口发现                配置系统
               │                       │
               ▼                       ▼
        Minecraft HWND          JSON 布局配置
               │
               ▼
        TouchOverlayWindow
               │
        ┌──────┼──────┐
        │      │      │
       触摸   鼠标   键盘
        │      │      │
        └──────┼──────┘
               ▼
        Windows Input Layer
               │
               ▼
          Minecraft Java
```

---

# 🧪 不只是“能跑”

项目中还包含：

* 输入绑定合法性检查
* 布局配置验证
* 配置升级机制
* 自动保存
* 日志
* 自测代码
* 游戏进程生命周期管理
* 多实例悬浮层会话管理
* 残留输入释放
* DPI / 多屏坐标处理
* 游戏窗口状态检测

换句话说：

**目标不是做一个能演示 30 秒的触摸键盘。**

而是做一个可以真正拿来长期玩 Minecraft 的工具。

---

# 🚀 快速开始

## 1. 启动 Minecraft

正常启动你的 Minecraft Java Edition。

## 2. 启动 MinecraftTouch

程序会自动扫描当前 Windows 桌面中的游戏窗口。

Java / JavaW / GLFW / LWJGL 窗口会被优先识别。

## 3. 选择 Minecraft 窗口

选择你想要控制的 Minecraft 实例。

## 4. 开启触摸板

点击：

**开启触摸板**

随后虚拟按键会出现在游戏窗口上。

## 5. 开始游戏

现在：

**你的触摸屏就是 Minecraft 的控制器。**

---

# 🎨 自定义你的 Minecraft

默认布局只是开始。

你可以进入：

**触屏按键与布局设置**

然后创造自己的布局。

例如：

### 平板布局

大按钮、低密度。

### 手机式布局

按钮集中在两侧。

### PvP 布局

攻击、切换物品、跳跃、潜行全部放在最顺手的位置。

### 建筑布局

把快捷栏、复制动作、切换视角等功能放到手指最容易触及的位置。

甚至可以：

**完全删除默认布局，然后从零开始设计。**

---

# 💡 MinecraftTouch 适合谁？

### 🖥️ Windows 平板用户

Surface、二合一设备、触摸屏笔记本等。

### 🎮 喜欢躺着玩 Minecraft 的人

没有鼠标？

没关系。

### 🧪 Minecraft 技术玩家

不想为了一个输入工具修改游戏或安装 Mod。

### 🛠️ 启动器作者

如果你的启动器本身就管理 Minecraft 实例，那么 MinecraftTouch 的设计思路也非常容易理解：

**找到游戏窗口 → 创建输入层 → 跟随游戏 → 管理生命周期。**

---

# 🌟 为什么是 MinecraftTouch？

因为 Minecraft Java Edition 从来不是为触摸屏设计的。

但这不代表它不能拥有触摸操作。

MinecraftTouch 做的事情很简单：

> **把键盘和鼠标之间的距离，缩短成你的手指。**

没有 Mod。

没有魔法。

没有修改 Minecraft。

只有一层精心设计的 Windows 输入转换。

---

# 📌 项目状态

MinecraftTouch 当前处于早期公开版本。

核心触摸控制、虚拟按键、游戏窗口检测、输入注入、菜单/游戏模式切换、自定义布局等功能已经形成完整的工作链路。

项目仍会继续完善。

如果你在 Windows 触摸屏上玩 Minecraft Java Edition，并且觉得：

> **“为什么这么多年了，Java 版还是不能好好用触摸屏？”**

那么——

**也许你正在寻找的，就是 MinecraftTouch。**

---

## ⭐ 如果它帮到了你

欢迎：

* ⭐ Star 项目
* 🐛 提交 Issue
* 💡 提交功能建议
* 🔧 提交 Pull Request
* 📢 分享给其他 Windows 触摸屏 Minecraft 玩家

项目地址：

https://github.com/adorable0127/MinecraftTouch

---

### MinecraftTouch

**Minecraft Java Edition × Windows Touch**

**让你的手指，成为 Minecraft 的控制器。**
