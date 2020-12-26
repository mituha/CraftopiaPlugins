開発環境
======================================

## 使用ツール
MODの作成、コンパイルは、[Visual Studio 2019 Community](https://visualstudio.microsoft.com/ja/downloads/) で行っています。  

このドキュメント等は、[Visual Studio Code](https://visualstudio.microsoft.com/ja/downloads/) に [Markdown Preview Enhanced](https://marketplace.visualstudio.com/items?itemName=shd101wyy.markdown-preview-enhanced) を追加してマークダウンを編集、閲覧しています。  

ソース管理は、[Git Extensions](http://gitextensions.github.io/)を使用して、[GitHub](https://github.com/mituha/CraftopiaPlugins) で行っています。  

## コンパイル
//TODO


## 参照DLL
//TODO
v20201221.2322 以降でDLLが分離しているため、必要なDLLを再検証が必要

### Craftopia関連
`C:\Program Files (x86)\Steam\steamapps\common\Craftopia\Craftopia_Data\Managed` 等にインストールされている、ファイルから必要なDLLを参照します。  
Craftopia関連、Unity関連、サードパーティ関連があります。  
`AD_`、または、`_AD` で始まるDLLは Assembly Definition により、元々は Assembly-CSharp.dll に含まれていた機能がDLLに分割されたものです。  
`External` が付いているものはサードパーティの機能部分と思われます。   

DLL参照用に、ソリューションのLibsフォルダーに下記のようにしてシンボリックリンクを貼ります。

```
Remove-Item .\Assembly-CSharp.dll
New-Item -Value "D:\Program Files (x86)\Steam\steamapps\common\Craftopia\Craftopia_Data\Managed\Assembly-CSharp.dll" -Path .\Assembly-CSharp.dll -ItemType SymbolicLink
```
//TODO スクリプト化したほうが良い


* Assembly-CSharp.dll
* AD__Overcraft.dll


//TODO  

AD_External_i2.dll
_AD_External_AdvancedDissolve.dll
_AD_External_AzureSky.dll
_AD_External_BendinGrass.dll
_AD_External_BestHttp.dll
_AD_External_ConsolePro.dll
_AD_External_DOTween.dll
_AD_External_EasySave3.dll
_AD_External_FromOtherSource_Forum.dll
_AD_External_MantisLOD.dll
_AD_External_Steamworks.net.dll
_AD_External_VRoidSDK.dll

Accessibility.dll

AloneSoft.VeryAnimation.dll
Assembly-CSharp-firstpass.dll

Autodesk.Fbx.dll
ch.sycoforge.Decal.dll
ch.sycoforge.Logging.dll
ch.sycoforge.Unity.Runtime.dll
Coffee.UIParticle.dll
DemiLib.dll
DOTween.dll
DOTweenPro.dll
LeTai.TranslucentImage.dll
MeshBakerCore.dll
Mono.Data.Sqlite.dll
Mono.Posix.dll
Mono.Security.dll
Mono.WebBrowser.dll
mscorlib.dll
NativeCollections.dll
NavMeshComponents.dll
netstandard.dll
Newtonsoft.Json.dll
Novell.Directory.Ldap.dll
Popcron.Gizmos.dll
sc.vegetationspawner.runtime.dll
SimpleAnimationComponent.dll
Sirenix.OdinInspector.Attributes.dll
System.ComponentModel.Composition.dll
System.ComponentModel.DataAnnotations.dll
System.Configuration.dll
System.Core.dll
System.Data.dll
System.Design.dll
System.Diagnostics.StackTrace.dll
System.DirectoryServices.dll
System.dll
System.Drawing.Design.dll
System.Drawing.dll
System.EnterpriseServices.dll
System.Globalization.Extensions.dll
System.IO.Compression.dll
System.IO.Compression.FileSystem.dll
System.Net.Http.dll
System.Numerics.dll
System.Runtime.dll
System.Runtime.Serialization.dll
System.Runtime.Serialization.Formatters.Soap.dll
System.Runtime.Serialization.Xml.dll
System.Security.dll
System.ServiceModel.Internals.dll
System.Transactions.dll
System.Web.ApplicationServices.dll
System.Web.dll
System.Web.Services.dll
System.Windows.Forms.dll
System.Xml.dll
System.Xml.Linq.dll
System.Xml.XPath.XDocument.dll
Trivial.CodeSecurity.dll
Trivial.Mono.Cecil.dll
Trivial.Mono.Cecil.Mdb.dll
Trivial.Mono.Cecil.Pdb.dll
UMod-Interface.dll
UMod-Shared.dll
UMod.dll
UniRx.Async.dll
UniRx.dll
Unity.Analytics.DataPrivacy.dll
Unity.Analytics.StandardEvents.dll
Unity.Analytics.Tracker.dll
Unity.AutoLOD.dll
Unity.Build.SlimPlayerRuntime.dll
Unity.Burst.dll
Unity.Burst.Unsafe.dll
Unity.Collections.dll
Unity.Collections.LowLevel.ILSupport.dll
Unity.Deformations.dll
Unity.Entities.dll
Unity.Entities.Hybrid.dll
Unity.Formats.Fbx.Runtime.dll
Unity.InputSystem.dll
Unity.InputSystem.RebindingUI.dll
Unity.Jobs.dll
Unity.Mathematics.dll
Unity.Mathematics.Extensions.dll
Unity.Mathematics.Extensions.Hybrid.dll
Unity.MemoryProfiler.dll
Unity.NetCode.dll
Unity.NetCode.Generated.dll
Unity.Networking.Transport.dll
Unity.Platforms.Common.dll
Unity.Postprocessing.Runtime.dll
Unity.Properties.dll
Unity.Properties.Reflection.dll
Unity.Properties.UI.dll
Unity.Recorder.dll
Unity.Rendering.Hybrid.dll
Unity.Scenes.dll
Unity.ScriptableBuildPipeline.dll
Unity.Serialization.dll
Unity.TerrainTools.dll
Unity.TextMeshPro.dll
Unity.Timeline.dll
Unity.Transforms.dll
Unity.Transforms.Generated.dll
Unity.Transforms.Hybrid.dll
UnityEngine.AccessibilityModule.dll
UnityEngine.AIModule.dll
UnityEngine.AndroidJNIModule.dll
UnityEngine.AnimationModule.dll
UnityEngine.ARModule.dll
UnityEngine.AssetBundleModule.dll
UnityEngine.AudioModule.dll
UnityEngine.ClothModule.dll
UnityEngine.ClusterInputModule.dll
UnityEngine.ClusterRendererModule.dll
UnityEngine.CoreModule.dll
UnityEngine.CrashReportingModule.dll
UnityEngine.DirectorModule.dll
UnityEngine.dll
UnityEngine.DSPGraphModule.dll
UnityEngine.GameCenterModule.dll
UnityEngine.GridModule.dll
UnityEngine.HotReloadModule.dll
UnityEngine.ImageConversionModule.dll
UnityEngine.IMGUIModule.dll
UnityEngine.InputLegacyModule.dll
UnityEngine.InputModule.dll
UnityEngine.JSONSerializeModule.dll
UnityEngine.LocalizationModule.dll
UnityEngine.ParticleSystemModule.dll
UnityEngine.PerformanceReportingModule.dll
UnityEngine.Physics2DModule.dll
UnityEngine.PhysicsModule.dll
UnityEngine.ProfilerModule.dll
UnityEngine.Purchasing.dll
UnityEngine.ScreenCaptureModule.dll
UnityEngine.SharedInternalsModule.dll
UnityEngine.SpriteMaskModule.dll
UnityEngine.SpriteShapeModule.dll
UnityEngine.StreamingModule.dll
UnityEngine.SubstanceModule.dll
UnityEngine.SubsystemsModule.dll
UnityEngine.TerrainModule.dll
UnityEngine.TerrainPhysicsModule.dll
UnityEngine.TextCoreModule.dll
UnityEngine.TextRenderingModule.dll
UnityEngine.TilemapModule.dll
UnityEngine.TLSModule.dll
UnityEngine.UI.dll
UnityEngine.UIElementsModule.dll
UnityEngine.UIElementsNativeModule.dll
UnityEngine.UIModule.dll
UnityEngine.UmbraModule.dll
UnityEngine.UNETModule.dll
UnityEngine.UnityAnalyticsModule.dll
UnityEngine.UnityConnectModule.dll
UnityEngine.UnityTestProtocolModule.dll
UnityEngine.UnityWebRequestAssetBundleModule.dll
UnityEngine.UnityWebRequestAudioModule.dll
UnityEngine.UnityWebRequestModule.dll
UnityEngine.UnityWebRequestTextureModule.dll
UnityEngine.UnityWebRequestWWWModule.dll
UnityEngine.VehiclesModule.dll
UnityEngine.VFXModule.dll
UnityEngine.VideoModule.dll
UnityEngine.VirtualTexturingModule.dll
UnityEngine.VRModule.dll
UnityEngine.WindModule.dll
UnityEngine.XRModule.dll
Unsafe.dll
VisualDesignCafe.Nature.dll
VisualDesignCafe.Rendering.dll
VisualDesignCafe.Rendering.Nature.dll
VRoidSDK.dll
WanzyeeStudio.Runtime.dll
Whinarn.UnityMeshSimplifier.Runtime.dll




