using System;
using System.Collections;
using System.Collections.Generic;
using Game.UserData.Model;
using ObservableCollections;

namespace Game.UserData.Repository
{
    //공용 Repository
    public class GenericRepository<Tkey, TValue> : IObservableCollection<TValue> where TValue : IModel
    {
        private readonly Func<TValue, Tkey> _keySelector;
        private readonly ObservableList<TValue> _models;
        private readonly Dictionary<Tkey, TValue> _index;

        public GenericRepository(Func<TValue, Tkey> keySelector)
        {
            //모델에서 key는 어느게 될지 알 수 없으므로 Func로 처리한다
            //모델의 키를 가져오는 함수
            _keySelector = keySelector;
            _models = new ObservableList<TValue>();
            _index = new Dictionary<Tkey, TValue>();
        }
        
        public IEnumerable<TValue> Models => _models;

        public event NotifyCollectionChangedEventHandler<TValue> CollectionChanged
        {
            add { _models.CollectionChanged += value; }
            remove { _models.CollectionChanged -= value; }
        }

        public ISynchronizedView<TValue, TView> CreateView<TView>(Func<TValue, TView> transform, bool reverse = false)
        {
            //사용 안 함
            throw new NotImplementedException();
        }

        public NotifyCollectionChangedHolder<TValue> Subscribe(NotifyCollectionChangedEventHandler<TValue> eventHandler)
        {
            return _models.RegisterNotification(eventHandler);
        }

        public IEnumerator<TValue> GetEnumerator() => _models.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ISynchronizedView<TValue, TView> CreateView<TView>(Func<TValue, TView> transform)
        {
            throw new NotImplementedException();
        }

        public object SyncRoot => _models.SyncRoot;

        public int Count => _models.Count;

        public void AddOrReplace(TValue model)
        {
            // 안전한 처리를 위해 넣어야 함
            if (TryGetValue(_keySelector(model), out var existingItem))
            {
                lock (SyncRoot)
                {
                    _index[_keySelector(model)] = model;

                    int index = _models.IndexOf(existingItem);
                    _models[index] = model;
                }
            }
            else
            {
                lock (SyncRoot)
                {
                    _models.Add(model);
                    _index.Add(_keySelector(model), model);
                }
            }
        }

        public void Remove(TValue model)
        {
            lock (SyncRoot)
            {
                _index.Remove(_keySelector(model));
                _models.Remove(model);
            }
        }

        public bool TryGetValue(Tkey key, out TValue value)
        {
            lock (SyncRoot)
            {
                return _index.TryGetValue(key, out value);
            }
        }
    }
}