ConsoleCommandsPlugin
=============================

チャットを使って、コマンド実行する雛形。

## 使用方法
ConsoleCommandPluginの派生クラスを作成することで、コマンド入力に対する処理を行うプラグインが実装できます。
`ConsoleCommandPlugin.cs` を自分のプロジェクトにコピーして使用してください。  

実装例は SampleConsoleCommandPlugin を参照してください。

## コマンド例
* `/echo message` 
	+ メッセージをそのまま返す
* `/now`
	+ 日時を返す
* `/log on`
* `/scene`
* `/list item`
* `/list scene`
* `/unity sphere`