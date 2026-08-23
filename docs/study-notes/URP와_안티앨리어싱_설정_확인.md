# URP와 안티앨리어싱 설정 확인

## 질문

화면이 많이 자글거리는데 안티앨리어싱이 적용되어 있는가? 또한 URP 프로젝트가 맞는가?

## 확인 결과

- `ProjectSettings/GraphicsSettings.asset`의 Scriptable Render Pipeline Settings에는 `UmaMini_URP`가 연결되어 있다. 따라서 현재 Unity 6 Ready 프로젝트는 URP 프로젝트가 맞다.
- `UmaMini_URP.asset`의 Render Scale은 `1.0`, MSAA는 `4x`다.
- `CharacterMovementTest` 씬의 Main Camera는 Allow MSAA가 활성화되어 있다.
- 같은 카메라의 URP Additional Camera Data에서 Anti-aliasing은 `None`, Post Processing은 비활성화되어 있다.
- Universal Renderer는 Forward 렌더링을 사용한다.

## 해석

현재 기하 폴리곤 경계에는 4x MSAA가 적용될 조건이 갖춰져 있다. 카메라 Inspector의 Anti-aliasing이 `None`인 것은 MSAA까지 꺼졌다는 뜻이 아니라, FXAA/SMAA/TAA 같은 카메라 후처리 AA를 사용하지 않는다는 뜻이다.

MSAA는 삼각형 외곽선에는 효과가 있지만 다음 종류의 자글거림은 충분히 해결하지 못한다.

- 머리카락이나 눈처럼 셰이더 또는 알파로 만든 내부 경계
- 고주파 텍스처와 낮은 밉맵 품질에서 생기는 반짝임
- 얇은 선, 작은 얼굴 파츠, 그림자 경계
- 움직일 때 프레임마다 픽셀 포함 여부가 달라지는 temporal shimmering

## Unity에서 확인할 위치

1. `Edit > Project Settings > Graphics`에서 Default Render Pipeline이 `UmaMini_URP`인지 확인한다.
2. `Edit > Project Settings > Quality`에서 현재 품질 단계가 `Ultra`인지 확인한다.
3. `UmaMini_URP` Inspector에서 Render Scale `1.0`, Anti Aliasing (MSAA) `4x`를 확인한다.
4. Main Camera의 Rendering 항목에서 Post Processing과 Anti-aliasing 설정을 확인한다.

## 다음 비교 실험

우선 Game 뷰를 고정된 해상도와 Scale `1x`로 두고 4x MSAA를 끈 화면과 비교한다. 차이가 거의 없거나 머리카락·얼굴 내부만 계속 흔들리면 원인은 폴리곤 외곽보다 텍스처/셰이더 또는 temporal aliasing 쪽이다. 그 경우 카메라 Post Processing을 켠 뒤 SMAA를 먼저 비교하고, 이동 중의 반짝임이 핵심이면 TAA도 별도로 비교한다. TAA는 잔상과 흐려짐이 생길 수 있으므로 SD 캐릭터의 얼굴과 빠른 이동에서 반드시 직접 확인한다.
