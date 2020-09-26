using Oc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    internal class PlayerCamera : MonoBehaviour
    {
        public GameObject player;

        void Start() {
            if (this.player == null) {
                //TODO 顔の位置を取得できると良い？
                this.player = OcPlMng.Inst.getPl(0).gameObject;
            }
        }

        // Update is called once per frame
        void Update() {
            //とりあえず、適当にプレーヤーの正面からの表示
            this.transform.position = this.player.transform.position + this.player.transform.forward * 0.5f + this.player.transform.up * 1.5f;
            this.transform.LookAt(this.player.transform.position + this.player.transform.up * 1.5f);
        }
    }
}
