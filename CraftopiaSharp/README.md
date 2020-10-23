Craftpia#
=============================


とりあえず、ユーティリティ系をまとめています。
このソリューション内はファイルをリンクして参照。

.NET Framework のバージョンは、4.6.1 以上が必要なので、4.8 としました。  
.NET Standard 2.0 のサポートは 4.6.1 から。  

Microsoft.CodeAnalysis.CSharp.Scripting は 2.6.1 を使用しています。  
Unity に パッケージマネージャーで `com.unity.immediate-window` を追加した場合、2.6.1 が使用されている。  

com.unity.immediate-window
https://github.com/Unity-Technologies/com.unity.immediate-window
https://docs.unity3d.com/ja/2019.4/Manual/com.unity.immediate-window.html

com.unity.code-analysis
https://docs.unity3d.com/Packages/com.unity.code-analysis@0.1/manual/index.html

https://forum.unity.com/threads/solved-can-i-use-microsoft-codeanalysis-csharp-for-editor-script.681283/

なお、com.unity.immediate-window で Microsoft.CodeAnalysis.CSharp.Scripting の出力が確認されたため、
com.unity.code-analysis 等は未検証。  
また、同一のDLLを使用しているため、NuGet参照の 2.6.1 を使用しています。  
-> 色々エラーが出たりもしたため、とりあえず、最新の
```
  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Scripting" Version="3.8.0-5.final" />
  </ItemGroup>
```
に現状はしています。

## 必要ファイル
`...\steamapps\common\Craftopia\BepInEx\plugins\CraftpiaSharp` に格納しているファイル
```
Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----       2020/10/24      8:07          16384 CraftopiaSharp.dll
-a----       2020/10/15      8:18        5996408 Microsoft.CodeAnalysis.CSharp.dll
-a----       2020/10/15      8:20          31096 Microsoft.CodeAnalysis.CSharp.Scripting.dll
-a----       2020/10/15      8:16        2802552 Microsoft.CodeAnalysis.dll
-a----       2020/10/15      8:18         136072 Microsoft.CodeAnalysis.Scripting.dll
-a----       2020/08/07     17:49         189320 System.Collections.Immutable.dll
-a----       2020/02/19     10:05         141184 System.Memory.dll
-a----       2020/08/07     17:59         463240 System.Reflection.Metadata.dll
-a----       2020/02/21      3:58          17000 System.Runtime.CompilerServices.Unsafe.dll
```
NuGetの参照に関係しそうなDLLをコピー。  
エラーのたびに追加します

* System.Numerics.Vectors.dll
    + for文で要求された


## TODO
とりあえず、技術デモ用にチャットウィンドウを仮に使用。  
独自のターミナルウィンドウを作成したいところ。  
