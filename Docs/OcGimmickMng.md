OcGimmickMng
===========================

釣り場や宝箱等の [OcGimmick](OcGimmick.md) の管理

## 構造
```
oc_pf_TreasureBox--RandEpic
 -(oc_pf_TreasureBox--RandEpic:Transform)
    TreasureBox_Core
        -(TreasureBox_Core:Transform)
        -(TreasureBox_Core:OcGimmick_TreasureBox)
```
島等のシーンには上記のようにオブジェクトが配置されており、コンポーネントとして `OcGimmick_TreasureBox` が設定されています。  
`OcGimmick.Start` で、`OcGimmickMng` に登録されている。  
つまり、シーンでの配置優先で管理。  


