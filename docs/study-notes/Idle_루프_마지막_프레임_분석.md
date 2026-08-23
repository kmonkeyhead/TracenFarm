# Idle 중간 지점에서 다리가 튀는 현상

## 관찰한 증상

처음에는 Idle 애니메이션이 끝나고 다시 시작될 때 다리가 튄다고 추정했다. 사용자가 제공한 5.96초 영상을 프레임 단위로 확인한 결과, 실제 문제는 루프 시작과 끝이 아니라 클립 중간 약 `1.3~1.4초`에서 발생했다.

화면 왼쪽 다리는 캐릭터 기준 오른쪽 다리인 `R` 본이다.

## 초기 가설과 반증

### 초기 가설

`Loop Pose`가 꺼져 있어 마지막 프레임과 첫 프레임 사이의 보간이 끊긴다고 추정했다.

### 검사 결과

- 클립 길이: 약 `2.666667초`
- 샘플 레이트: `30 FPS`
- 다리 본의 첫 프레임과 마지막 프레임 회전값: 동일
- 실제 급변 시점: `1.333초`와 `1.433초`

따라서 Loop Pose는 켜 두는 편이 좋지만, 영상에서 보이는 중간 다리 튐의 직접 원인은 아니었다.

## 실제 원인

무릎 회전은 Quaternion 네 채널로 저장된다.

```text
m_LocalRotation.x
m_LocalRotation.y
m_LocalRotation.z
m_LocalRotation.w
```

Idle 클립의 `Knee_L`, `Knee_R`에서 값이 계속 0인 Y/Z 채널 일부에 다음과 같은 비정상 Tangent가 들어 있었다.

```text
outTangent = -Infinity
```

Unity는 이 Quaternion 채널들을 하나의 회전 곡선 묶음으로 평가한다. Y/Z 채널의 `-Infinity` Tangent 때문에 X/W까지 사실상 Step 형태로 평가되어 다음 현상이 발생했다.

```text
시작 직후 → 중간 무릎 포즈로 즉시 이동
중간 지점 → 다음 무릎 포즈로 한 프레임에 이동
```

Tangent 자체는 곡선이 키를 통과할 때의 기울기다. `Infinity` Tangent는 일반적인 부드러운 기울기가 아니라 Constant/Step 보간을 나타내는 특수값으로 사용될 수 있다.

## 수치로 확인한 결과

수정 전 30 FPS 샘플링:

- `Knee_R`: `1.333초`에 한 프레임 동안 약 `6.56°` 회전
- `Knee_L`: `1.433초`에 한 프레임 동안 약 `1.85°` 회전
- 화면 왼쪽 `Ankle_R`: 평상시보다 약 20배 큰 위치 이동

비정상 Tangent 수정 후:

- `Knee_R` 최대 프레임 회전: 약 `0.33°`
- 화면 왼쪽 발목 위치 급변: 약 125배 감소
- 반대쪽 발목 위치 급변: 약 44배 감소

## 적용한 해결 방법

무릎 Quaternion 채널에서 `NaN`, `Infinity`, `-Infinity`인 Tangent만 찾아 `0`으로 바꾸고 Tangent Mode를 `Free`로 설정했다. 정상적인 유한 Tangent와 키 값은 변경하지 않았다.

Unity 메뉴에서 다시 실행할 수 있다.

```text
Uma Extracted
└─ Fix Special Week MINI Idle Knee Curves
```

## Loop Time과 Loop Pose

중간 지점 문제와 별개로 루프 경계 품질을 위해 다음 설정도 유지한다.

```text
Loop Time: On
Loop Pose: On
```

## 용어 정리

- `Keyframe`: 특정 시간의 값
- `Animation Curve`: 키 사이의 값을 계산하는 곡선
- `Tangent`: 키에 들어오고 나가는 곡선의 기울기
- `Step/Constant`: 중간값을 부드럽게 계산하지 않고 특정 시점에 값이 즉시 바뀌는 보간
- `Quaternion`: 3D 회전을 X/Y/Z/W 네 값으로 표현하는 방식
- `Loop Pose`: 끝과 시작의 포즈 차이를 루프 전체에 분산해 경계를 부드럽게 만드는 Unity 옵션

## 원본 문제인가, 추출 과정의 문제인가

### 결론

현재 증거로는 **게임 원본 모션이 아니라, 추출된 AnimationClip을 Unity 편집용 곡선으로 변환하는 과정에서 생긴 문제일 가능성이 매우 높다.**

파일 복호화나 AssetBundle을 꺼내는 단계보다는 다음 구간이 가장 의심된다.

```text
원본의 압축 AnimationClip 데이터
→ AssetRipper가 Unity 편집용 AnimationCurve로 재구성
→ Unity 6가 내보낸 .anim의 Quaternion 곡선을 평가
```

### 그렇게 판단하는 근거

1. 추출된 `Knee_L`, `Knee_R`의 Quaternion Y/Z 채널에는 값이 모두 `0`인데 일부 Tangent만 `-Infinity`로 기록되어 있었다.
2. 이 비정상 Tangent 때문에 Unity가 X/Y/Z/W 회전 채널 전체를 묶어서 사실상 Step 곡선처럼 평가했다.
3. 키프레임의 시간과 값은 바꾸지 않고 비정상 Tangent만 `0`으로 고쳤는데 점프가 사라졌다. 원래 자세 데이터보다 **보간 정보의 변환 결과**가 잘못됐다는 증거다.
4. 의도된 연출이라면 무릎이 오랫동안 같은 각도로 고정되었다가 드문 키 위치에서 갑자기 바뀌어야 한다. 자연스러운 Idle 동작으로 보기 어렵다.
5. 다른 모델·텍스처·애니메이션 데이터는 정상적으로 읽혔으므로 AssetBundle 전체의 복호화 실패나 파일 손상 패턴과도 다르다.

### 아직 100% 단정하지 않는 이유

AssetRipper의 정확히 같은 `-Infinity Quaternion tangent` 문제를 명시한 공식 버그 보고는 확인하지 못했다. 따라서 이는 **관측된 결과와 변환 구조에 기반한 판단**이다. 추출 프로젝트의 대상 버전과 현재 사용하는 Unity 6 사이의 버전 차이가 최종 곡선 평가에 영향을 주었을 가능성도 일부 남아 있다.

다만 잘못된 Tangent가 이미 추출된 `.anim`에 저장되어 있으므로 가능성의 우선순위는 다음과 같다.

```text
1순위: AssetRipper의 AnimationClip 편집 형식 변환
2순위: 변환된 곡선과 Unity 6 사이의 호환성
가능성 낮음: 게임 원본 모션 자체
```

### 확정하는 비교 방법

- UmaViewer에서 **원본 AssetBundle의 같은 Idle**을 재생했을 때 점프가 없다면 추출/변환 문제로 확정할 수 있다.
- 추출 프로젝트가 지정한 원래 Unity 버전에서도 같은 `.anim`이 튀는지 비교하면 AssetRipper 출력 문제와 Unity 6 호환성 문제를 더 구분할 수 있다.
- 원본 압축 AnimationClip의 회전 곡선을 직접 디코딩하여 추출된 `.anim`의 키와 Tangent를 비교하는 방법이 가장 확실하다.
