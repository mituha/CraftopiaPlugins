using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Oc;
using SR;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReSTAR.Craftopia.Plugin
{
    partial class CraftopiaSharp
    {
        #region ターミナルウィンドウ

        private TerminalWindow _TerminalWindow;

        private void InitializeTerminalWindow() {
            if (_TerminalWindow == null) {
                var inst = OcUI_ChatHandler.Inst;
                var root = inst.GetParent();
                var w = root.GetComponentInChildren<TerminalWindow>(true);
                if (w != null) {
                    _TerminalWindow = w;
                }
                if (_TerminalWindow == null) {
                    //Window枠に相当する
                    var o = new GameObject();
                    o.SetParent(root);
                    //TerminalWindowスクリプトを追加することで残りは自動生成
                    var tw = o.AddComponent<TerminalWindow>();
                    _TerminalWindow = tw;
                    _TerminalWindow.OnInput += InputCode;
                    _TerminalWindow.OnEscape += Escape;
                }
            }
            _TerminalWindow.SetActive(true);
        }
        private void TerminateTerminalWindow() {
            if (_TerminalWindow != null) {
                _TerminalWindow.OnInput -= InputCode;
                _TerminalWindow.OnEscape -= Escape;
                _TerminalWindow?.gameObject?.Destroy();
                _TerminalWindow = null;
            }

        }
        #endregion


        #region 補足
        /*
         * スクリプトとしての処理部分。
         * CraftopiaSharp.cs 部分は汎用的に他のプロジェクトでもリンクで使用するため、
         * partialとして分離。
         * 
         * 内部的な変数は `__hoge` でアクセスできるようにする
         * 
         */
        #endregion

        private ScriptState State { get; set; }
        public Script __Script => this.State?.Script;
        public Exception __Exception => this.State?.Exception;

        public object __ReturnValue => this.State?.ReturnValue;
        public ImmutableArray<ScriptVariable> __Variables => this.State?.Variables ?? new ImmutableArray<ScriptVariable>();

        public async void Initialize() {
            InitializeTerminalWindow();
            try {
                UnityEngine.Debug.Log($">> ScriptOptions");
                var options = ScriptOptions.Default
                    .AddReferences("Assembly-CSharp")   //文字列指定でなく、Assembly指定だとエラーになるかも？
                    .AddImports(
                        "System",
                        "System.Collections",
                        "System.Collections.Generic",
                        "System.Linq"
                    )
                    .AddImports(
                        "UnityEngine"   //Vector3等用
                    );
                UnityEngine.Debug.Log($"<< ScriptOptions");
                UnityEngine.Debug.Log($">> CSharpScript.Create");
                var s = CSharpScript.Create("", options, this.GetType());
                UnityEngine.Debug.Log($"<< CSharpScript.Create");

                UnityEngine.Debug.Log($">> ScriptState.RunAsync");
                this.State = await s.RunAsync(this);
                UnityEngine.Debug.Log($"<< ScriptState.RunAsync");
                //            this.State = await CSharpScript.RunAsync("", options, this);
                //       this.State = await CSharpScript.RunAsync("WriteLine(\"Craftopia#\")", options, this);
            } catch (TypeLoadException tlEx) {
                UnityEngine.Debug.LogError($">> Initialize TypeLoadError {tlEx.GetType().Name} / {tlEx.TypeName}");
                UnityEngine.Debug.LogError(tlEx);
                if (tlEx.InnerException != null) {
                    UnityEngine.Debug.LogError($"** {tlEx.InnerException.GetType().Name}");
                    UnityEngine.Debug.LogError(tlEx.InnerException);
                }
                UnityEngine.Debug.LogError("<< Initialize TypeLoadError");
                throw;
            } catch (Exception ex) {
                UnityEngine.Debug.LogError($">> Initialize Error {ex.GetType().Name}");
                UnityEngine.Debug.LogError(ex);
                UnityEngine.Debug.LogError("<< Initialize TypeLoadError");
                throw;
            }
        }

        public void Terminate() {
            TerminateTerminalWindow();
        }
        private void InputCode(string code) {
            this.OnInput?.Invoke(code);
        }
        public Action<string> OnInput;

        public Action<string> OnWriteLine;
        public Action<string, object> OnSuccess;
        public Action<string, Exception> OnError;

        public async Task Evaluate(string code) {
            this.OnWriteLine?.Invoke(code);
            var error = await EvaluateCore(code);
            if (error == null) {
                this.OnSuccess?.Invoke($"{this.State.ReturnValue}", this.State.ReturnValue);
            } else {
                var message = string.Join(Environment.NewLine, error.Diagnostics);
                this.OnError?.Invoke(message, error);
            }
        }
        public async Task<CompilationErrorException> EvaluateCore(string code) {
            CompilationErrorException error = null;
            try {
                this.State = await this.State.ContinueWithAsync(code);
            } catch (CompilationErrorException cex) {
                error = cex;
            } catch (Exception ex) {
                this.OnError?.Invoke(ex.Message, ex);
            }
            return error;
        }

        public void WriteLine(string message) {
            this.OnWriteLine?.Invoke(message);
        }

        public void Log(object message) {
            UnityEngine.Debug.Log(message);
        }
        public void LogError(object message) {
            UnityEngine.Debug.LogError(message);
        }


        public void StartEnterCode() {
            _TerminalWindow?.OnTerminalWindowShow();
        }
        public void EndEnterCode() {
            _TerminalWindow?.OnTerminalWindowHide();
        }
        public void ClearInputCode() {
            _TerminalWindow?.ClearInputCode();
        }

        public Action OnEscape;
        private void Escape() {
            this.OnEscape?.Invoke();
        }

    }
}
