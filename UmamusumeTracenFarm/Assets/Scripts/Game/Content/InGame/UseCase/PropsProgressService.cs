using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Content.InGame.Payload;
using UnityEngine;

namespace Game.Content.InGame.UseCase
{
    public class PropsProgressService : IDisposable
    {
        private readonly FarmStore _farmStore;
        private readonly CancellationTokenSource _cts = new();

        public PropsProgressService(FarmStore farmStore)
        {
            _farmStore = farmStore;
        }

        public void StartProgress() => UpdateProgressAsync(_cts.Token).Forget();

        private async UniTask UpdateProgressAsync(CancellationToken ct)
        {
            try
            {
                while (true)
                {
                    foreach (var entry in _farmStore.Entries)
                    {
                        if (entry.WorkingCount == 0)
                        {
                            entry.WorkingProgress = 0;
                            entry.WorkingType = PropWorkingType.None;
                            continue;
                        }

                        if (entry.WorkingType == PropWorkingType.Complete)
                        {
                            continue;
                        }

                        float progress = entry.WorkingProgress;
                        progress += entry.WorkingCount * Time.deltaTime;
                        entry.WorkingProgress = Mathf.Clamp01(progress);

                        if (entry.WorkingProgress >= 1f)
                        {
                            entry.WorkingType = PropWorkingType.Complete;
                        }
                    }

                    await UniTask.WaitForEndOfFrame(cancellationToken: ct);
                }
            }
            finally
            {
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}