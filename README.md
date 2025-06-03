# 🧠 Vision Field System for Unity

## 🔍 Overview

**VisionFieldSystem** — is a modular and flexible field of view (FOV) system for Unity that combines visual cone rendering with target detection, taking obstacles into account.
It supports various vision shapes and is fully configurable through the Inspector.

Perfect for enemies, turrets, NPCs, guards, and any AI perception systems.


## 📽️ Demo Preview

| Type             | Mesh + Detector Pair                                   | Preview                                                |
|------------------|--------------------------------------------------------|--------------------------------------------------------|
| **Circle**       | `VisionFieldMesh` + `TargetFieldDetector`             | ![](https://github.com/user-attachments/assets/1f34bb28-ebc9-4d96-980d-25c04aa4ec32) |
| **Arrow**        | `VisionArrowMesh` + `TargetArrowDetector`             | ![](https://github.com/user-attachments/assets/3d156c53-6247-4b12-964c-b37761eb8e22) |
| **Half Ellipse** | `VisionHalfEllipseMesh` + `TargetHalfEllipseDetector` | ![](https://github.com/user-attachments/assets/7428179c-7076-4655-a80b-7e3c62c0acfc) |
| **Triangle**     | `VisionOffsetTriangleMesh` + `TargetOffsetTriangleDetector` | ![](https://github.com/user-attachments/assets/14311f6b-3cf5-4a4b-8e3b-7f0ded496cf4) |
| **Square**       | `VisionSquareMesh` + `TargetSquareDetector`           | ![](https://github.com/user-attachments/assets/209ff446-9c2f-40d1-bcf4-e7555e82b437) |
| **Rhombus**      | `VisionRhombusMesh` + `TargetRhombusDetector`         | ![](https://github.com/user-attachments/assets/7a5720b7-c31b-47d9-b20e-e50b0fe9806c) |

## 📽️ Videos

https://github.com/user-attachments/assets/c0864105-69f8-498b-9de5-39c403db2e54

https://github.com/user-attachments/assets/3f6d499e-c2e4-4b62-b511-3e272ae8206d

https://github.com/user-attachments/assets/8ce44c5d-a9ee-48d7-9dfa-58d729b61d30

## ⚙️ Features

- 🧩 Visual FOV using procedural mesh
- 🎯 Raycast-based target detection with obstacle mask
- 🧪 Debug Gizmos for ray tracing and hit visualization
- 🛠️ Fully customizable parameters (width, height, segments, tilt, offset)
- 🎥 Runtime detection events: `OnTargetDetected`, `OnTargetLost`
- 🌗 `RaycastOffset` to avoid surface clipping
- ♻️ Clean modular architecture: easy to extend

## 🧰 How to Use

1. Add a **Vision Mesh** component (e.g. `VisionFieldMesh`) to a GameObject.
2. Attach the corresponding **Target Detector** (e.g. `TargetFieldDetector`).
3. Set required masks and dimensions in the inspector.
4. Enable **Is Show Debug** to see rays and hit points in the Scene.
5. Subscribe to target events:

```csharp
_detector.OnTargetDetected += enemy => Debug.Log($"🎯 Seen: {enemy.name}");
_detector.OnTargetLost += enemy => Debug.Log($"👀 Lost: {enemy.name}");
🗂️ Project Structure
bash
Копировать
Редактировать
Assets/
└── Code/
    ├── Cameras/
    ├── Editors/
    ├── Enemies/
    ├── Infrastructure/
    ├── Levels/
    ├── Players/
    ├── UI/
    ├── Utilities/
    ├── Weapon/
    └── VisionCone/
        ├── Detectors/
        ├── Factory/
        ├── Provider/
        └── Visions/
```

✅ Requirements
Unity 2021.3+
URP or Built-in RP
DOTween (optional, for demo animations)
