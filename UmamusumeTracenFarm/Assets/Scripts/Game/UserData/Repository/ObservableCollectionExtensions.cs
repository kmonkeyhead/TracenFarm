using ObservableCollections;

namespace Game.UserData.Repository
{
    public static class ObservableCollectionExtensions
    {
        public static NotifyCollectionChangedHolder<T> RegisterNotification<T>(this IObservableCollection<T> observable, NotifyCollectionChangedEventHandler<T> eventHandler)
        {
            return new NotifyCollectionChangedHolder<T>(observable, eventHandler);
        }
    }
}