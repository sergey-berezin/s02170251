using System;
using SixLabors.ImageSharp; // Из одноимённого пакета NuGet
using SixLabors.ImageSharp.PixelFormats;
using System.Linq;
using SixLabors.ImageSharp.Processing;
using Microsoft.ML.OnnxRuntime;
using System.Collections.Generic;
using System.Numerics.Tensors;
using System.IO;
using System.Threading;

namespace DigitRecognition
{
    public class Recognizer
    {
        AutoResetEvent outputHandler = new AutoResetEvent(true);
        public AutoResetEvent workHandler;

        List<String> result = new List<String>();

        public Recognizer (ref AutoResetEvent wH )
        {
            workHandler = wH;
        }

        private void Proceed(object arg)
        {
            var imageList = (List<Image<Rgb24>>)arg;

            const int TargetWidth = 28;
            const int TargetHeight = 28;

            foreach (var image in imageList)
            {
                if (workHandler.Equals(true))
                {
                    Console.WriteLine("break");
                    break;
                }
                

                // Изменяем размер картинки до 224 x 224
                image.Mutate(x =>
                {
                    x.Resize(new ResizeOptions
                    {
                        Size = new Size(TargetWidth, TargetHeight),
                        Mode = ResizeMode.Crop // Сохраняем пропорции обрезая лишнее (по левому верхнему углу) 
                    });
                });

                // Перевод пикселов в тензор и нормализация
                var input = new DenseTensor<float>(new[] { 1, 1, TargetHeight, TargetWidth }); //тройка тк три матрицы на каждый из цветов

                for (int y = 0; y < TargetHeight; y++)
                {
                    Span<Rgb24> pixelSpan = image.GetPixelRowSpan(y);
                    for (int x = 0; x < TargetWidth; x++)
                    {
                        input[0, 0, y, x] = ((pixelSpan[x].R + pixelSpan[x].G + pixelSpan[x].B) / (float)3.0) / 255f; // сначала приводим в [0,1], а потом производим z-нормализацию                    
                    }
                }

                // Подготавливаем входные данные нейросети. Имя input задано в файле модели
                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("Input3", input)
                };

                // Вычисляем предсказание нейросетью
                var session = new InferenceSession(@"D:\7_sem_prac\Lab_1\Lab_1\mnist-8.onnx");
                IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);

                // Получаем 1000 выходов и считаем для них softmax
                var output = results.First().AsEnumerable<float>().ToArray();
                var sum = output.Sum(x => (float)Math.Exp(x));
                var softmax = output.Select(x => (float)Math.Exp(x) / sum);

                //Console.WriteLine("this thread works");

                outputHandler.WaitOne();
                // Выдаем 10 наиболее вероятных результатов на экран
                result.Add("Thread id = " + Thread.CurrentThread.ManagedThreadId.ToString());
                foreach (var p in softmax
                    .Select((x, i) => new { Label = classLabels[i], Confidence = x })
                    .OrderByDescending(x => x.Confidence)
                    .Take(1))
                {
                    result.Add($"{p.Label} with confidence {p.Confidence}");
                }
                result.Add("");
                outputHandler.Set();

                Thread.Sleep(100);
            }
        }

        List<Thread> threads = new List<Thread>();

        public List<String> GetResults(string arg)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(arg);
            List<Image<Rgb24>> myList = new List<Image<Rgb24>>();
            foreach (var file in directoryInfo.GetFiles()) //проходим по файлам
            {
                //получаем расширение файла и проверяем подходит ли оно нам                 
                myList.Add(Image.Load<Rgb24>(file.FullName));
            }
            

            Console.WriteLine(myList.Count);

            int numPrc = Environment.ProcessorCount;
            int picsPerThread = myList.Count / numPrc + 1;

            Console.WriteLine("Number of threads =" + numPrc.ToString() + "  | Images per thread = " + picsPerThread.ToString());

            for (int i = 0; i < numPrc; ++i)
            {
                Thread myThread = new Thread(new ParameterizedThreadStart(Proceed));
                //Console.WriteLine(myThread.ManagedThreadId.ToString());
                threads.Add(myThread);
                if (i != numPrc - 1)
                    myThread.Start(myList.GetRange(i * picsPerThread, picsPerThread));
                else
                    myThread.Start(myList.GetRange(i * picsPerThread, myList.Count - i * picsPerThread));
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return result;
        }

       

        static readonly string[] classLabels = new[]
        {
            "zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine"
        };
    }
}
