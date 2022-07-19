using System;
using System.Collections;
using System.Collections.Generic;

namespace AIO.Helpers {
    /// <summary>
    /// A generic ring buffer with fixed capacity.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the buffer</typeparam>
    public class RingBuffer<T> : IEnumerable<T>, IEnumerable, ICollection<T>, 
        ICollection {
        private int Head = 0;
        private int Tail = 0;

        private T[] Buffer;

        public bool AllowOverflow { get; }

        /// <summary>
        /// The total number of elements the buffer can store (grows).
        /// </summary>
        public int Capacity => Buffer.Length;

        /// <summary>
        /// The number of elements currently contained in the buffer.
        /// </summary>
        public int Size { get; private set; } = 0;

        /// <summary>
        /// Retrieve the next item from the buffer.
        /// </summary>
        /// <returns>The oldest item added to the buffer.</returns>
        public T Get() {
            if(Size == 0) throw new System.InvalidOperationException("Buffer is empty.");
            T item = Buffer[Head];
            Head = (Head + 1) % Capacity;
            Size--;
            return item;
        }

        /// <summary>
        /// Adds an item to the end of the buffer.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void Put(T item) {
            // If tail & head are equal and the buffer is not empty, assume
            // that it will overflow and throw an exception.
            if(Tail == Head && Size != 0) {
                if(AllowOverflow) {
                    AddToBuffer(item, true);
                }
                else {
                    throw new System.InvalidOperationException("The RingBuffer is full");
                }
            }
            // If the buffer will not overflow, just add the item.
            else {
                AddToBuffer(item, false);
            }
        }

        private void AddToBuffer(T toAdd, bool overflow) {
            if(overflow) {
                Head = (Head + 1) % Capacity;
            }
            else {
                Size++;
            }
            Buffer[Tail] = toAdd;
            Tail = (Tail + 1) % Capacity;
        }

        #region Constructors
        // Default capacity is 4, default overflow behavior is false.
        public RingBuffer() : this(4) { }

        public RingBuffer(int capacity) : this(capacity, false) { }

        public RingBuffer(int capacity, bool overflow) {
            Buffer = new T[capacity];
            AllowOverflow = overflow;
        }
        #endregion

        #region IEnumerable Members
        public IEnumerator<T> GetEnumerator() {
            int _index = Head;
            for(int i = 0; i < Size; i++, _index = (_index + 1) % Capacity) {
                yield return Buffer[_index];
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return (IEnumerator)GetEnumerator();
        }
        #endregion

        #region ICollection<T> Members
        public int Count => Size;
        public bool IsReadOnly => false;

        public void Add(T item) {
            Put(item);
        }
        
        /// <summary>
        /// Determines whether the RingBuffer contains a specific value.
        /// </summary>
        /// <param name="item">The value to check the RingBuffer for.</param>
        /// <returns>True if the RingBuffer contains <paramref name="item"/>
        /// , false if it does not.
        /// </returns>
        public bool Contains(T item) {
            var comparer = EqualityComparer<T>.Default;
            int index = Head;
            for(var i = 0; i < Size; i++, index = (index + 1) % Capacity) {
                if(comparer.Equals(item, Buffer[index])) return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all items from the RingBuffer.
        /// </summary>
        public void Clear() {
            for(int i = 0; i < Capacity; i++) {
                Buffer[i] = default(T);
            }
            Head = 0;
            Tail = 0;
            Size = 0;
        }

        /// <summary>
        /// Copies the contents of the RingBuffer to <paramref name="array"/>
        /// starting at <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array">The array to be copied to.</param>
        /// <param name="arrayIndex">The index of <paramref name="array"/>
        /// where the buffer should begin copying to.</param>
        public void CopyTo(T[] array, int arrayIndex) {
            int index = Head;
            for(var i = 0; i < Size; i++, arrayIndex++, index = (index + 1) %
                Capacity) {
                array[arrayIndex] = Buffer[index];
            }
        }

        /// <summary>
        /// Removes <paramref name="item"/> from the buffer.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if <paramref name="item"/> was found and 
        /// successfully removed. False if <paramref name="item"/> was not
        /// found or there was a problem removing it from the RingBuffer.
        /// </returns>
        public bool Remove(T item) {
            int index = Head;
            var removeIndex = 0;
            var foundItem = false;
            var comparer = EqualityComparer<T>.Default;
            for(var i = 0; i < Size; i++, index = (index + 1) % Capacity) {
                if(comparer.Equals(item, Buffer[index])) {
                    removeIndex = index;
                    foundItem = true;
                    break;
                }
            }
            if(foundItem) {
                var newBuffer = new T[Size - 1];
                index = Head;
                var pastItem = false;
                for(var i = 0; i < Size - 1; i++, index = (index + 1) % Capacity) {
                    if(index == removeIndex) {
                        pastItem = true;
                    }
                    if(pastItem) {
                        newBuffer[index] = Buffer[(index + 1) % Capacity];
                    }
                    else {
                        newBuffer[index] = Buffer[index];
                    }
                }
                Size--;
                Buffer = newBuffer;
                return true;
            }
            return false;
        }
        #endregion

        #region ICollection Members
        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// RingBuffer.
        /// </summary>
        public object SyncRoot => this;

        /// <summary>
        /// Gets a value indicating whether access to the RingBuffer is 
        /// synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized => false;

        /// <summary>
        /// Copies the elements of the RingBuffer to <paramref name="array"/>, 
        /// starting at a particular Array <paramref name="index"/>.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the 
        /// destination of the elements copied from RingBuffer. The Array must 
        /// have zero-based indexing.</param>
        /// <param name="index">The zero-based index in 
        /// <paramref name="array"/> at which copying begins.</param>
        void ICollection.CopyTo(Array array, int index) {
            CopyTo((T[])array, index);
        }
        #endregion
    }
}