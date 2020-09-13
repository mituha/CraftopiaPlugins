テスト用
===========================

都度、実験的な内容を追加。
実用的なのがあったら別プロジェクトに切り出していきます。

## メモ
* １つのプロジェクト(Assembly)に複数 Plugin 用クラスをおいても問題ない。
	+ BepInPlugin属性のGUIDは変更する必要があると思われる。  

## TestPlugin、TestPlugin2
都度、テスト的に使用。

* `Assembly-CSharp.dll` に含まれるクラス等の検索


## GetItemListPlugin
https://w.atwiki.jp/craftopiamodder/pages/11.html 参照。
起動時にアイテムの定義一覧を取得

## SceneCheckPlugin
UnityのScene関連の動作確認

* OcScene_Home
	+ 最初のタイトル画面
* OcScene_Setting
	+ Tipsが表示されるロード画面
* OcScene_DevTest_yohei_Tutorial_0728_MS
	+ 初期島
* OcScene_DevTest_yohei_Island15_0823_MS
	+ 別な島

関連しそうなのは OcAllSceneShareSingleton クラス。  
だが、シーンぽいのは `OcScene_Home` しか見当たらない。  
