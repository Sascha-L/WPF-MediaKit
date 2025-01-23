# WPF MediaKit

This is the official new home of WPF MediaKit (the former was https://wpfmediakit.codeplex.com/). A library to quickly build DirectShow media player controls in WPF. The kit comes with a video player MediaUriElement (a WPF MediaElement replacement), a VideoCaptureElement for web cams and a DVDPlayerElement that plays DVDs and supports interactive menus.

For questions and answers follow the tag `wpf-mediakit` on StackOverflow: [![StackExchange](https://img.shields.io/stackexchange/stackoverflow/t/wpf-mediakit.svg)](http://stackoverflow.com/questions/tagged/wpf-mediakit)

NuGet Package: [![NuGet](https://img.shields.io/nuget/v/WPFMediaKit.svg)](https://www.nuget.org/packages/WPFMediaKit/)

[![Build status](https://ci.appveyor.com/api/projects/status/c9d8fgjridsge83c/branch/master?svg=true)](https://ci.appveyor.com/project/xmedeko/wpf-mediakit/branch/master)

Many thanks to Jeremiah Morrill who originally created and developed this awesome control!

## Contribute
There is still a lot to do! So, please feel free to help, if you are an advanced developer! :-)
Answer issues, open PR, or become a maintainer. Open an issue, if you are willing to help maintaning this project.

## Changelog

Version 3.0:
- Added .NET 8 & .NET 9 compatibility, removed .NET Framework compatibility. If you are still using .NET Framework, please use Version 2.3.0 of WPF MediaKit!

Version 2.3:
- Bugfixes
- Upgraded to .NET Framework 4.8
- Test Application with x64 support

Version 2.2:
- Video snapshots by D3DRenderer.CloneSingleFrameImage()
- D3DRenderer optimizations.
- NuGet package.

Version 2.1:
- Bugfixes (Memory Leak)
- fallback mechanism to auto generate graph if manually generated graph doesn't work
- fix video playback if no audio device is available
- LAV Splitter Source is not hardcorded anymore
- EVR is default Video Renderer

Version 2.0:
- EVR Presenter uses DXVA2 hardware acceleration (thanks to Siegfried Krüger!).
- EVR Presenter doesn't require the DirectX  March 2009 Runtimes anymore (thanks to Siegfried Krüger!).
- DirectShow graph will be built manually to have full control.
- Options to set the desired Video, Audio and Splitter Codecs.
