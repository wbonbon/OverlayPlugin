# OverlayPlugin

显示可自定义模板的ACT插件。

## 下载

你可以从[release page](https://github.com/ngld/OverlayPlugin/releases)下载发行版和预发行版档案。

## 系统要求

* .NET Framework 4.7.1
* MS Visual C++ Redistributable for Visual Studio 2019

## 如何编译

* 安装Visual Studio 2019（早期版本未经测试但可能可以工作）和.NET桌面工作负载工具
* 用git查看源代码，或将源代码下载为ZIP文件并解压缩。
* 请遵循`Thirdparty`文件夹中README文件中的说明。
* 在Visual Studio中打开`OverlayPlugin.sln`并建立解决方案。

完成后, 插件文件`OverlayPlugin.dll`将出现在`out`文件夹中。根据您所构建的配置，它将位于`Release`或`Debug`子文件夹中。

## 如何使用

要使用此插件，请在ACT的`插件`选项卡中添加`OverlayPlugin.dll`. 此文件不可单独移动，因为周围的其他文件很重要。

在ACT中，转到`插件`选项卡，然后转到`OverlayPlugin.dll`选项卡。您将在此找到所有与模板相关的设置。

示例HTML文件位于`resources`文件中。

我们还为[想要在OBS中显示模板的用户](./STREAMER_CN.md)提供了说明文件。

## 故障排除

[查看ACT Discord的FAQ页面以进行故障排除](https://gist.github.com/ngld/e2217563bbbe1750c0917217f136687d)

## 许可证

MIT许可证。有关详情请参见LICENSE.txt。
