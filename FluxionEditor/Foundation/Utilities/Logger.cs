using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FluxionEditor.Foundation.Utilities
{
    public enum SeverityLevel
    {
        Info    = 0x01,
        Warning = 0x02,
        Error   = 0x04,
        All     = Info | Warning | Error
    }

    public class MessageLog
    {
        public DateTime Time { get; }
        public SeverityLevel Level { get; }
        public string Log { get; }
        public string File { get; }
        public string Caller { get; }
        public int Line { get; }
        public string MetaData => $"{File}:{Caller}:{Line}";

        public MessageLog(SeverityLevel level, string log, string file, string caller, int line)
        {
            Time = DateTime.Now;
            Level = level;
            Log = log;
            File = System.IO.Path.GetFileName(file);
            Caller = caller;
            Line = line;
        }
    }

    /// <summary>
    /// Static, thread-safe logger with bitmask filtering and text search.
    /// Use <c>{x:Static utilities:Logger.FilteredMessages}</c> for UI binding.
    /// </summary>
    public static class Logger
    {
        // ── Messages ──

        private static readonly ObservableCollection<MessageLog> _messages = new();
        public static ReadOnlyObservableCollection<MessageLog> Messages { get; }
            = new ReadOnlyObservableCollection<MessageLog>(_messages);

        // ── Filters ──

        private static string _filterText = string.Empty;
        private static int _messageFilter = (int)SeverityLevel.All;
        private static Func<MessageLog, bool>? _customFilter;

        /// <summary>Fires when filter settings change so the UI can refresh.</summary>
        public static event Action? FiltersChanged;

        /// <summary>
        /// WPF-style filter event. Register a callback that returns true for
        /// messages that should be visible. Set to null to clear.
        /// </summary>
        public static event Func<MessageLog, bool>? Filter
        {
            add
            {
                _customFilter += value;
                FiltersChanged?.Invoke();
            }
            remove
            {
                _customFilter -= value;
                FiltersChanged?.Invoke();
            }
        }

        static Logger()
        {
            // Default filter: use bitmask to include only matching severity levels
            Filter += m => ((int)m.Level & _messageFilter) != 0;
        }

        public static string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    FiltersChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Bitmask filter for message types.
        /// Combine with |: <c>MessageFilter = (int)(SeverityLevel.Warning | SeverityLevel.Error)</c>.
        /// </summary>
        public static int MessageFilter
        {
            get => _messageFilter;
            set
            {
                if (_messageFilter != value)
                {
                    _messageFilter = value;
                    FiltersChanged?.Invoke();
                }
            }
        }

        /// <summary>Convenience: sets filter by enum instead of raw int.</summary>
        public static void SetMessageFilter(SeverityLevel mask)
        {
            MessageFilter = (int)mask;
        }

        /// <summary>
        /// Filtered view of <see cref="Messages"/>. Applies text search,
        /// bitmask filter, and any registered <see cref="Filter"/> callbacks.
        /// </summary>
        public static IEnumerable<MessageLog> FilteredMessages
        {
            get
            {
                var filtered = Messages.AsEnumerable();

                // Apply registered filter callbacks (WPF CollectionViewSource style)
                if (_customFilter != null)
                {
                    foreach (Func<MessageLog, bool> filter in _customFilter.GetInvocationList())
                        filtered = filtered.Where(filter);
                }

                // Text search
                if (!string.IsNullOrWhiteSpace(FilterText))
                    filtered = filtered.Where(m =>
                        m.Log.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                        m.MetaData.Contains(FilterText, StringComparison.OrdinalIgnoreCase));

                return filtered;
            }
        }

        // ── Logging methods ──

        public static async void Log(SeverityLevel type, string message,
            [CallerFilePath] string file = "",
            [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0)
        {
#if DEBUG
            // Also write to Visual Studio / Rider output window in debug builds
            System.Diagnostics.Debug.WriteLine($"[{type}] {message} ({System.IO.Path.GetFileName(file)}:{caller}:{line})");
#endif
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                _messages.Add(new MessageLog(type, message, file, caller, line));
            });
        }

        public static async void Clear()
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                _messages.Clear();
            });
        }
    }
}
