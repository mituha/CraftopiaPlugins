Layer
=======================

`gameObject.layer` について。  
レイヤーはUnityでカメラの描画等をグループ化するために用いられます。
プレイヤーに固有のLayerが割り当てられているなら、プレイヤーのみ表示とかが可能となる。

プレイヤーは 14(0x4000,16384)  
っぽいけど、プレイヤーのみ表示は上手くいっていない


```
this._InWater = (other.gameObject.layer == 4);
```
水が 4(0x0010)

OcAttach_Absorber が 20  
OcDropItem は 16?

OcDamageのcolCheckで色々
```
bool flag = base.gameObject.layer == 9 && other.gameObject.layer == 13;
bool flag2 = base.gameObject.layer == 9 && other.gameObject.layer == 12;
bool flag3 = base.gameObject.layer == 10 && other.gameObject.layer == 12;
bool flag4 = base.gameObject.layer == 22 && other.gameObject.layer == 12;
bool flag5 = base.gameObject.layer == 9 && other.gameObject.layer == 11;
bool flag6 = base.gameObject.layer == 10 && other.gameObject.layer == 11;
bool flag7 = base.gameObject.layer == 22 && other.gameObject.layer == 11;
bool flag8 = false;
if (base.gameObject.layer == 22 && other.gameObject.layer == 13 && this._Health.IsInstallObj)
{
    flag8 = true;
}
```
