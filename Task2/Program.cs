using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    public class DataProcessor
    {
        private const int DelayMilliseconds = 3000;

        // Синхронная имитация задачи
        public string ProcessData(string dataName)
        {
            Console.WriteLine($"Начата обработка (синхронно): {dataName}");
            Thread.Sleep(DelayMilliseconds); // блокирующая задержка
            return $"Обработка '{dataName}' завершена за {DelayMilliseconds / 1000} секунды (синхронно)";
        }

        // Асинхронная имитация задачи
        public async Task<string> ProcessDataAsync(string dataName)
        {
            Console.WriteLine($"Начата обработка (асинхронно): {dataName}");
            await Task.Delay(DelayMilliseconds); // не блокирует поток
            return $"Обработка '{dataName}' завершена за {DelayMilliseconds / 1000} секунды (асинхронно)";
        }
    }

    class Program
    {
        static async Task Main()
        {
            var processor = new DataProcessor();
            var dataFiles = new[] { "Файл 1", "Файл 2", "Файл 3" };

            Console.WriteLine("Синхронная обработка");
            var swSync = Stopwatch.StartNew();
            foreach (var file in dataFiles)
            {
                string result = processor.ProcessData(file);
                Console.WriteLine(result);
            }
            swSync.Stop();
            double syncTime = swSync.Elapsed.TotalSeconds;
            Console.WriteLine($"Синхронная обработка заняла: {syncTime:F2} сек.\n");

            Console.WriteLine("Асинхронная обработка");
            var swAsync = Stopwatch.StartNew();

            var tasks = new List<Task<string>>();
            foreach (var file in dataFiles)
            {
                // Запускаем все задачи одновременно
                tasks.Add(processor.ProcessDataAsync(file));
            }

            // Дожидаемся завершения всех и выводим результаты по мере готовности
            while (tasks.Count > 0)
            {
                Task<string> finished = await Task.WhenAny(tasks);
                tasks.Remove(finished);
                Console.WriteLine(await finished);
            }

            swAsync.Stop();
            double asyncTime = swAsync.Elapsed.TotalSeconds;
            Console.WriteLine($"Асинхронная обработка заняла: {asyncTime:F2} сек.\n");

            double ratio = syncTime / asyncTime;
            Console.WriteLine($"Разница во времени: асинхронная версия примерно в {ratio:F2} раза быстрее.");
        }
    }
}
