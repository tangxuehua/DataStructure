using System;
using System.Collections;

namespace NetFocus.DataStructure.Internal.Algorithm
{
	/// <summary>
	///     <para>
	///       A collection that stores <see cref="StatusItem"/> objects.
	///    </para>
	/// </summary>
	/// <seealso cref="StatusItemCollection"/>
	[Serializable()]
	public class StatusItemCollection : CollectionBase 
	{
		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref="StatusItemCollection"/>.
		///    </para>
		/// </summary>
		public StatusItemCollection() 
		{
		}
		
		/// <summary>
		/// <para>Represents the entry at the specified index of the <see cref="StatusItem"/>.</para>
		/// </summary>
		/// <param name="index"><para>The zero-based index of the entry to locate in the collection.</para></param>
		/// <value>
		///    <para> The entry at the specified index of the collection.</para>
		/// </value>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the valid range of indexes for the collection.</exception>
		public StatusItem this[int index] 
		{
			get 
			{
				return ((StatusItem)(List[index]));
			}
			set 
			{
				List[index] = value;
			}
		}
		
		/// <summary>
		///    <para>Adds a <see cref="StatusItem"/> with the specified value to the
		///    <see cref="StatusItemCollection"/> .</para>
		/// </summary>
		/// <param name="value">The <see cref="StatusItem"/> to add.</param>
		/// <returns>
		///    <para>The index at which the new element was inserted.</para>
		/// </returns>
		public int Add(StatusItem value) 
		{
			return List.Add(value);
		}
		
		/// <summary>
		/// <para>Gets a value indicating whether the
		///    <see cref="StatusItemCollection"/> contains the specified <see cref="StatusItem"/>.</para>
		/// </summary>
		/// <param name="value">The <see cref="StatusItem"/> to locate.</param>
		/// <returns>
		/// <para><see langword="true"/> if the <see cref="StatusItem"/> is contained in the collection;
		///   otherwise, <see langword="false"/>.</para>
		/// </returns>
		/// <seealso cref="StatusItemCollection.IndexOf"/>
		public bool Contains(StatusItem value) 
		{
			return List.Contains(value);
		}
		
		/// <summary>
		///    <para>Returns the index of a <see cref="StatusItem"/> in
		///       the <see cref="StatusItemCollection"/> .</para>
		/// </summary>
		/// <param name="value">The <see cref="StatusItem"/> to locate.</param>
		/// <returns>
		/// <para>The index of the <see cref="StatusItem"/> of <paramref name="value"/> in the
		/// <see cref="StatusItemCollection"/>, if found; otherwise, -1.</para>
		/// </returns>
		/// <seealso cref="StatusItemCollection.Contains"/>
		public int IndexOf(StatusItem value) 
		{
			return List.IndexOf(value);
		}
		
		/// <summary>
		///    <para>Returns an enumerator that can iterate through
		///       the <see cref="StatusItemCollection"/> .</para>
		/// </summary>
		/// <returns><para>None.</para></returns>
		/// <seealso cref="System.Collections.IEnumerator"/>
		public new StatusItemEnumerator GetEnumerator() 
		{
			return new StatusItemEnumerator(this);
		}
		
		/// <summary>
		///    <para> Removes a specific <see cref="StatusItem"/> from the
		///    <see cref="StatusItemCollection"/> .</para>
		/// </summary>
		/// <param name="value">The <see cref="StatusItem"/> to remove from the <see cref="StatusItemCollection"/> .</param>
		/// <returns><para>None.</para></returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not found in the Collection. </exception>
		public void Remove(StatusItem value) 
		{
			List.Remove(value);
		}
		
		
		/// <summary>
		/// Default enumerator.
		/// </summary>
		public class StatusItemEnumerator : object, IEnumerator 
		{
			IEnumerator baseEnumerator;
			IEnumerable temp;
			/// <summary>
			/// Creates a new instance.
			/// </summary>
			public StatusItemEnumerator(StatusItemCollection mappings) 
			{
				this.temp = (IEnumerable)mappings;
				this.baseEnumerator = temp.GetEnumerator();
			}
			
			/// <summary>
			/// Returns the current object.
			/// </summary>
			public StatusItem Current 
			{
				get 
				{
					return ((StatusItem)(baseEnumerator.Current));
				}
			}
			
			object IEnumerator.Current 
			{
				get 
				{
					return baseEnumerator.Current;
				}
			}
			
			/// <summary>
			/// Moves to the next object.
			/// </summary>
			public bool MoveNext() 
			{
				return baseEnumerator.MoveNext();
			}
			bool IEnumerator.MoveNext()
			{
				return baseEnumerator.MoveNext();
			}
			/// <summary>
			/// Resets this enumerator.
			/// </summary>
			public void Reset() 
			{
				baseEnumerator.Reset();
			}
			
			void IEnumerator.Reset() 
			{
				baseEnumerator.Reset();
			}
		}


	}
}
