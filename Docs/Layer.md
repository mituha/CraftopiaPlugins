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
| Layer | Mask | 名前 |
|----|-----|--------|  
| 0 | 0x1 | Default |  
| 1 | 0x2 | TransparentFX |  
| 2 | 0x4 | Ignore Raycast |  
| 3 | 0x8 |  |  
| 4 | 0x10 | Water |  
| 5 | 0x20 | UI |  
| 6 | 0x40 |  |  
| 7 | 0x80 |  |  
| 8 | 0x100 | Terrain |  
| 9 | 0x200 | DamagePl |  
| 10 | 0x400 | DamageEm |  
| 11 | 0x800 | AttackAll |  
| 12 | 0x1000 | AttackPl |  
| 13 | 0x2000 | AttackEm |  
| 14 | 0x4000 | Pl |  
| 15 | 0x8000 | Em |  
| 16 | 0x10000 | Item |  
| 17 | 0x20000 | _Dummy_OnMap |  
| 18 | 0x40000 | Platform_OneWay |  
| 19 | 0x80000 | Prop |  
| 20 | 0x100000 | Event |  
| 21 | 0x200000 | Climb |  
| 22 | 0x400000 | DamageObj |  
| 23 | 0x800000 | Pickup |  
| 24 | 0x1000000 | CheckOtherBldg |  
| 25 | 0x2000000 | InstallObj |  
| 26 | 0x4000000 | AdultWall |  
| 27 | 0x8000000 | _dummy27 |  
| 28 | 0x10000000 | _dummy28 |  
| 29 | 0x20000000 | Postprocessing |  
| 30 | 0x40000000 | Effect |  
| 31 | 0x80000000 | WaterChecker |  