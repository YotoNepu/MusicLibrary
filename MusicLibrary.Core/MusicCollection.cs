using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MusicLibrary.Core
{
    public class MusicCollection : IEnumerable<MusicItem>
    {
        private readonly List<MusicItem> items;

        public MusicCollection()
        {
            items = new List<MusicItem>();
        }

        public int Count
        {
            get { return items.Count; }
        }

        public MusicItem this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        public void Add(MusicItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (string.IsNullOrWhiteSpace(item.Title))
                throw new MusicItemException("Название композиции не может быть пустым.");

            if (string.IsNullOrWhiteSpace(item.Artist))
                throw new MusicItemException("Исполнитель не может быть пустым.");

            items.Add(item);
        }

        public bool Remove(MusicItem item)
        {
            return items.Remove(item);
        }

        public MusicItem FindById(int id)
        {
            return items.FirstOrDefault(x => x.Id == id);
        }

        public void Clear()
        {
            items.Clear();
        }

        public IEnumerator<MusicItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static MusicCollection operator +(MusicCollection collection, MusicItem item)
        {
            collection.Add(item);
            return collection;
        }
    }
}