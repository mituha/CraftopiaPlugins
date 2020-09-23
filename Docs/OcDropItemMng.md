OcDropItemMng
========================

落とし物管理クラス。

| PoolType      | 保持可能数 |
|---------------|----------:|
| PlDrop        |  16       |
| KillDrop      | 256       |
| FieldCollect  | 64        |
| Bldg          | 64        |
| Gacha         | 64        |
| STOOL         | 32        |
| Automation    | 128       |


保持可能数は `MAX_DROP_ITEM_NUM` で定義されている。  
//TODO 配列数未指定になっているがコンパイルで消えるのか確認
//      消えないなら拡張時に注意がいる書き方
