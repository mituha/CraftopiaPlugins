SingletonMonoBehaviour<T>派生クラス
=======================================

`SingletonMonoBehaviour<T>` 派生クラスは、シングルトンとなるゲーム中でインスタンスが1つのみとなるクラスです。  
インスタンスの取得は、`OcGameMng.Inst` 等から取得できます。  

実際には、最初に作成されたインスタンスがstaticなInstに自身を設定することでシングルトンとなっています。  

使用する分には `Inst` を null チェック後使用可能です。  
例えば、`OcUI_ChatHandler` 等は、ゲームに入ってからのUIとなる、`OcScene_PlMasterUI` シーンでインスタンスが作成されているため、タイトル画面ではnull参照を返します。  

特に、XxxMgrとついているクラスは何らかの管理クラスです。  
実際に追いかけるのであれば、`OcGameMng` から確認するのが良いです。  


| 名前空間 | クラス     |       |  
|----------|------------|-------|  
| Oc |         |           |  
| Oc | OcUI_KeybindIconHandler |    | 
| Oc | OcEmMng |    | 
| Oc | OcNetMng |    | 
| Oc | OcGimmickMng |    | 
| Oc | OcStaticObjMng |    | 
| Oc | OcBeaconMng |    | 
| Oc | OcCharaData |    | 
| Oc | OcDayMng |    | 
| Oc | OcDevMng |    | 
| Oc | OcDisasterMng |    | 
| Oc | OcDropItemMng |    | 
| Oc | OcEfcMng |    | 
| Oc | OcGameMng |    | 
| Oc | OcInstallObjMng |    | 
| Oc | OcMapMarkerMng |    | 
| Oc | OcSaveManager |    | 
| Oc | OcSpawnerMng |    | 
| Oc | OcPlCam |    | 
| Oc | OcPlCharacterManager |    | 
| Oc | OcPlMng |    | 
| Oc | OcUICam |    | 
| Oc | OcUI_SettingMenu |    | 
| Oc | OcUI_HomeScene |    | 
| Oc | OcCurtainCtrl |    | 
| Oc | OcGamepadUIMng |    | 
| Oc | OcResidentData |    | 
| Oc | OcUI_BeaconHandler |    | 
| Oc | OcUI_CenterTopNotificationHandler |    | 
| Oc | OcUI_ChatHandler |    | 
| Oc | OcUI_Damage |    | 
| Oc | OcUI_DialogBox |    | 
| Oc | OcUI_ErrorNotification |    | 
| Oc | OcUI_EscScene |    | 
| Oc | OcUI_HudTrackerManager |    | 
| Oc | OcUI_ItemPickupTickerHandler |    | 
| Oc | OcUI_MapHandler |    | 
| Oc | OcUI_MemberHealthHandler |    | 
| Oc | OcUI_PlayerCraftProcess |    | 
| Oc | OcUI_PlayerJoinNotification |    | 
| Oc | OcUI_RideActionHandler |    | 
| Oc | OcUI_SlaveHeaderHandler |    | 
| Oc | OcUI_SpeechBubbleHandler |    | 
| Oc | OcUI_Stamina |    | 
| Oc | OcUI_TrackerTargetHolder |    | 
| Oc | OcGameSceneTransformManager |    | 
| Oc | OcSteamworkHander |    | 
| Oc | OcAutoMoveAndRotateMng |    | 
| Oc.Skills |         |           |  
| Oc.Skills | OcSkillManager |    | 
| Oc.Skills | OcUI_NewSkillTree |    | 
| Oc.Skills | OcUI_SkillTree |    | 
| Oc.Statistical |         |           |  
| Oc.Statistical | StatisticalDataManager |    | 
| Oc.Missions |         |           |  
| Oc.Missions | OcMissionManager |    | 
| Oc.Missions | OcUI_HUDMissionClearHandler |    | 
| Oc.Missions | OcUI_HUDMissionSheetHandler |    | 
| Oc.Missions | OcUI_MissionMng |    | 
| Oc.Maps |         |           |  
| Oc.Maps | MapPieceCapture |    | 
| Oc.Maps | OcWorldManager |    | 
| Oc.Item |         |           |  
| Oc.Item | OcItemDataMng |    | 
| Oc.Item.UI |         |           |  
| Oc.Item.UI | OcUI_MenuHandler |    | 
| Oc.Item.UI | OcUI_ShopHandler |    | 
| Oc.Item.UI | OcUI_StatusUpHandler |    | 
| Oc.Item.UI | OcItemUI_AutoCraftMng |    | 
| Oc.Item.UI | OcItemUI_ChestMng |    | 
| Oc.Item.UI | OcItemUI_CraftMng |    | 
| Oc.Item.UI | OcItemUI_EquipmentMng |    | 
| Oc.Item.UI | OcItemUI_FooterMng |    | 
| Oc.Item.UI | OcItemUI_HotkeyMng |    | 
| Oc.Item.UI | OcItemUI_InventoryBoxMng |    | 
| Oc.Item.UI | OcItemUI_InventoryMng |    | 
| Oc.Item.UI | OcItemUI_MarketMng |    | 
| Oc.Item.UI | OcItemUI_QuickAccessMng |    | 
| Oc.Item.UI | OcItemUI_SelectMng |    | 
| Oc.Item.UI | OcItem_SlotSelectFinder |    | 
| Oc.Dungeon |         |           |  
| Oc.Dungeon | OcDungeonManager |    | 
| Oc.Achievements |         |           |  
| Oc.Achievements | OcAchievementManager |    | 
| Oc.OcInput |         |           |  
| Oc.OcInput | OcInputTrack |    | 
