using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FightinWords.Submissions;

/// <summary>
/// Sort of like a <see cref="Stack{T}"/>, in that it is last-in first-out, but has a limited <see cref="Capacity"/>,
/// automatically removing the oldest entry when it runs out of space.
/// </summary>
/// <typeparam name="T">the element type</typeparam>
public sealed class History<T>(int capacity) : IEnumerable<T>
{
    private readonly T[] _storage = new T[capacity];

    private int _offsetInStorage;

    internal int OffsetInStorage
    {
        get => _offsetInStorage;
        private set => _offsetInStorage = value % Capacity;
    }

    /// <summary>
    /// The maximum number of entries I can <see cref="Record(T)"/> before I have to start forgetting old ones.
    /// </summary>
    public int Capacity => _storage.Length;

    /// <summary>
    /// The number of entries that I <i>currently</i> have.
    /// </summary>
    public int Count { get; private set; }

    private int NewestIndexInStorage => GetIndexInStorage(0);
    private int OldestIndexInStorage => OffsetInStorage;

    /// <summary>
    /// My most recently <see cref="Record(T)"/>ed entry.
    /// </summary>
    public T Newest => _storage[NewestIndexInStorage];
    /// <summary>
    /// The first <see cref="Record(T)"/>ed entry <i>(that I can remember)</i>. 
    /// </summary>
    public T Oldest => _storage[OldestIndexInStorage];

    public T? OldestOrDefault() => Count > 0 ? Oldest : default;
    public T? NewestOrDefault() => Count > 0 ? Newest : default;

    /// <summary>
    /// Returns my <paramref name="index"/>-th most recently <see cref="Record(T)"/>ed <typeparamref name="T"/> entry. 
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="IndexOutOfRangeException">if <paramref name="index"/> isn't within <c>0..</c><see cref="Count"/></exception>
    public T this[int index]
    {
        get
        {
            // We have to put an extra check here, because `GetIndexInStorage(index)` will constrain the `index` to be within `length`.
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            return _storage[GetIndexInStorage(index)];
        }
    }

    private int GetIndexInStorage(int indexInHistory) =>
        LoopIndex(OffsetInStorage + Count - 1 - indexInHistory, Capacity);

    /// <summary>
    /// Takes <b><i>any</i></b> <see cref="int"/>, positive or negative, and Pac-Man loops it to be within the indices of a collection with <paramref name="length"/> elements.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// LoopIndex(-4, 3); // => 1
    /// LoopIndex(-3, 3); // => 0
    /// LoopIndex(-2, 3); // => 1
    /// LoopIndex(-1, 3); // => 2
    ///  
    /// LoopIndex( 0, 3); // => 0
    ///  
    /// LoopIndex( 1, 3); // => 1
    /// LoopIndex( 2, 3); // => 2
    /// LoopIndex( 3, 3); // => 0
    /// LoopIndex( 4, 3); // => 1
    /// LoopIndex( 5, 3); // => 2
    /// LoopIndex( 6, 3); // => 0
    ///  
    /// LoopIndex(int.MaxValue, 3); // => 1
    /// ]]></code>
    /// </example>
    /// <param name="index">the index that you want to loop around to keep inside of <paramref name="length"/></param>
    /// <param name="length">the size of the actual collection</param>
    /// <returns>a value in <c>0..length</c></returns>
    internal static int LoopIndex(int index, int length)
    {
        Debug.Assert(length > 0,
            "Can't loop an index through a collection with length 0, because that would be INFINITE");

        index %= length;
        if (index < 0)
        {
            return length - ~index - 1;
        }

        return index;
    }

    /// <summary>
    /// Records a new <see cref="History{T}"/> entry.
    /// <br/>
    /// The new <paramref name="entry"/> will become my <see cref="Newest"/> element, at index <c>0</c>.
    /// <br/>
    /// If I'm already at <see cref="Capacity"/>, then my <see cref="Oldest"/> entry will be <paramref name="evicted"/>.
    /// </summary>
    /// <param name="entry">the new <see cref="Newest"/> entry</param>
    /// <param name="evicted">the entry that got removed, if I was already at <see cref="Capacity"/></param>
    /// <returns><c>true</c> if I was already at <see cref="Capacity"/> and someone got <paramref name="evicted"/></returns>
    public bool Record(T entry, [MaybeNullWhen(false)] out T evicted)
    {
        if (Count == Capacity)
        {
            evicted                        = Oldest;
            _storage[OldestIndexInStorage] = entry;
            OffsetInStorage += 1;
            return true;
        }

        evicted                        =  default;
        Count                          += 1;
        _storage[NewestIndexInStorage] =  entry;
        return false;
    }

    /// <summary>
    /// <inheritdoc cref="Record(T,out T)"/>
    /// </summary>
    /// <param name="entry"><inheritdoc cref="Record(T,out T)"/></param>
    public void Record(T entry) => Record(entry, out _);

    /// <summary>
    /// Enumerates through my entries <b><i>from <see cref="History{T}.Newest"/> to <see cref="History{T}.Oldest"/></i></b>.
    /// </summary>
    /// <remarks>
    /// If my <see cref="History{T}"/> gets modified while the <see cref="Enumerator"/> is in use, you're gunna have a bad time.
    /// If you want to be extra safe, make an immutable snapshot of the <see cref="History{T}"/> using <see cref="ImmutableArray.ToImmutableArray{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>.
    /// <br/>
    /// <br/>
    /// 📎 Note that this behavior matches that of <see cref="ConcurrentDictionary{TKey,TValue}"/>'s <see cref="ConcurrentDictionary{TKey,TValue}.GetEnumerator"/>, etc.
    /// </remarks>
    public Enumerator GetEnumerator() => new(this);

    /// <remarks>
    /// Interestingly, the javadocs for <see cref="ConcurrentStack{T}"/>.<see cref="ConcurrentStack{T}.GetEnumerator"/> note that the usage of <c>yield return</c>
    /// causes their generated <see cref="IEnumerator{T}"/> to contain a "snapshot" of the contents.
    /// This method does not do that.
    /// <br/>
    /// I prefer it that way, so that it matches the behavior of the <c>ref struct</c> <see cref="GetEnumerator"/>,
    /// but I wonder what's actually causing <see cref="ConcurrentStack{T}.GetEnumerator"/> to be a snapshot?
    /// </remarks>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

    /// <inheritdoc cref="History{T}.GetEnumerator"/>
    public struct Enumerator(History<T> history)
    {
        private int _position = -1;
        public  T   Current => history[_position];

        public bool MoveNext()
        {
            _position += 1;
            return _position < history.Count;
        }
    }

    /// <summary>
    /// <see cref="Array.Clear(System.Array)"/>s my <see cref="_storage"/> and resets my <see cref="Count"/>.
    /// </summary>
    public void Clear()
    {
        Array.Clear(_storage);
        OffsetInStorage = 0;
        Count           = 0;
    }
}