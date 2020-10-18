using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// ポーズ同期用
    /// </summary>
    internal sealed class SyncHumanPose : MonoBehaviour
    {
        private Animator _Source;
        private HumanPoseHandler _SrcPose;
        private HumanPoseHandler _DstPose;

        public void Setup(Animator source) {
            var animator = this.GetComponent<Animator>();
            _Source = source;
            animator.runtimeAnimatorController = source.runtimeAnimatorController;
            _SrcPose = new HumanPoseHandler(source.avatar, source.transform);
            _DstPose = new HumanPoseHandler(animator.avatar, animator.transform);


#if true    //親子関係を作る前に個々の高さを取得
            var srcHeight = GetHeight("Src", source);
            var dstHeight = GetHeight("Dst", animator);
            var scale = dstHeight / srcHeight;
            Debug.Log($"{dstHeight} / {srcHeight} = {scale}");
#endif
            //Player側の表示を消す
            foreach (var renderer in source.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                Debug.Log($"{renderer.name}({renderer.GetType().Name}) OFF");
                renderer.enabled = false;
            }
            //顔や髪の毛はMeshRenderer
            foreach (var renderer in source.GetComponentsInChildren<MeshRenderer>()) {
                Debug.Log($"{renderer.name}({renderer.GetType().Name}) OFF");
                renderer.enabled = false;
            }



            //位置も親子関係とする
            this.gameObject.transform.SetParent(source.transform, false);
            //this.gameObject.transform.localPosition = this.gameObject.transform.right;// Vector3.zero;
            //this.gameObject.transform.position = this.gameObject.transform.right;// Vector3.zero;
            //this.gameObject.transform.position = Vector3.zero;
            //this.gameObject.transform.rotation = Quaternion.identity;
            this.gameObject.transform.localPosition = Vector3.zero;
            this.gameObject.transform.localRotation = Quaternion.identity;
            //Debug.Log($"Setup Src {source.transform.localPosition} / {source.transform.position}");
            //Debug.Log($"Setup Dst {this.gameObject.transform.localPosition} / {this.gameObject.transform.position}");




#if false   //この位置は正規化さえている？
            //ボーン位置比較
            foreach (var bone in Enum.GetValues(typeof(HumanBodyBones)).OfType<HumanBodyBones>().Where(b => b != HumanBodyBones.LastBone)) {
                var t = source.GetBoneTransform(bone);
                var t2 = animator.GetBoneTransform(bone);

                UnityEngine.Debug.Log($"{bone} : {t?.position} : {t2?.position} : {t2?.position.y / t?.position.y}");
            }
#endif

            //ボーン位置は正規化されている？っぽいので高さ比較には使用できない
#if false
            //大雑把にスケールの調整
            //  このSetup時点の位置に依存する
            var srcHips = source.GetBoneTransform(HumanBodyBones.Hips);
            var dstHips = animator.GetBoneTransform(HumanBodyBones.Hips);
            var scale = dstHips.position.y / srcHips.position.y;
            Debug.Log($"{dstHips.position} / {srcHips.position} = {scale}");
            //元のプレーヤーのスケールをVRMに合わせる
            source.gameObject.transform.localScale *= scale;
            //VRMのスケールも合わせてかわってしまうため、戻す
            this.gameObject.transform.localScale /= scale;
            Debug.Log($"{this.gameObject.transform.localScale} / {source.gameObject.transform.localScale}");
#endif
#if false
            //見た目と異なり、ボーン位置に差がない？

            //大雑把にスケールの調整
            //  このSetup時点の位置に依存する
            var srcHead = source.GetBoneTransform(HumanBodyBones.Head);
            var dstHead = animator.GetBoneTransform(HumanBodyBones.Head);
            var scale = dstHead.position.y / srcHead.position.y;
            Debug.Log($"{dstHead.position} / {srcHead.position} = {scale}");
            //元のプレーヤーのスケールをVRMに合わせる
            source.gameObject.transform.localScale *= scale;
            //VRMのスケールも合わせてかわってしまうため、戻す
            this.gameObject.transform.localScale /= scale;
            Debug.Log($"{this.gameObject.transform.localScale} / {source.gameObject.transform.localScale}");
#endif
            //元のプレーヤーのスケールをVRMに合わせる
            source.gameObject.transform.localScale *= scale;
            //VRMのスケールも合わせてかわってしまうため、戻す
            this.gameObject.transform.localScale /= scale;
            Debug.Log($"{this.gameObject.transform.localScale} / {source.gameObject.transform.localScale}");

        }

        /// <summary>
        /// モデルの身長の取得
        /// </summary>
        /// <param name="name"></param>
        /// <param name="animator"></param>
        /// <returns></returns>
        /// <remarks>
        /// PlayerはSkinnedMeshRendererが4つしかなかった。
        /// </remarks>
        private float GetHeight(string name, Animator animator) {
            float max = 0, min = float.MaxValue;
            foreach (var renderer in animator.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                Debug.Log($"[{name}]({renderer.name}) localBounds : {renderer.localBounds} / Bounds : {renderer.bounds} / min : {renderer.bounds.min} / max : {renderer.bounds.max}");

                var yMax = renderer.bounds.max.y;
                if (yMax > max) {
                    max = yMax;
                }
                var yMin = renderer.bounds.min.y;
                if (yMin < min) {
                    min = yMin;
                }
            }
            return max - min;
        }

        private HumanPose _Pose;
        private void LateUpdate() {
            if (_SrcPose == null || _DstPose == null) { return; }
            _SrcPose.GetHumanPose(ref _Pose);
            //ワールド座標系での値になっている模様
            //Debug.Log($"bodyPosition : {_Pose.bodyPosition} / bodyRotation : {_Pose.bodyRotation}");
            WriteInformation("Source", _Source);

            //this.gameObject.transform.position = Vector3.zero;
            //this.gameObject.transform.rotation = Quaternion.identity;
            //this.gameObject.transform.localPosition = Vector3.zero;
            //this.gameObject.transform.localRotation = Quaternion.identity;

            HumanPose oldPose = new HumanPose();
            //HumanPose newPose = new HumanPose();
            _DstPose.GetHumanPose(ref oldPose);

            //重心、および、腰と両肩の中点からの向きであるため、正確に処理するには問題がありそう
            //_Pose.bodyPosition -= _Source.transform.position;
            //_Pose.bodyRotation *= Quaternion.Inverse(_Source.transform.rotation);


            _DstPose.SetHumanPose(ref _Pose);
            //_DstPose.GetHumanPose(ref newPose);

            //Debug.Log($"localPosition : {this.transform.localPosition} / position : {this.transform.position} / localRotation : {this.transform.localRotation} / rotation : {this.transform.rotation}");
            WriteInformation("VRM", this.GetComponent<Animator>());

            //Debug.Log($"{oldPose.bodyPosition} -> {_Pose.bodyPosition} -> {newPose.bodyPosition}");
            //Debug.Log($"{oldPose.bodyRotation} -> {_Pose.bodyRotation} -> {newPose.bodyRotation}");

            //return;

            //_SrcPose = null;

            //https://forum.unity.com/threads/humanpose-issue-continued.484128/
            //ポーズの同期で位置も変わってしまう？
            //  Craftopiaのプレーヤーでは問題なさそう
            //      
            //なお、テスト的にユニティちゃんのプロジェクトで同期させようとした場合、上手く動かなかった
            //ユニティちゃん自体のCharacter1_Hipsの配置が正規化？されたおらず、回転がかかった状態であるためと考えられます。

            //this.gameObject.transform.localPosition = this.gameObject.transform.right;// Vector3.zero;
            //this.gameObject.transform.localPosition = Vector3.zero;
            //this.gameObject.transform.localRotation = Quaternion.identity;
            //Debug.Log($"LateUpdate {this.gameObject.transform.localPosition} / {this.gameObject.transform.position}");
        }
        private void WriteInformation(string name, Animator animator) {
            //Transformは同じままの模様
            var transform = animator.transform;
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips);

            //Debug.Log($"[{name}] {hips.position}({hips.rotation}) / localPosition : {transform.localPosition} / position : {transform.position} / localRotation : {transform.localRotation} / rotation : {transform.rotation} / localScale : {transform.localScale} / {transform}");
        }
    }
}
