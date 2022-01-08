using System;
using System.IO;
using System.Threading.Tasks;

namespace ET
{
    class LearnAwait
    {
        #region 一
        public void Invoke()
        {
            Log.Debug("1111111111");
            //ReadFile(); // 1
            ReadFileAsync(); // 2
            Log.Debug("2222222222");
        }

        /// <summary>
        /// 1
        /// </summary>
        public void ReadFile()
        {
            string content = File.ReadAllText("test.txt");
            Log.Debug(content);
        }

        /// <summary>
        /// 2
        /// </summary>
        public async void ReadFileAsync()
        {
            string content = await File.ReadAllTextAsync("test.txt");
            Log.Debug(content);
        }

        /// <summary>
        /// 2编译后的样子
        /// </summary>
        public void ReadFileAsyncCLR()
        {
            var awaiter = File.ReadAllTextAsync("test.txt").GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                string content = awaiter.GetResult();
                Log.Debug(content);
            });
        }
        #endregion

        #region 二
        async static void AsyncTestFunction()
        {
            await Task.Delay(1);
            for (int i = 0; i < 10; i++)
            {
                Log.Debug($"AsyncFunction:i={i}");
            }
        }

        public static void Main()
        {
            AsyncTestFunction();

            for (int i = 0; i < 10; i++)
            {
                Log.Debug($"Main:i={i}");
            }
            Console.ReadLine();
        }
        #endregion
    }
}
