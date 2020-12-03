﻿using System;
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
        public ManualResetEvent workHandler = new ManualResetEvent(false);
        IResultOutput resultOutput;

        public Recognizer(IResultOutput outpObj)
        {
            resultOutput = outpObj;
        }

        private ImageInfo Proceed(Tuple<byte[], string> pair)
        {
            const int TargetWidth = 28;
            const int TargetHeight = 28;
            
            var image = Image.Load<Rgb24>(pair.Item1);

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

            //outputHandler.WaitOne();
            // Выдаем 10 наиболее вероятных результатов на экран
            //result.Add("Thread id = " + Thread.CurrentThread.ManagedThreadId.ToString());
            int j = 0;
            var result = new ResultStruct();
            result.filename = pair.Item2;
            result.picture = pair.Item1;
            float max = 0;
            foreach (var p in softmax
                .Select((x, i) => new { Label = classLabels[i], Confidence = x })
                /*.OrderByDescending(x => x.Confidence)*/
                .Take(10))
            {
                //result.confidence[j] = p.Confidence;
                if (p.Confidence > max)
                {
                    max = p.Confidence;
                    result.res_digit = j;
                }
                j++;
            }
            
            resultOutput.SendResult(result);  
            return new ImageInfo { filename = result.filename, res_digit = result.res_digit };
        }


        public ImageInfo GetResults(Tuple<byte[], string> picture)
        {
            return Proceed(picture);            
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
