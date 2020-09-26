using Oc;
using SR;
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
    internal class LookAtPlayerCamera : MonoBehaviour
    {
        public GameObject player;

        public HumanBodyBones? Target = HumanBodyBones.Head;    // HumanBodyBones.LeftEye;  //目は未定義
        public HumanBodyBones? Target2 = null;   // HumanBodyBones.RightEye;

        /// <summary>
        /// カメラまでの距離
        /// </summary>
        public float Distance = 0.5f;

        private Transform _TargetTransform;
        private Transform _Target2Transform;

        /// <summary>
        /// カメラ配置オフセット
        /// </summary>
        public Vector3 Offset = new Vector3(0, 0, 0);

        /// <summary>
        /// 高さ変更割合
        /// </summary>
        public float YRate = 1.0f;

        void Start() {
            if (this.player == null) {
                //TODO 顔の位置を取得できると良い？
                this.player = OcPlMng.Inst.getPl(0).gameObject;
            }
            //TODO OcPl.gameObjectからはAnimatorが取れない
            var a = player.GetComponent<Animator>();
            if (a == null) {
                //OcPl想定
                a = player.GetComponent<OcPl>().Animator;
            }
#if false
            //ボーン有無確認
            foreach (var bone in Enum.GetValues(typeof(HumanBodyBones)).OfType<HumanBodyBones>().Where(b => b != HumanBodyBones.LastBone)) {
                var t = a.GetBoneTransform(bone);
                UnityEngine.Debug.Log($"{bone} : {t?.position} : {t?.forward} / {t?.up} / {t?.right}");
            }
#endif

            if (this.Target != null) {
                _TargetTransform = a.GetBoneTransform(this.Target.Value);
            }
            if (this.Target2 != null) {
                _Target2Transform = a.GetBoneTransform(this.Target2.Value);
            }
            if (_TargetTransform == null && _Target2Transform == null) {
                _TargetTransform = a.GetBoneTransform(HumanBodyBones.Head);
            }
        }

        // Update is called once per frame
        void Update() {
            //プレイヤー位置
            var t = player.transform;

            //とりあえず、適当にプレーヤーの正面からの表示
            //  個別の注視位置基準ではなく、プレイヤー基準
            Vector3 v = t.forward;
            if (this.Target == HumanBodyBones.Hips) {
                //後ろからのテスト
                //TODO  向きを指定するような方法。
                //      横からとか Func<Transform,Vector3>?
                v = t.forward * -1;
            }
            var posTarget = _TargetTransform.position;
            if (_Target2Transform != null) {
                //右目と左目を指定の場合等
                posTarget = (posTarget + _Target2Transform.position) / 2;
            }
            //通常、注視点と同じ高さで離れた位置
            //YRateにより、地面からの割合でYの変更し、地面に埋まりづらいように調整
            float y = t.position.y + (posTarget.y - t.position.y) * this.YRate;
            var posCamera = posTarget + v * this.Distance;
            posCamera.y = y;

            //オフセットを加えてカメラ位置とする
            this.transform.position = posCamera + this.Offset;
            this.transform.LookAt(posTarget);
        }
    }
}
