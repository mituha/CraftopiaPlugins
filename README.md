クラフトピア用プラグイン
=============================

現状、手探りかつ練習用です。  

## ビルド
開発環境は Visual Studio 2019  

BepInExを利用しています。  
導入方法とかは、https://github.com/BepInEx/BepInEx 参照  

参照用のDLLはLibフォルダーにコピーしてください。  

プロジェクトの作成は Core 用で新規作成して、TargetFramework を書き換えています。


## プロジェクト
### ConsoleCommandPlugin
コンソールコマンドの雛形

### HellowWorldPlugin
Hello world!

### TestPlugin
適当に実験用


## プラグインの取っ掛かり
### シングルトンな管理クラスっぽいやつ
SingletonMonoBehaviour<T>派生で抽出

* OcUI_KeybindIconHandler
* OcEmMng
* OcNetMng
* OcGimmickMng
* OcStaticObjMng
* OcBeaconMng
* OcCharaData
* OcDayMng
* OcDevMng
* OcDisasterMng
* OcDropItemMng
* OcEfcMng
* OcGameMng
* OcInstallObjMng
* OcMapMarkerMng
* OcSaveManager
* OcSpawnerMng
* OcPlCam
* OcPlCharacterManager
* OcPlMng
* OcUICam
* OcUI_SettingMenu
* OcUI_HomeScene
* OcCurtainCtrl
* OcGamepadUIMng
* OcResidentData
* OcUI_BeaconHandler
* OcUI_CenterTopNotificationHandler
* OcUI_ChatHandler
* OcUI_Damage
* OcUI_DialogBox
* OcUI_ErrorNotification
* OcUI_EscScene
* OcUI_HudTrackerManager
* OcUI_ItemPickupTickerHandler
* OcUI_MapHandler
* OcUI_MemberHealthHandler
* OcUI_PlayerCraftProcess
* OcUI_PlayerJoinNotification
* OcUI_RideActionHandler
* OcUI_SlaveHeaderHandler
* OcUI_SpeechBubbleHandler
* OcUI_Stamina
* OcUI_TrackerTargetHolder
* OcGameSceneTransformManager
* OcSteamworkHander
* OcAutoMoveAndRotateMng
* OcSkillManager
* OcUI_NewSkillTree
* OcUI_SkillTree
* StatisticalDataManager
* OcMissionManager
* OcUI_HUDMissionClearHandler
* OcUI_HUDMissionSheetHandler
* OcUI_MissionMng
* MapPieceCapture
* OcWorldManager
* OcItemDataMng
* OcUI_MenuHandler
* OcUI_ShopHandler
* OcUI_StatusUpHandler
* OcItemUI_AutoCraftMng
* OcItemUI_ChestMng
* OcItemUI_CraftMng
* OcItemUI_EquipmentMng
* OcItemUI_FooterMng
* OcItemUI_HotkeyMng
* OcItemUI_InventoryBoxMng
* OcItemUI_InventoryMng
* OcItemUI_MarketMng
* OcItemUI_QuickAccessMng
* OcItemUI_SelectMng
* OcItem_SlotSelectFinder
* OcDungeonManager
* OcAchievementManager
* OcInputTrack

