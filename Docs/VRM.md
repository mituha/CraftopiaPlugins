VRM
===========================

## DLL
UnityでUniVRMを導入してビルドすることで生成されるファイルを使用

```mermaid
classDiagram

	class VRM{
	}


	VRM ..> MToon
	class MToon{
	}

	VRM ..> DepthFirstScheduler
	class DepthFirstScheduler{
	}

	VRM ..> UniUnlit
	class UniUnlit{
	}

	VRM ..> UniHumanoid
	class UniHumanoid{
	}

	VRM ..> UniJSON
	class UniJSON{
	}

    VRM ..> ShaderPropertyRuntime
	class ShaderPropertyRuntime{
	}

    VRM ..> MeshUtility
	class MeshUtility{
	}
```

* VRM.dll
* MToon.dll
* DepthFirstScheduler.dll
* UniUnlit.dll
* UniHumanoid.dll
* UniJSON.dll
* ShaderProperty.Runtime.dll
* MeshUtility.dll

## UniVRM
https://vrm.dev/docs/univrm/

