# OverlayPlugin

Plugin to show customizable overlays for Advanced Combat Tracker.

## Download

You can download release and pre-release archives from [release page](https://github.com/ngld/OverlayPlugin/releases).

## System requirements

* .NET Framework 4.7.1
* MS Visual C++ Redistributable for Visual Studio 2019

## How to build

* Install Visual Studio 2019 (earlier versions might work as well but haven't been tested) and the .NET desktop workload through its installer
* Checkout source codes with git, or download source code as ZIP and extract.
* Follow the instructions in the README files in the `Thirdparty` folder.
* Open `OverlayPlugin.sln` in Visual Studio and build the solution.

Once finished, the plugin file `OverlayPlugin.dll` will appear in the `out` folder. It will either be in the `Release` or `Debug` subfolder depending on which configuration you built.

## How to use

To use this plugin, add the `OverlayPlugin.dll` in ACT's plugin tab. It can not be moved around alone, as the files around it are important.

In ACT, go to the `Plugins` tab and then to the `OverlayPlugin.dll` tab. You'll find all overlay related settings there.

Example HTML files are in the `resources` folder.

We also have [instructions for users who want to display their overlay in OBS](./STREAMER.md).

## Troubleshooting

[Check the ACT Discord's FAQ for troubleshooting steps](https://gist.github.com/ngld/e2217563bbbe1750c0917217f136687d)

## License

MIT license. See LICENSE.txt for details.
