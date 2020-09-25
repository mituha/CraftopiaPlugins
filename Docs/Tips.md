Tips
==================

小ネタ的なメモ

## バージョン
```
[Info   : Unity Log] ApplicationVersion = 20200924.0334
[Info   : Unity Log] VersionInt=9240334
```
`Application.Version` は `string` で `20200924.0334` のような値。  
`OcAllSceneShareSingleton.Inst.Version` は `int` で `9240334` のような値





## 列挙型の `Max`  
```
public enum PoolType : byte
{
    PlDrop,
    KillDrop,
    FieldCollect,
    Bldg,
    Gacha,
    STOOL,
    Automation,
    Max
}
```

上記のように列挙型の最後が `Max` となっている場合、配列確保のため等に使用するために定義されています。  

```
private OcDropItemPool[] _DropItemPool = new OcDropItemPool[7];
```
近くに上記のような配列定義がある場合、
```
private OcDropItemPool[] _DropItemPool = new OcDropItemPool[(int)PoolType.Max)];
```
元は、このように列挙型を用いて定義されていたと考えられます。  
なので、これらの配列にアクセスする場合は対応する列挙型を使用するほうが良いです。  



