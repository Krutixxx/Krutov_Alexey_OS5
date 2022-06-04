using System;
using System.Threading;
using System.Diagnostics;

namespace Krutov_OS5
{
    class Threader
    {
        public Thread thread;
        public bool shutDown;
        public bool isSleep;
        public int thrPriority;
    }

    class Producer : Threader
    {
        int ProducerNumber;
        Random randomNubmer = new Random();
        int counter;
        public Producer(int num)
        {
            ProducerNumber = num;
            isSleep = true;
            shutDown = false;
            thrPriority = 3;
            counter = randomNubmer.Next(1, 10);
            thread = new Thread(Сounter);
            thread.Start();
        }
        private void Сounter()
        {
        prod:
            if (counter < 100)
            {
                if (!isSleep)
                {
                    Console.WriteLine($"Поток №{ProducerNumber} получил число: {counter}");
                    counter++;
                    Thread.Sleep(2000);
                }
                goto prod;
            }
            else
            {
                Console.WriteLine($"Поток №{ProducerNumber} завершил свою работу");
                shutDown = true;
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }

    class Program
    {

        public static int buttonSwitch(ConsoleKeyInfo key)
        {
            int temp;
            switch (key.Key.ToString())
            {
                case "A":
                    temp = 0;
                    break;
                case "S":
                    temp = 1;
                    break;
                case "D":
                    temp = 2;
                    break;
                default:
                    temp = 3;
                    break;
            }
            return temp;
        }

        public static ThreadPriority threadPriorityChanchePlus(int priorityNumber, int threadNumber)
        {
            ThreadPriority priority;
            switch (priorityNumber)
            {
                case 1:
                    priority = ThreadPriority.BelowNormal;
                    Console.WriteLine($"\nПриоритет потока №{threadNumber} повышен.");
                    break;
                case 2:
                    priority = ThreadPriority.Normal;
                    Console.WriteLine($"\nПриоритет потока №{threadNumber} повышен.");
                    break;
                case 3:
                    priority = ThreadPriority.AboveNormal;
                    Console.WriteLine($"\nПриоритет потока №{threadNumber} повышен.");
                    break;
                case 4:
                    priority = ThreadPriority.Highest;
                    Console.WriteLine($"\nПриоритет потока №{threadNumber} повышен.");
                    break;
                case 5:
                    priority = ThreadPriority.Highest;
                    Console.WriteLine($"\nПриоритет потока максимален.");
                    break;
                default:
                    priority = ThreadPriority.Highest;
                    break;
            }
            return priority;
        }

        public static ThreadPriority threadPriorityChancheMinus(int priorityNumber, int threadNumber)
        {
            ThreadPriority priority;
            switch (priorityNumber)
            {
                case 1:
                    priority = ThreadPriority.Lowest;
                    Console.WriteLine($"\nПриоритет потока минимален.");
                    break;
                case 2:
                    priority = ThreadPriority.Lowest;
                    Console.WriteLine($"\nПриоритет потока №{threadNumber} понижен.");
                    break;
                case 3:
                    priority = ThreadPriority.BelowNormal;
                    Console.WriteLine($"\nПриоритет потока №{threadNumber} понижен.");
                    break;
                case 4:
                    priority = ThreadPriority.Normal;
                    Console.WriteLine($"\nПриоритет потока №{threadNumber} понижен.");
                    break;
                case 5:
                    priority = ThreadPriority.AboveNormal;
                    Console.WriteLine($"\nПриоритет потока №{threadNumber} понижен.");
                    break;
                default:
                    priority = ThreadPriority.Lowest;
                    break;
            }
            return priority;
        }

        public static int ThreadWorkStart(int num, Threader[] Producers)
        {
            int counter = 0;
            while (true)
            {
                if (counter == 3)
                {
                    Console.WriteLine("\nВсе потоки завершили свою работу.");
                    for (int i = 0; i < 3; i++)
                    {
                        Producers[i].thread.Interrupt();
                    }
                }
                else
                {
                    if (Producers[num].shutDown == true)
                    {
                        counter++;
                        num = (num + 1) % 3;
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"Поток №{num + 1} возобновил работу.");
                        Producers[num].isSleep = false;
                        return num;
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Threader[] Producers =
            {
                new Producer(1),
                new Producer(2),
                new Producer(3)
            };

            Console.WriteLine("Выберете режим работы (1/2): ");
            string number = Console.ReadLine();
            switch (number)
            {
                case "1":
                    int num = -1;
                    int counter = 0;
                    var sw = new Stopwatch();
                    while (true)
                    {
                        sw.Start();
                        ConsoleKey key = ConsoleKey.Enter;
                        if (sw.ElapsedMilliseconds <= 10000)
                        {
                            if (Console.KeyAvailable)
                            {
                                key = Console.ReadKey(true).Key;
                            }
                            else
                            {
                                key = ConsoleKey.LeftArrow;
                            }
                            if (key.ToString() == "Z")
                            {
                                sw.Restart();
                                if (Producers[(num + 2) % 3].shutDown == true & Producers[(num + 1) % 3].shutDown == true)
                                {
                                    if (Producers[num].isSleep == true)
                                    {
                                        Console.WriteLine($"\nПоток №{num + 1} возобновил работу.");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"\nПоток №{num + 1} приостановлен.");
                                    }
                                    Producers[num].isSleep = !Producers[num].isSleep;
                                }
                                else
                                {
                                    if (counter != 0)
                                    {
                                        if (Producers[num].shutDown != true)
                                        {
                                            Console.WriteLine($"\nПоток №{num + 1} приостановлен.");
                                            Producers[num].isSleep = true;
                                        }
                                    }
                                    counter++;
                                    num = (ThreadWorkStart((num + 1) % 3, Producers));
                                }
                            }
                            if (key.ToString() == "A")
                            {
                                if (Producers[num].shutDown == false)
                                {
                                    Producers[num].thread.Priority = threadPriorityChanchePlus(Producers[num].thrPriority, num + 1);
                                    if (Producers[num].thrPriority != 5)
                                    {
                                        Producers[num].thrPriority++;
                                    }
                                }
                            }

                            if (key.ToString() == "S")
                            {
                                if (Producers[num].shutDown == false)
                                {
                                    Producers[num].thread.Priority = threadPriorityChancheMinus(Producers[num].thrPriority, num + 1);
                                    if (Producers[num].thrPriority != 5)
                                    {
                                        Producers[num].thrPriority--;
                                    }
                                }
                            }

                            if (key.ToString() == "Q")
                            {
                                Console.WriteLine("\nЗавершение работы программы.");
                                for (int i = 0; i < 3; i++)
                                {
                                    Producers[i].thread.Interrupt();
                                }
                                break;
                            }
                        }
                        else
                        {
                            sw.Restart();
                            if (Producers[(num + 2) % 3].shutDown == true & Producers[(num + 1) % 3].shutDown == true)
                            {
                                if (Producers[num].isSleep == true)
                                {
                                    Console.WriteLine($"\nПоток №{num + 1} возобновил работу.");
                                }
                                else
                                {
                                    Console.WriteLine($"\nПоток №{num + 1} приостановлен.");
                                }
                                Producers[num].isSleep = !Producers[num].isSleep;
                            }
                            else
                            {
                                if (counter != 0)
                                {
                                    if (Producers[num].shutDown != true)
                                    {
                                        Console.WriteLine($"\nПоток №{num + 1} приостановлен.");
                                        Producers[num].isSleep = true;
                                    }
                                }
                                counter++;
                                num = (ThreadWorkStart((num + 1) % 3, Producers));
                            }
                        }
                    }
                    break;
                case "2":
                    Console.WriteLine("Нажмите клавишу 'P' для активации потоков.");
                    while (true)
                    {
                        ConsoleKeyInfo key = Console.ReadKey();

                        bool threadNumberChecker = (key.Key.ToString() == "A") | (key.Key.ToString() == "S") | (key.Key.ToString() == "D");

                        if (key.Key.ToString() == "Q")
                        {
                            Console.WriteLine("\nЗавершение работы программы.");
                            for (int i = 0; i < 3; i++)
                            {
                                Producers[i].thread.Interrupt();
                            }
                            break;
                        }

                        if (((key.Modifiers & ConsoleModifiers.Shift) != 0) & threadNumberChecker)
                        {
                            int temp = buttonSwitch(key);
                            if (temp != 3 & Producers[temp].shutDown == false)
                            {
                                Producers[temp].thread.Priority = threadPriorityChanchePlus(Producers[temp].thrPriority, temp + 1);
                                if (Producers[temp].thrPriority != 5)
                                {
                                    Producers[temp].thrPriority++;
                                }
                            }
                        }

                        if (((key.Modifiers & ConsoleModifiers.Control) != 0) & threadNumberChecker)
                        {
                            int temp = buttonSwitch(key);
                            if (temp != 3 & Producers[temp].shutDown == false)
                            {
                                Producers[temp].thread.Priority = threadPriorityChancheMinus(Producers[temp].thrPriority, temp + 1);
                                if (Producers[temp].thrPriority != 1)
                                {
                                    Producers[temp].thrPriority--;
                                }
                            }
                        }

                        if (((key.Modifiers & ConsoleModifiers.Alt) != 0) & threadNumberChecker)
                        {
                            int temp = buttonSwitch(key);
                            if (temp != 3 & Producers[temp].shutDown == false)
                            {
                                Producers[temp].isSleep = !Producers[temp].isSleep;
                                if (Producers[temp].isSleep)
                                {
                                    Console.WriteLine($"\nПоток №{temp + 1} приостановлен.");
                                }
                                else
                                {
                                    Console.WriteLine($"\nПоток №{temp + 1} возобновлён.");
                                }
                            }
                        }

                        if (key.Key.ToString() == "P")
                        {
                            Console.WriteLine("\n");
                            for (int i = 0; i < 3; i++)
                            {
                                Producers[i].isSleep = false;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}