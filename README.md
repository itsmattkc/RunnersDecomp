# RunnersDecomp by [MattKC](https://github.com/itsmattkc) and [Ramen2X](https://github.com/Ramen2X)

The original Sonic Runners (2015) game client!

This project requires `Unity Pro 4.6.9` `CriWare Unity SDK v1.20.3.0` and `NGUI v3.0.2`, we will NOT provide the aforementioned software and plugins, you will have to get them legally.

# Before opening the project:
It is recommended that you place your CriWare Unity SDK and NGUI in the project directory.
- Place your legally obtained CriWare plugin into `Assets\Plugins`.
- Place your legally obtained NGUI plugin into `Assets\NGUI`.
By doing this, you will prevent scenes from losing component references related to these plugins.

# Important, don't forget the AssetBundles!
You will need to download the AssetBundles decomp and place it into the project if you want to playtest
using the Unity Editor.

- Download the [RunnersAssetBundleDecomp](https://github.com/itsmattkc/RunnersAssetBundleDecomp).
- Make a new folder in the project root named `AssetBundles`
- Place the [RunnersAssetBundleDecomp files](https://github.com/itsmattkc/RunnersAssetBundleDecomp) inside of the folder you just created.
- Add every scene into the Unity Build Settings.
- Go to `Env.cs` and set `m_useAssetBundle` to `false` (this will make the game load local scenes).

If you plan on building, make sure to uncheck unnecessary scenes and to set `m_useAssetBundle` back to `true`

<br />
<br />
<br />

# Building for Android
- Ensure the project is targeting Android in Unity.
- Install the Android 6.0 (Marshmallow) SDK.
- Ensure Unity is set to build against the Android 6.0 SDK in Unity's settings.
- Download [JDK 7](http://dl-ssl.google.com/android/repository/tools_r25.2.5-windows.zip).
- Replace the `tools` folder in your Android 6.0 SDK with the one from JDK 7.
- Build for Android through Unity! It should output an APK file of the game.

# Building for iOS (requires macOS)
- Ensure the project is targeting iOS in Unity.
- Go to the Player Settings for iOS in Unity.
- Ensure that the Graphics API is set to Metal.
- Build the Xcode project through Unity (This could take upwards of an hour).
- Add libstdc++ libraries from Xcode 9 into your Xcode 12 installation, as they are no longer included by default (You will need to install both versions, you can later remove Xcode9).
- Open the Xcode project with Xcode 12.
- In Xcode, set the project to use the Legacy Build System in "Project Settings".
- In "Build Settings", add `-fdeclspec` as a C build flag.
- In "Info", under "Custom iOS Target Settings", add `App Transport Security Settings`, and then add `Allow Arbitrary Loads` and set it to YES.
- Also in "Info", add `Application supports iTunes file sharing` and set it to YES.
- In "General", under "Frameworks, Libraries, and Embedded Content", add `VideoToolbox.framework`, `Metal.framework`, `MetalKit.framework`, and `MetalPerformanceShaders.framework`.
- If you get a permissions error upon building, ensure that you give permission to `MapFileParser.sh` to execute by running `chmod +x MapFileParser.sh`
- Compile the Xcode project and deploy to your iOS device.

<br />
<br />
<br />
<br />
<br />

If you run into any issues, please let us know.<br />
Open source preparations made by [\_F121_](https://github.com/F121Live).
