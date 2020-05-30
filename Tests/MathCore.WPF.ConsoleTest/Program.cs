using System;
using System.Threading.Tasks;
using NAudio.Wave;

namespace MathCore.WPF.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            const string wav_file = "test.wav";

            using (var input = new WaveInEvent { DeviceNumber = 1 })
            using (var writer = new WaveFileWriter(wav_file, input.WaveFormat))
            {
                long total_count = 0;
                input.DataAvailable += async (s, e) =>
                {
                    total_count += e.BytesRecorded;
                    await writer.WriteAsync(e.Buffer, 0, e.BytesRecorded).ConfigureAwait(false);
                    Console.WriteLine("Received {0} bytes, total {1}", e.BytesRecorded, total_count);
                };

                Console.WriteLine("Ready to record.");
                Console.ReadLine();

                input.StartRecording();

                Console.WriteLine("Rocording...");
                Console.ReadLine();

                input.StopRecording();
            }

            Console.WriteLine("Ready to play.");
            Console.ReadLine();

            using (var file = new AudioFileReader(wav_file))
            using (var output = new WaveOutEvent())
            {
                output.Init(file);

                output.Play();
                var play_completed_event = new TaskCompletionSource<object?>();
                output.PlaybackStopped += (_, __) => play_completed_event.SetResult(null);
                
                Console.WriteLine("Playing...");
                Task.WaitAny(play_completed_event.Task, Task.Run(Console.ReadLine));
            }

            Console.WriteLine("Complete!");
            Console.ReadLine();
        }
    }
}
