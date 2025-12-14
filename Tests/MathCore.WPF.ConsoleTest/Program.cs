using System.Threading.Channels;

_ = Console.Out << "Bounded Channel Test" << Environment.NewLine;

var sh = Channel.CreateBounded<int>(new BoundedChannelOptions(10)
{
    AllowSynchronousContinuations = true,
    SingleReader = true,
    SingleWriter = true,
    FullMode = BoundedChannelFullMode.DropWrite
});
var writer = sh.Writer;
var reader = sh.Reader;

var processing_task = Task.Run(
    async () =>
    {
        await foreach (var v in reader.ReadAllAsync())
        {
            Console.WriteLine("    {0}", v);
            await Task.Delay(50);
        }
    });

for (var i = 0; i < 20; i++)
{
    Console.WriteLine("{0}", i);
    await writer.WriteAsync(i);
}

writer.Complete();
Console.WriteLine("Producer completed");

await sh.Reader.Completion;
Console.WriteLine("Reader completed");

await processing_task;

Console.WriteLine("Completed");
Console.ReadLine();

public static class TextWriterOperators
{
    extension(TextWriter writer) // блок расширения для TextWriter
    {
        // Запись строки и возврат того же TextWriter для цепочек
        public static TextWriter operator <<(TextWriter Writer, string Value)
        {
            Writer.Write(Value); // записать значение
            return Writer; // вернуть для цепочки
        }

        // Универсальный вариант для любых объектов
        public static TextWriter operator <<(TextWriter Writer, object? Value) => Writer << (Value?.ToString() ?? string.Empty);
    }
}