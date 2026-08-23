using System;
using ObservableCollections;

//IObservableCollection의 NotifyCollectionChangedEventHandler를 담고있는 Holder, IDisposable 처리를 위해 필요하다
namespace Game.UserData.Repository
{
    public class NotifyCollectionChangedHolder<TModel> : IDisposable
    {
        private IObservableCollection<TModel> _models;
        private NotifyCollectionChangedEventHandler<TModel> _eventHandler;

        public NotifyCollectionChangedHolder(IObservableCollection<TModel> models, NotifyCollectionChangedEventHandler<TModel> eventHandler)
        {
            _models = models;
            _eventHandler = eventHandler;
            _models.CollectionChanged += eventHandler;
        }

        public void Dispose()
        {
            _models.CollectionChanged -= _eventHandler;
        }
    }
}