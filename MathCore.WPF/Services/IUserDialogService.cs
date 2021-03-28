using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MathCore.WPF.Services
{
    public interface IUserDialogBase
    {
        FileInfo? OpenFile(string Title, string Filter = "Все файлы (*.*)|*.*", string? DefaultFilePath = null);
        FileInfo? SaveFile(string Title, string Filter = "Все файлы (*.*)|*.*", string? DefaultFilePath = null);

        bool YesNoQuestion(string Text, string Title = "Вопрос...");
        bool OkCancelQuestion(string Text, string Title = "Вопрос...");
        void Information(string Text, string Title = "Вопрос...");
        void Warning(string Text, string Title = "Вопрос...");
        void Error(string Text, string Title = "Вопрос...");
    }
}
