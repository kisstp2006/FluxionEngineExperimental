using Avalonia;
using Avalonia.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Tmds.DBus.Protocol;

namespace FluxionEditor.Foundation.Utilities
{
    enum SeverityLevel
    {
        Info = 0x01,
        Warning =0x02,
        Error=0x04
    }

    class MessageLog
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
            Log = log;
            File = Path.GetFileName(file);
            Caller = caller;
            Line = line;
        }
    }
    internal class Logger
    {
        private readonly static ObservableCollection<MessageLog> _messages = new ObservableCollection<MessageLog>();
        public static ReadOnlyObservableCollection<MessageLog> Messages { get; } = new ReadOnlyObservableCollection<MessageLog>(_messages);

        public static async void Log(SeverityLevel type, string message, [CallerFilePath]string file = "", [CallerMemberName]string caller ="", [CallerLineNumber] int line =0)
        {
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

        public static 
    }
}
