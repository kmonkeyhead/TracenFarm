using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Command;
using Game.Content.InGame.Payload;
using UnityEngine;
using VitalRouter;

namespace Game.Content.InGame.UseCase
{
    public class PropsProgressService : IDisposable
    {
        private readonly FarmStore _farmStore;
        private readonly ICommandPublisher _commandPublisher;
        private readonly CancellationTokenSource _cts = new();

        public PropsProgressService(FarmStore farmStore, ICommandPublisher commandPublisher)
        {
            _farmStore = farmStore;
            _commandPublisher = commandPublisher;
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
                            entry.WorkingProgress = 0f;
                            _commandPublisher.PublishAsync(new PropWorkCompletedCommand(entry.Prop.PropType, entry.Id)).AsUniTask().Forget();
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