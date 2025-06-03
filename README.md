# 🧠 Vision Field System for Unity

## 🔍 Overview

**VisionFieldSystem** — модульная и гибкая система обзора (FOV) для Unity, включающая визуализацию зоны видимости и обнаружение целей с учётом препятствий. Поддерживает разные формы обзора и настраивается через инспектор.

Подходит для врагов, турелей, NPC, охранников и любых AI-систем восприятия.


## 📽️ Demo Preview

| Type             | Mesh + Detector Pair                                   | Preview                                                |
|------------------|--------------------------------------------------------|--------------------------------------------------------|
| **Circle**       | `VisionFieldMesh` + `TargetFieldDetector`             | ![](https://github.com/user-attachments/assets/c0864105-69f8-498b-9de5-39c403db2e54) |
| **Arrow**        | `VisionArrowMesh` + `TargetArrowDetector`             | ![](https://github.com/user-attachments/assets/3f6d499e-c2e4-4b62-b511-3e272ae8206d) |
| **Half Ellipse** | `VisionHalfEllipseMesh` + `TargetHalfEllipseDetector` | ![](https://github.com/user-attachments/assets/8ce44c5d-a9ee-48d7-9dfa-58d729b61d30) |
| **Triangle**     | `VisionOffsetTriangleMesh` + `TargetOffsetTriangleDetector` | ![](https://github.com/user-attachments/assets/891876e5-8b9b-4720-b407-1e030d66192f) |
| **Square**       | `VisionSquareMesh` + `TargetSquareDetector`           | ![](https://github.com/user-attachments/assets/5a57c130-37a7-4c5e-9add-12e90ee40af6) |
| **Rhombus**      | `VisionRhombusMesh` + `TargetRhombusDetector`         | ![](https://github.com/user-attachments/assets/e03875ed-f6a6-4724-a7a3-da4cb4524cde) |

![image](https://github.com/user-attachments/assets/4acb0538-51d5-40d1-9329-589ddc005b30)

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

---

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
