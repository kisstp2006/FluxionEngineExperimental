using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace FluxionEditor
{
    internal static class MessageBox
    {
        public static Task Info(
            string message,
            string title = "Info",
            Window? owner = null)
        {
            return Show(title, message, Icon.Info, ButtonEnum.Ok, owner);
        }

        public static Task Success(
            string message,
            string title = "Success",
            Window? owner = null)
        {
            return Show(title, message, Icon.Success, ButtonEnum.Ok, owner);
        }

        public static Task Warning(
            string message,
            string title = "Warning",
            Window? owner = null)
        {
            return Show(title, message, Icon.Warning, ButtonEnum.Ok, owner);
        }

        public static Task Error(
            string message,
            string title = "Error",
            Window? owner = null)
        {
            return Show(title, message, Icon.Error, ButtonEnum.Ok, owner);
        }

        public static Task Error(
            Exception exception,
            string title = "Error",
            Window? owner = null)
        {
            string message = exception.Message;

#if DEBUG
            message += "\n\n" + exception;
#endif

            return Show(title, message, Icon.Error, ButtonEnum.Ok, owner);
        }

        public static async Task<bool> Question(
            string message,
            string title = "Question",
            Window? owner = null)
        {
            var result = await Show(title, message, Icon.Question, ButtonEnum.YesNo, owner);
            return result == ButtonResult.Yes;
        }

        public static async Task<bool> Confirm(
            string message,
            string title = "Confirm",
            Window? owner = null)
        {
            var result = await Show(title, message, Icon.Question, ButtonEnum.YesNo, owner);
            return result == ButtonResult.Yes;
        }

        private static async Task<ButtonResult> Show(
            string title,
            string message,
            Icon icon,
            ButtonEnum buttons,
            Window? owner)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                title,
                message,
                buttons,
                icon);

            if (owner != null)
                return await box.ShowWindowDialogAsync(owner);

            return await box.ShowAsync();
        }
    }
}