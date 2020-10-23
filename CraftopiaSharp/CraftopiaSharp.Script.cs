﻿using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReSTAR.Craftopia.Plugin
{
    partial class CraftopiaSharp
    {
        #region 補足
        /*
         * スクリプトとしての処理部分。
         * CraftopiaSharp.cs 部分は汎用的に他のプロジェクトでもリンクで使用するため、
         * partialとして分離。
         * 
         */
        #endregion

        private ScriptState State { get; set; }

        public async void Initialize() {
            try {
                UnityEngine.Debug.Log($">> ScriptOptions");
                var options = ScriptOptions.Default;
                //.AddReferences(AppDomain.CurrentDomain.GetAssemblies()); //この方式はエラー  Can't create a metadata reference to an assembly without location.
                /*
                    .AddReferences(Assembly.GetEntryAssembly())
                    .AddImports(
                        "System",
                        "System.Collections",
                        "System.Collections.Generic",
                        "System.Linq"
                    )
                    .AddImports(
                        "UnityEngine"
                    );*/
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
    }
}
