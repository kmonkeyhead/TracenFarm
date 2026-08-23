# 비동기와 UniTask 질문

UniTask, `CancellationToken`, 비동기 작업 수명주기와 취소 처리에 관한 질문을 모은다.

## CancellationToken 취소를 즉시 예외로 전환

- **UniTask 코드에서 Cancellation이 요청됐을 때 강제로 예외를 던지는 한 줄 함수는 무엇인가?**  
  .NET `CancellationToken`의 `ThrowIfCancellationRequested()`를 호출한다.

  ```csharp
  cancellationToken.ThrowIfCancellationRequested();
  ```

  토큰에 취소 요청이 없으면 아무 동작도 하지 않고 다음 줄로 진행한다. 취소가 요청된 상태라면 즉시 `OperationCanceledException`을 던진다. UniTask 비동기 메서드 안에서 이 예외가 밖으로 전달되면 해당 UniTask는 일반 실패가 아니라 취소 상태로 처리된다.

  ```csharp
  private async UniTask ProduceAsync(CancellationToken cancellationToken)
  {
      cancellationToken.ThrowIfCancellationRequested();
      await UniTask.Delay(1000, cancellationToken: cancellationToken);
      cancellationToken.ThrowIfCancellationRequested();
  }
  ```

  이 함수는 실행 중인 코드를 외부에서 강제로 중단하는 함수가 아니다. 코드가 해당 줄에 도달했을 때 취소 여부를 확인하는 협력적 취소 지점이다. 오래 실행하는 반복문에서는 반복 중간에도 호출해야 빠르게 취소된다. 예외를 원하지 않는 흐름이라면 `IsCancellationRequested`를 검사해 반환하거나, await 결과에서 `SuppressCancellationThrow()`를 사용하는 방식을 선택한다.
