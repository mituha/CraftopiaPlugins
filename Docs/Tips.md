Tips
==================

小ネタ的なメモ

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



