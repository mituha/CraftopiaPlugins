Craftpia#
=============================

クラフトピア内でC#によるスクリプト実行を行うためのプラグインです。  

また、このプロジェクトは、  

* クラフトピア内でのC#によるスクリプト実行
* 他のMOD作成補助用のクラスの提供

の２種類を想定しています。  

その他、色々は [https://github.com/mituha/CraftopiaPlugins](https://github.com/mituha/CraftopiaPlugins) を参照してください。

## 導入方法
//TODO	他のMODと同じ感じで導入してください
フォルダーごと `Craftopia\BepInEx\plugins` に配置

## 使用方法
チャット欄(Enterで起動)で `#cs` とコマンドを入力します。  
スクリプト入力用のUIが表示されます。  

スクリプト欄で、`#exit` とコマンドを入力することで終了します。  
`#load "test.cs"` 等でファイルから読み込んで実行します。

それ以外は C# のコードが動作します。  
[Craftopia.cs](https://github.com/mituha/CraftopiaPlugins/blob/main/CraftopiaSharp/CraftopiaSharp.cs) に実装されているメソッドは直接呼び出せます。


### 制限
複数行や継続行での入力はできません。  
１行で完結するコードを入力してください。  
UI的には Shift+Enterで改行か、キー入力の取得が可能なら複数行対応しても良いかも。  
VB的な継続行(` _`)を導入する？  

コマンドヒストリー的な機能は有りません。  
そもそも、UI的にキーイベントがとれない。  


