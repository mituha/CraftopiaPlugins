MultiCameraPlugin
===============================

別アングルでのキャラクター表示等、PinP的な複数カメラ表示を行うためのプラグインです。  

なお、どちらかというと技術的なサンプルとしての役割を想定しています。  
ソースを参照の上、適切に各自のMODで活用してください。  

その他、色々は [https://github.com/mituha/CraftopiaPlugins](https://github.com/mituha/CraftopiaPlugins) を参照してください。

## 導入方法
//TODO	他のMODと同じ感じで導入してください


## 使用方法
チャット欄(Enterで起動)で後述のコマンドを入力します。  

※コマンドは今後変わります。
現状、実験的に試していたのをそのまま公開しています。  

コマンドとして、`/camera3` のように、1-3までの番号をつけたコマンドを取ることでカメラを区別します。  
なお、以下では、`/camera[1-9]` のように表記しています。  
使用時は、`/camera3` のように置き換えてください。

`/camera[1-9] treasure (n)` のように、`()` カッコでくくられたパラメーターは省略可能です。
`/camera2 treasure` のように未指定、もしくは、`/camera2 treasure 2` のようにあパラメーターを追加できます。



### 使用例
```
# プレイヤーの顔を表示するカメラの追加
# カメラ３は右側表示
/camera3 player
# 距離の変更
/camera3 distance 2.0

# プレイヤーの尻を表示するカメラの追加
# カメラ２は上側表示
/camera2 hips
# アングルの変更
/camera2 angle 0.5

# 近くの宝箱表示
# カメラ１は左側表示
/camera1 treasure

```

### コマンド
#### 作成系
新規にカメラ表示を追加します。

* `/camera[1-9] player`
	+ 正面からの顔表示
	+ //TODO 名称変更
* `/camera[1-9] player muki`
	+ 指定方向からの顔表示
	+ //TODO 名称変更
	+ muki
		+ forward
		+ back
		+ up
		+ down
		+ right
		+ left
* `/camera[1-9] hips`
	+ 後ろからの尻表示
	+ その他、ボーン名に準拠して各部の表示

* `/camera[1-9] treasure (pin) (all) (n)`
	+ 近くの宝箱表示
	+ pin : "pin" 時、ピン表示
	+ all : "all" 時、開いている宝箱も表示
 	+ n : 1-
* `/camera[1-9] fish (pin) (all) (n)`
	+ 近くの釣り場表示
	+ pin : "pin" 時、ピン表示
	+ all : "all" 時、すべて表示
	+ n : 1-
* `/camera[1-9] fragment (pin) (n)`
	+ 近くの世界遺産の断片表示
	+ pin : "pin" 時、ピン表示
	+ all : "all" 時、すべて表示
	+ n : 1-
* `/camera[1-9] door (pin) (n)`
	+ 近くのキルカウントする扉表示
	+ pin : "pin" 時、ピン表示
	+ all : "all" 時、すべて表示
	+ n : 1-
* `/camera[1-9] pickup (pin) (n)`
	+ 近くの？表示
	+ pin : "pin" 時、ピン表示
	+ all : "all" 時、すべて表示
	+ n : 1-



//TODO

* `/camera[1-9] off`
	+ カメラ表示を消します
	+ //TODO 実際にカメラを削除する方法
* `/camera[1-9] on`
	+ `off` で消したカメラ表示を行います

* `/camera[1-9] mask bits`
	+ カメラに表示するレイヤーを指定します
	+ bits : 65536や0x4000等 ビットで32種類のレイヤーを指定できます
	+ bits引数なしですべて表示


//TODO カメラ自体の削除

#### 表示パラメーター調整系
* `/camera[1-9] distance kyori`
	+ カメラの距離を変更します
	+ 0.0 < kyori
		+ 指定分離した距離にカメラを配置します
		+ 現状、プレイヤー追随用のカメラで有効
* `/camera[1-9] angle rate`
	+ カメラの高さを割合で変更します
	+ 0.0 < kyori
		+ デフォルトの高さに対して指定した割合の高さにカメラを配置します
		+ 1.0より小さい場合はローアングル
		+ 現状、プレイヤー追随用のカメラで有効
* `/camera[1-9] position muki`
	+ 指定方向からのプレイヤー表示
	+ muki
		+ forward
		+ back
		+ up
		+ down
		+ right
		+ left

#### 配置調整系
作成済みのカメラ表示の位置等を調整します。

* `/camera[1-9] size width height`
	+ 表示サイズを変更します
	+ 0.0 < width < 1.0
		+ 画面全体を 1.0 とした表示幅比率
	+ 0.0 < height < 1.0
		+ 画面全体を 1.0 とした表示高さ比率
* `/camera[1-9] position x y
	+ 表示位置を変更します`
	+ 0.0 <= x < 1.0
		+ 画面左下を 0.0 とする、0 - 1 の位置
	+ 0.0 <= y < 1.0
		+ 画面左下を 0.0 とする、0 - 1 の位置
* `/camera[1-9] reset`
	+ 初期配置位置へ移動します
* `/camera[1-9] reset number`
	+ 指定の初期配置位置へ移動します
	+ 1<= number <=9
		+ 指定番号のカメラの初期配置

#### TODO
* `/camera main`
	+ (開発者向け)カメラ情報出力
* `/camera list`
	+ (開発者向け)カメラ情報出力
* `/camera[1-9] add`
	+ (テスト)何か表示追加



### 補足
カメラ表示のViewの座標系は、左下を(0,0)とする、(0,0) - (1,1) の座標系です。  

