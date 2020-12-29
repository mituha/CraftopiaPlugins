#
# Libs フォルダーで実行し、必要なファイルをシンボリックリンクとして設定します
#
#

function Test-Admin
{
    $principal = [Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()
    $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

if(!(Test-Admin)){
    # 新しいPowerShell(PowerShell 7.1)のインストールが必要です
    # https://github.com/PowerShell/PowerShell/releases/tag/v7.1.0
    Start-Process pwsh.exe -ArgumentList $PSCommandPath -Verb RunAs -Wait
    exit
}


function Update-SymbolicLink([string] $srcDir,[string] $dll) {
    # Libsでの実行を想定
    $dstPath = Join-Path $PSScriptRoot $dll
    Remove-Item $dstPath
    $srcPath = Join-Path $srcDir $dll -Resolve

    New-Item -Value $srcPath -Path $dstPath -ItemType SymbolicLink
}

$dirCraftopia = "D:\Program Files (x86)\Steam\steamapps\common\Craftopia\"
# TODO Cドライブ等

# BepInEx関連
$srcDir = Join-Path $dirCraftopia "BepInEx\core"

$files = @(
    "BepInEx.dll"       # 
    "BepInEx.Harmony.dll"       # 
    "0Harmony.dll"    #
    "0Harmony.xml"    #
)
foreach($file in $files){
    Update-SymbolicLink $srcDir $file
}



# Craftopia固有
$srcDir = Join-Path $dirCraftopia "Craftopia_Data\Managed\"

$files = @(
    "Assembly-CSharp.dll"  # 
    "AD__Overcraft.dll"    #
)
foreach($file in $files){
    Update-SymbolicLink $srcDir $file
}

# Unity関連 フォルダーは同じ
$srcDir = Join-Path $dirCraftopia "Craftopia_Data\Managed\"

$files = @(
    "UnityEngine.CoreModule.dll"       # 
    "UnityEngine.dll"                   #

    # UI用
    "UnityEngine.UI.dll"
    "UnityEngine.UIModule.dll"
    # Font
    "UnityEngine.TextRenderingModule.dll"
    # Rigidbody
    "UnityEngine.PhysicsModule.dll"
    # Animator
    "UnityEngine.AnimationModule.dll"
    # AssetBundle用
    "UnityEngine.AssetBundleModule.dll"

    # TextMeshPro
    "Unity.TextMeshPro.dll"
)
foreach($file in $files){
    Update-SymbolicLink $srcDir $file
}
