using System;
using System.ComponentModel;
using Tortuga.Chain.Core;
using System.Collections.Concurrent;

#if !WINDOWS_UWP
using System.Runtime.Caching;
#endif

namespace Tortuga.Chain.DataSources
{
    /// <summary>
    /// Class DataSource.
    /// </summary>
    public abstract class DataSource
    {

        /// <summary>
        /// 
        /// </summary>
        protected DataSource()
        {
#if !WINDOWS_UWP
            Cache = MemoryCache.Default;
#endif
        }

        /// <summary>
        /// Raised when a executionDetails is canceled in any dispatcher.
        /// </summary>
        /// <remarks>This is not used for timeouts.</remarks>
        public static event EventHandler<ExecutionEventArgs> GlobalExecutionCanceled;

        /// <summary>
        /// Raised when a procedure call fails in any dispatcher.
        /// </summary>
        public static event EventHandler<ExecutionEventArgs> GlobalExecutionError;

        /// <summary>
        /// Raised when a procedure call is successfully completed in any dispatcher
        /// </summary>
        public static event EventHandler<ExecutionEventArgs> GlobalExecutionFinished;

        /// <summary>
        /// Raised when a procedure call is started in any dispatcher
        /// </summary>
        public static event EventHandler<ExecutionEventArgs> GlobalExecutionStarted;

        /// <summary>
        /// Raised when a executionDetails is canceled.
        /// </summary>
        /// <remarks>This is not used for timeouts.</remarks>
        public event EventHandler<ExecutionEventArgs> ExecutionCanceled;

        /// <summary>
        /// Raised when a procedure call fails.
        /// </summary>
        public event EventHandler<ExecutionEventArgs> ExecutionError;

        /// <summary>
        /// Raised when a procedure call is successfully completed
        /// </summary>
        public event EventHandler<ExecutionEventArgs> ExecutionFinished;

        /// <summary>
        /// Raised when a procedure call is started
        /// </summary>
        public event EventHandler<ExecutionEventArgs> ExecutionStarted;

#if !WINDOWS_UWP
        /// <summary>
        /// Gets or sets the cache to be used by this data source. The default is .NET's MemoryCache.
        /// </summary>
        /// <remarks>This is used by the WithCaching materializer.</remarks>
        internal ObjectCache Cache { get; set; }
#endif

        /// <summary>
        /// Gets or sets the default command timeout.
        /// </summary>
        /// <value>The default command timeout.</value>
        public TimeSpan? DefaultCommandTimeout { get; set; }

        /// <summary>
        /// Gets the name of the data source.
        /// </summary>
        /// <value>
        /// The name of the data source.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether strict mode is enabled.
        /// </summary>
        /// <remarks>Strict mode requires all properties that don't represent columns to be marked with the NotMapped attribute.</remarks>
        public bool StrictMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to suppress global events.
        /// </summary>
        /// <value>If <c>true</c>, this data source will not honor global event handlers.</value>
        public bool SuppressGlobalEvents { get; set; }

#if !WINDOWS_UWP
        /// <summary>
        /// Invalidates a cache key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        internal void InvalidateCache(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentException("cacheKey is null or empty.", "cacheKey");

            Cache.Remove(cacheKey, null);
        }
#endif

        /// <summary>
        /// Raises the <see cref="E:ExecutionCanceled" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnExecutionCanceled(ExecutionEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e", "e is null.");

            if (ExecutionCanceled != null)
                ExecutionCanceled(this, e);
            if (!SuppressGlobalEvents && GlobalExecutionCanceled != null)
                GlobalExecutionCanceled(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:ExecutionError" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnExecutionError(ExecutionEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e", "e is null.");

            if (ExecutionError != null)
                ExecutionError(this, e);
            if (!SuppressGlobalEvents && GlobalExecutionError != null)
                GlobalExecutionError(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:ExecutionFinished" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnExecutionFinished(ExecutionEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e", "e is null.");

            if (ExecutionFinished != null)
                ExecutionFinished(this, e);
            if (!SuppressGlobalEvents && GlobalExecutionFinished != null)
                GlobalExecutionFinished(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:ExecutionStarted" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnExecutionStarted(ExecutionEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e", "e is null.");

            if (ExecutionStarted != null)
                ExecutionStarted(this, e);
            if (!SuppressGlobalEvents && GlobalExecutionStarted != null)
                GlobalExecutionStarted(this, e);
        }

#if !WINDOWS_UWP
        /// <summary>
		/// Try to read from the cache.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cacheKey">The cache key.</param>
		/// <param name="result">The cached result.</param>
		/// <returns><c>true</c> if the key was found in the cache, <c>false</c> otherwise.</returns>
		/// <exception cref="ArgumentException">cacheKey is null or empty.;cacheKey</exception>
		/// <exception cref="InvalidOperationException">Cache is corrupted.</exception>
		internal bool TryReadFromCache<T>(string cacheKey, out T result)
        {
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentException("cacheKey is null or empty.", "cacheKey");

            var cacheItem = Cache.GetCacheItem(cacheKey, null);
            if (cacheItem == null)
            {
                result = default(T);
                return false;
            }

            //Nulls can't be stored in a cache, so we simulate it using NullObject.Default.
            if (cacheItem.Value == NullObject.Default)
            {
                result = default(T);
                return true;
            }

            if (!(cacheItem.Value is T))
                throw new InvalidOperationException($"Cache is corrupted. Cache Key \"{cacheKey}\" is a {cacheItem.Value.GetType().Name} not a {typeof(T).Name}");

            result = (T)cacheItem.Value;

            return true;
        }

        /// <summary>
        /// Writes to cache, replacing any previous value.
        /// </summary>
        /// <param name="item">The cache item.</param>
        /// <param name="policy">Optional cache invalidation policy.</param>
        /// <exception cref="ArgumentNullException">item;item is null.</exception>
        internal void WriteToCache(CacheItem item, CacheItemPolicy policy)
        {
            if (item == null)
                throw new ArgumentNullException("item", "item is null.");

            //Nulls can't be stored in a cache, so we simulate it using NullObject.Default.
            if (item.Value == null)
                item.Value = NullObject.Default;

            Cache.Set(item, policy);
        }
#endif

        /// <summary>
        /// Data sources can use this to indicate an executionDetails was canceled.
        /// </summary>
        /// <param name="executionDetails">The executionDetails.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <remarks>This is not used for timeouts.</remarks>
        protected void OnExecutionCanceled(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException("executionDetails", "executionDetails is null.");

            if (ExecutionCanceled != null)
                ExecutionCanceled(this, new ExecutionEventArgs(executionDetails, startTime, endTime, state));
            if (!SuppressGlobalEvents && GlobalExecutionCanceled != null)
                GlobalExecutionCanceled(this, new ExecutionEventArgs(executionDetails, startTime, endTime, state));
        }

        /// <summary>
        /// Data sources can use this to indicate an error has occurred.
        /// </summary>
        /// <param name="executionDetails">The executionDetails.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="error">The error.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        protected void OnExecutionError(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, Exception error, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException("executionDetails", "executionDetails is null.");
            if (error == null)
                throw new ArgumentNullException("error", "error is null.");

            if (ExecutionError != null)
                ExecutionError(this, new ExecutionEventArgs(executionDetails, startTime, endTime, error, state));
            if (!SuppressGlobalEvents && GlobalExecutionError != null)
                GlobalExecutionError(this, new ExecutionEventArgs(executionDetails, startTime, endTime, error, state));
        }

        /// <summary>
        /// Data sources can use this to indicate an executionDetails has finished.
        /// </summary>
        /// <param name="executionDetails">The executionDetails.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="rowsAffected">The number of rows affected.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        protected void OnExecutionFinished(ExecutionToken executionDetails, DateTimeOffset startTime, DateTimeOffset endTime, int? rowsAffected, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException("executionDetails", "executionDetails is null.");

            if (ExecutionFinished != null)
                ExecutionFinished(this, new ExecutionEventArgs(executionDetails, startTime, endTime, rowsAffected, state));
            if (!SuppressGlobalEvents && GlobalExecutionFinished != null)
                GlobalExecutionFinished(this, new ExecutionEventArgs(executionDetails, startTime, endTime, rowsAffected, state));
        }

        /// <summary>
        /// Data sources can use this to indicate an executionDetails has begun.
        /// </summary>
        /// <param name="executionDetails">The executionDetails.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        protected void OnExecutionStarted(ExecutionToken executionDetails, DateTimeOffset startTime, object state)
        {
            if (executionDetails == null)
                throw new ArgumentNullException("executionDetails", "executionDetails is null.");


            if (ExecutionStarted != null)
                ExecutionStarted(this, new ExecutionEventArgs(executionDetails, startTime, state));
            if (!SuppressGlobalEvents && GlobalExecutionStarted != null)
                GlobalExecutionStarted(this, new ExecutionEventArgs(executionDetails, startTime, state));
        }
#if !WINDOWS_UWP
        private class NullObject
        {
            public static readonly NullObject Default = new NullObject();

            private NullObject() { }
        }
#endif

        /// <summary>
        /// The extension cache is used by extensions to store data source specific informmation.
        /// </summary>
        /// <value>The extension cache.</value>
        private readonly ConcurrentDictionary<Type, object> m_ExtensionCache = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Gets the extension data.
        /// </summary>
        /// <typeparam name="TTKey">The type of extension data desired.</typeparam>
        /// <returns>T.</returns>
        /// <remarks>Chain extensions can use this to store data source specific data. The key should be a data type defined by the extension.
        /// 
        /// Transactional data sources should override this method and return the value held by their parent data source.</remarks>
        public virtual TTKey GetExtensionData<TTKey>()
            where TTKey : new()
        {
            return (TTKey)m_ExtensionCache.GetOrAdd(typeof(TTKey), x => new TTKey());
        }
    }
}