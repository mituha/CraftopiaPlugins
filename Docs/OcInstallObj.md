OcInstallObj 派生
=========================

下記表は[TestUtility](../TestUtility/README.md)を使用して抽出しています。



| 名前空間 | クラス     | 基本クラス   |       |  
|----------|------------|--------------|-------|  
| Oc | OcBldg_Aging  | OcBuildingBase_Craft    |   |  
| Oc | OcBldg_AltarOfAge  | OcBuildingBase    |   |  
| Oc | OcBldg_AltarOfWorld  | OcBuildingBase    |   |  
| Oc | OcBldg_Automation  | OcBldg_CraftingBench    |   |  
| Oc | OcBldg_BlackSmith  | OcBuildingBase_Craft    |   |  
| Oc | OcBldg_BreedPlant  | OcBuildingBase_Craft    |   |  
| Oc | OcBldg_Breeding  |     |   |  
| Oc | OcBldg_Campfire  | OcBuildingBase_Craft    |   |  
| Oc | OcBldg_Chest  | OcBuildingBase    |   |  
| Oc | OcBldg_CraftingBench  | OcBuildingBase_Craft    |   |  
| Oc | OcBldg_Dryer  | OcBuildingBase_Craft    |   |  
| Oc | OcBldg_EnchantTable  | OcBuildingBase_Craft    |   |  
| Oc | OcBldg_Farm  | OcBldg_FarmBase    |   |  
| Oc | OcBldg_FarmBase  | OcBuildingBase_Craft    | abstract  |  
| Oc | OcBldg_FilletMachine  | OcBuildingBase_Craft    |   |  
| Oc | OcBldg_Gacha  | OcBuildingBase    |   |  
| Oc | OcBldg_ItemGenerator  | OcBuildingBase_Craft    |   |  
| Oc | OcBldg_ItemGenerator_CookingPot  | OcBldg_ItemGenerator    |   |  
| Oc | OcBldg_Market  | OcBuildingBase    |   |  
| Oc | OcBldg_SeedMakingMachine  | OcBuildingBase_Craft    |   |  
| Oc | OcBldg_Signboard  | OcBuildingBase    |   |  
| Oc | OcBldg_WheatField  | OcBldg_FarmBase    |   |  
| Oc | OcBldg_WorldHeritage  | OcBuildingBase    |   |  
| Oc | OcBuildingBase  |     | abstract  |  
| Oc | OcBuildingBase_Craft  | OcBuildingBase    | abstract  |  
| Oc | OcInstallObj_Sprinkler  |     |   |  
| Oc | OcInstallObj_SummonNPC  |     |   |  
| Oc | OcInstallObj  | MonoBehaviour    | abstract  |  
| Oc | OcInstallObj_DroneBase  |     |   |  
| Oc | OcInstallObj_Miner  |     |   |  

```mermaid
classDiagram

	OcBuildingBase_Craft <|-- OcBldg_Aging
	class OcBldg_Aging{
	}


	OcBuildingBase <|-- OcBldg_AltarOfAge
	class OcBldg_AltarOfAge{
	}


	OcBuildingBase <|-- OcBldg_AltarOfWorld
	class OcBldg_AltarOfWorld{
	}


	OcBldg_CraftingBench <|-- OcBldg_Automation
	class OcBldg_Automation{
	}


	OcBuildingBase_Craft <|-- OcBldg_BlackSmith
	class OcBldg_BlackSmith{
	}


	OcBuildingBase_Craft <|-- OcBldg_BreedPlant
	class OcBldg_BreedPlant{
	}


	OcInstallObj <|-- OcBldg_Breeding
	class OcBldg_Breeding{
	}


	OcBuildingBase_Craft <|-- OcBldg_Campfire
	class OcBldg_Campfire{
	}


	OcBuildingBase <|-- OcBldg_Chest
	class OcBldg_Chest{
	}


	OcBuildingBase_Craft <|-- OcBldg_CraftingBench
	class OcBldg_CraftingBench{
	}


	OcBuildingBase_Craft <|-- OcBldg_Dryer
	class OcBldg_Dryer{
	}


	OcBuildingBase_Craft <|-- OcBldg_EnchantTable
	class OcBldg_EnchantTable{
	}


	OcBldg_FarmBase <|-- OcBldg_Farm
	class OcBldg_Farm{
	}


	OcBuildingBase_Craft <|-- OcBldg_FarmBase
	class OcBldg_FarmBase{
	}


	OcBuildingBase_Craft <|-- OcBldg_FilletMachine
	class OcBldg_FilletMachine{
	}


	OcBuildingBase <|-- OcBldg_Gacha
	class OcBldg_Gacha{
	}


	OcBuildingBase_Craft <|-- OcBldg_ItemGenerator
	class OcBldg_ItemGenerator{
	}


	OcBldg_ItemGenerator <|-- OcBldg_ItemGenerator_CookingPot
	class OcBldg_ItemGenerator_CookingPot{
	}


	OcBuildingBase <|-- OcBldg_Market
	class OcBldg_Market{
	}


	OcBuildingBase_Craft <|-- OcBldg_SeedMakingMachine
	class OcBldg_SeedMakingMachine{
	}


	OcBuildingBase <|-- OcBldg_Signboard
	class OcBldg_Signboard{
	}


	OcBldg_FarmBase <|-- OcBldg_WheatField
	class OcBldg_WheatField{
	}


	OcBuildingBase <|-- OcBldg_WorldHeritage
	class OcBldg_WorldHeritage{
	}


	OcInstallObj <|-- OcBuildingBase
	class OcBuildingBase{
	}


	OcBuildingBase <|-- OcBuildingBase_Craft
	class OcBuildingBase_Craft{
	}


	OcInstallObj <|-- OcInstallObj_Sprinkler
	class OcInstallObj_Sprinkler{
	}


	OcInstallObj <|-- OcInstallObj_SummonNPC
	class OcInstallObj_SummonNPC{
	}


	class OcInstallObj{
	}


	OcInstallObj <|-- OcInstallObj_DroneBase
	class OcInstallObj_DroneBase{
	}


	OcInstallObj <|-- OcInstallObj_Miner
	class OcInstallObj_Miner{
	}

```

