using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestPlugin
{
    /// <summary>
    /// シーン関連の確認用
    /// </summary>
    [BepInPlugin("me.mituha.craftopia.plugins.scenecheckplugin", "SceneCheck Plug-In", "1.0.0.0")]
    public class SceneCheckPlugin : BaseUnityPlugin
    {

        void Awake() {
            UnityEngine.Debug.Log($"{this.GetType().Name} Awake {SceneManager.GetActiveScene().name}");
        }

        void Start() {
            //Sceneは構造体
            UnityEngine.Debug.Log($"{this.GetType().Name} Start {SceneManager.GetActiveScene().name}");

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene arg0, Scene arg1) {
            UnityEngine.Debug.Log($"{this.GetType().Name} OnActiveSceneChanged {arg0.name} {arg1.name}");
        }

        private float _CheckTime = 0;

        /// <summary>
        /// 
        /// </summary>
        void Update() {
            //MonoBehaviour派生で単純にUnityのUpdate動作
            //  なので、きっとなんでもできる
            _CheckTime += Time.deltaTime;
            if (_CheckTime >= 5) {
                UnityEngine.Debug.Log($"{this.GetType().Name} Update {Time.deltaTime}/{_CheckTime}");
                _CheckTime = 0;
            }
        }
    }
}
