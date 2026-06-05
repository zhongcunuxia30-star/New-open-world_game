using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Cinemachine FreeLook セットアップヘルパー
/// Unity エディタ上で Cinemachine カメラを自動構成する
/// Cinemachine 3.x (com.unity.cinemachine 3.1+) 対応
/// </summary>
public class CinemachineSetup : MonoBehaviour
{
    [Header("このスクリプトは Cinemachine 3.x のセットアップガイドです")]
    [Header("以下の手順でシーンにカメラを構成してください:")]
    [Space]
    [TextArea(15, 30)]
    [SerializeField] private string setupInstructions = @"
=== Cinemachine 3.x FreeLook セットアップ手順 ===

1. Hierarchy > Cinemachine > Cinemachine Camera を作成

2. 作成した CinemachineCamera に以下コンポーネントを追加:
   - CinemachineOrbitalFollow (Body)
   - CinemachineRotationComposer (Aim)

3. CinemachineOrbitalFollow の設定:
   - Orbit Style: ThreeRing
   - Top Rig:    Height = 4.5, Radius = 1.75
   - Middle Rig: Height = 2.5, Radius = 3.0
   - Bottom Rig: Height = 0.4, Radius = 1.3
   - Damping: Position = 1, Rotation = 1

4. CinemachineRotationComposer の設定:
   - Tracked Object Offset: (0, 1.5, 0)
   - Lookahead Time: 0
   - Damping: 0.5

5. Tracking Target に Player オブジェクトを設定

6. CinemachineInputAxisController を追加:
   - Orbit X: Mouse X → Sensitivity 200
   - Orbit Y: Mouse Y → Sensitivity 2, Invert

7. Main Camera に CinemachineBrain が自動追加されることを確認

※ このプロジェクトでは CameraController.cs による
  手動カメラ制御も用意しています。
  Cinemachine を使用しない場合はそちらをご利用ください。
";

    [Header("代替: CameraController.cs を使用する場合")]
    [SerializeField] private bool useCinemachine = true;

    private void OnValidate()
    {
        if (!useCinemachine)
        {
            Debug.Log("[CinemachineSetup] Cinemachine 無効 - CameraController.cs を使用してください");
        }
    }
}
