using UnityEngine;

/// <summary>
/// シーンセットアップの手順書
/// このスクリプトは参照用であり、ゲームオブジェクトにアタッチする必要はありません
/// </summary>
public class SetupGuide : MonoBehaviour
{
    [TextArea(40, 60)]
    [SerializeField] private string guide = @"
╔══════════════════════════════════════════════════════════╗
║         原神風操作システム - セットアップガイド          ║
╚══════════════════════════════════════════════════════════╝

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
■ 1. Player オブジェクト構成
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  Player (GameObject)
  ├─ CharacterController (コンポーネント)
  │   ├─ Center: (0, 1, 0)
  │   ├─ Height: 2
  │   └─ Radius: 0.3
  ├─ PlayerController.cs
  ├─ PlayerInput (コンポーネント)
  │   ├─ Actions: GameInputActions.inputactions
  │   ├─ Default Map: Player
  │   └─ Behavior: Invoke Unity Events
  └─ Animator (任意)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
■ 2. Camera 構成 (手動カメラ版)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  Main Camera
  └─ CameraController.cs
      ├─ Follow Target: Player の Transform
      └─ Target Offset: (0, 1.5, 0)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
■ 2B. Camera 構成 (Cinemachine 版)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  Main Camera
  ├─ CinemachineBrain (自動追加)
  
  CinemachineCamera (別オブジェクト)
  ├─ CinemachineOrbitalFollow
  │   ├─ Orbit Style: ThreeRing
  │   ├─ Top:    H=4.5, R=1.75
  │   ├─ Mid:    H=2.5, R=3.0
  │   └─ Bottom: H=0.4, R=1.3
  ├─ CinemachineRotationComposer
  │   └─ Tracked Offset: (0, 1.5, 0)
  ├─ CinemachineInputAxisController
  └─ Tracking Target: Player

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
■ 3. UI 構成
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  Canvas
  ├─ MenuPanel (初期非表示)
  ├─ MapPanel (初期非表示)
  ├─ BagPanel (初期非表示)
  └─ CharacterPanel (初期非表示)

  UIManager (GameObject)
  └─ UIManager.cs
      ├─ Menu Panel → MenuPanel
      ├─ Map Panel → MapPanel
      ├─ Bag Panel → BagPanel
      └─ Character Panel → CharacterPanel

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
■ 4. GameManager 構成
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  GameManager (GameObject)
  └─ GameManager.cs
      ├─ Player Controller → Player
      ├─ Camera Controller → Main Camera
      ├─ UI Manager → UIManager
      └─ Player Input → Player の PlayerInput

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
■ 5. PlayerInput イベント接続
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  PlayerInput コンポーネントの Events > Player:
  ├─ Move      → PlayerController.OnMove
  ├─ Look      → CameraController.OnLook
  ├─ Jump      → PlayerController.OnJump
  ├─ Sprint    → PlayerController.OnSprint
  ├─ Walk      → PlayerController.OnWalk
  ├─ Attack    → PlayerController.OnAttack
  ├─ Skill     → PlayerController.OnSkill
  ├─ ElementalBurst → PlayerController.OnElementalBurst
  ├─ Dodge     → PlayerController.OnDodge
  ├─ Interact  → PlayerController.OnInteract
  └─ Zoom      → CameraController.OnZoom

  PlayerInput コンポーネントの Events > UI:
  ├─ Menu      → UIManager.OnMenu
  ├─ Map       → UIManager.OnMap
  ├─ Bag       → UIManager.OnBag
  └─ Character → UIManager.OnCharacter

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
■ 操作一覧
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  WASD        : 移動
  Space       : ジャンプ
  Shift(長押し): ダッシュ
  Ctrl        : 歩き切替
  左クリック   : 通常攻撃
  E           : スキル
  Q           : 元素爆発
  右クリック   : 回避
  F           : インタラクト
  Esc         : メニュー
  M           : マップ
  B           : バッグ
  C           : キャラ画面
  マウスホイール: カメラズーム
";
}
