OcEm_NPC_Base
===========================

[OcEm](OcEm.md)派生。
大本は、[OcCharacter](OcCharacter.md)  

`OcEmType` に列挙型も用意されている

下記表は[TestUtility](../TestUtility/README.md)を使用して抽出しています。


| 名前空間 | クラス     | 基本クラス   |       |  
|----------|------------|--------------|-------|  
| Oc.Em | `OcEm_NPC_Bandit`  |     |   |  
| Oc.Em | `OcEm_NPC_Base`  | `OcEm`    | abstract  |  
| Oc.Em | `OcEm_NPC_ChestBase`  |     | abstract  |  
| Oc.Em | `OcEm_NPC_Event`  | `OcEm_NPC_ChestBase`    |   |  
| Oc.Em | `OcEm_NPC_Merchant`  | `OcEm_NPC_ChestBase`    |   |  
| Oc.Em | `OcEm_NPC_Mob`  |     |   |  
| Oc.Em | `OcEm_NPC_Mob_Anubis`  | `OcEm_NPC_Mob`    |   |  
| Oc.Em | `OcEm_NPC_Reception`  |     |   |  

```mermaid
classDiagram

	OcEm_NPC_Base <|-- OcEm_NPC_Bandit
	class OcEm_NPC_Bandit{
	}


	class OcEm_NPC_Base{
	}


	OcEm_NPC_Base <|-- OcEm_NPC_ChestBase
	class OcEm_NPC_ChestBase{
	}


	OcEm_NPC_ChestBase <|-- OcEm_NPC_Event
	class OcEm_NPC_Event{
	}


	OcEm_NPC_ChestBase <|-- OcEm_NPC_Merchant
	class OcEm_NPC_Merchant{
	}


	OcEm_NPC_Base <|-- OcEm_NPC_Mob
	class OcEm_NPC_Mob{
	}


	OcEm_NPC_Mob <|-- OcEm_NPC_Mob_Anubis
	class OcEm_NPC_Mob_Anubis{
	}


	OcEm_NPC_Base <|-- OcEm_NPC_Reception
	class OcEm_NPC_Reception{
	}

```
