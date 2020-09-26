カメラ関連
=============================


* OcPlCam
    - SoCamera
    - SoCamera_PV
    - SoCameraShake
* OcUICam
* OcCamCtrl
* OcFreeCam
* OcMapCam


Unityの機能？として、`Camera.main` で `MainCamera` を取得可能。  

```
OcPlCam : Camera : [-982409472]
	Scene : DontDestroyOnLoad
	position : (293.7, 38.2, -225.9)
	enabled : True
	depth : 10
	 -(OcPlCam) : Transform : [-1913131520]
	 -(OcPlCam) : Camera : [-982409472]
	 -(OcPlCam) : AudioListener : [-1600610304]
	 -(OcPlCam) : OcPlCam : [563218944]
	 -(OcPlCam) : PostProcessLayer : [1036566528]
	 -(OcPlCam) : TranslucentImageSource : [-569508864]
	 -(OcPlCam) : AzureFogScattering : [-107697152]
	 -(OcPlCam) : FlareLayer : [1496608768]
	parent : OcPlMasterSetPrefab(Clone)
OcUICamera : Camera : [788097280]
	Scene : DontDestroyOnLoad
	position : (1024.0, 1024.0, 1024.0)
	enabled : True
	depth : 80
	 -(OcUICamera) : Transform : [-860216832]
	 -(OcUICamera) : Camera : [788097280]
	 -(OcUICamera) : OcUICam : [2098509056]
	parent : OcPlMasterSetPrefab(Clone)
```
depthが10になっている。
