OcGimmick 派生クラス
============================

下記表は[TestUtility](../TestUtility/README.md)を使用して抽出しています。


| 名前空間 | クラス     | 基本クラス   |       |  
|----------|------------|--------------|-------|  
| Oc | `OcGimmick`  | `MonoBehaviour`    | abstract  |  
| Oc | `OcGimmick_FishingPoint`  |     |   |  
| Oc | `OcGimmick_TreasureBox`  | `OcGimmick_TreasureBoxBase`    |   |  
| Oc | `OcGimmick_TreasureBoxBase`  | `OcGimmick_Pickup`    | abstract  |  
| Oc | `OcGimmick_WorldHeritageFragment`  | `OcGimmick_TreasureBoxBase`    |   |  
| Oc | `OcGimmick_DoorEmKillCount`  | `OcGimmick_Pickup`    |   |  
| Oc | `OcGimmick_Pickup`  |     |   |  

```mermaid
classDiagram

	class OcGimmick{
	}


	OcGimmick <|-- OcGimmick_FishingPoint
	class OcGimmick_FishingPoint{
	}


	OcGimmick_TreasureBoxBase <|-- OcGimmick_TreasureBox
	class OcGimmick_TreasureBox{
	}


	OcGimmick_Pickup <|-- OcGimmick_TreasureBoxBase
	class OcGimmick_TreasureBoxBase{
	}


	OcGimmick_TreasureBoxBase <|-- OcGimmick_WorldHeritageFragment
	class OcGimmick_WorldHeritageFragment{
	}


	OcGimmick_Pickup <|-- OcGimmick_DoorEmKillCount
	class OcGimmick_DoorEmKillCount{
	}


	OcGimmick <|-- OcGimmick_Pickup
	class OcGimmick_Pickup{
	}

```
