using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Adaline
{
    public partial class MainWindow : Window
    {
        public int train_amount = 60000;
        public int test_amount = 10000;
        public double learn_var = 0.0001;
        public double ERROR = 0;
        public int image_size = 28 * 28;
        public int maxRounds = 700000;
        public List<int> tmp = new List<int>() { 1 };
        public List<byte> train_labels;
        public List<byte> train_images;
        public List<byte> test_labels;
        public List<byte> test_images;
        public Random randW = new Random();
        public Random randE = new Random();
        public List<List<int>> train_examples = new List<List<int>>();
        public List<List<int>> test_examples = new List<List<int>>();
        public List<List<double>> units = new List<List<double>>();

        public MainWindow()
        {
            InitializeComponent();
            train_labels = new List<byte>(File.ReadAllBytes("train-labels.idx1-ubyte"));
            train_images = new List<byte>(File.ReadAllBytes("train-images.idx3-ubyte"));
            test_labels = new List<byte>(File.ReadAllBytes("test-labels.idx1-ubyte"));
            test_images = new List<byte>(File.ReadAllBytes("test-images.idx3-ubyte"));
            train_labels.RemoveRange(0, 8);
            test_labels.RemoveRange(0, 8);
            train_images.RemoveRange(0, 16);
            test_images.RemoveRange(0, 16);

            for (int i = 0; i < train_images.Count(); i++)
            {
                if(train_images[i] < 128)
                {
                    tmp.Add(1);
                }
                else
                {
                    tmp.Add(-1);
                }
                if((i + 1) % image_size == 0)
                {
                    train_examples.Add(tmp);
                    tmp = new List<int>() { 1 };
                }
                
            }
            tmp = new List<int>() { 1 };
            for (int i = 0; i < test_images.Count(); i++)
            {
                if (test_images[i] < 128)
                {
                    tmp.Add(1);
                }
                else
                {
                    tmp.Add(-1);
                }
                if ((i + 1) % image_size == 0)
                {
                    test_examples.Add(tmp);
                    tmp = new List<int>() { 1 };

                }
            }

            //for(int i = 0; i < train_amount; i += image_size)
            //{
            //    train_examples.Add(train_images.GetRange(i, image_size));
            //}
            //for (int i = 0; i < test_amount; i += image_size)
            //{
            //    test_examples.Add(test_images.GetRange(i, image_size));
            //}

            //for (int i = 0; i < image_size; i++)
            //{
            //    if (test_examples[2][i] < 128)
            //    {
            //        Debug.Write("-");
            //    }
            //    else
            //    {
            //        Debug.Write("X");
            //    }

            //    if ((i + 1) % 28 == 0)
            //    {
            //        Debug.WriteLine("");
            //    }
            //}
            //foreach (byte i in test_labels)
            //{
            //    Debug.Write(i + " ");
            //}
        }
        private void TestClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                Testing(i);
            }
            Debug.WriteLine(ERROR / 170 + " %");
        }
        private void Testing(int unit)
        {
            double O;
            int C;

            for(int k = 0; k < test_examples.Count(); k++)
            {
                O = 0;

                if (test_labels[k] == unit)
                {
                    C = 1;
                }
                else
                {
                    C = -1;
                }

                for (int i = 0; i < image_size + 1; i++)
                {
                    O += units[unit][i] * train_examples[k][i];
                }

                for (int j = 0; j < image_size + 1; j++)
                {
                    units[unit][j] += learn_var * (C - O) * train_examples[k][j];
                }

                if ((k + 1) % 100 == 0)
                {
                    ERROR += Math.Pow(O - C, 2);
                }
            }
        }
        private void TrainClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                Training(i);
            }
            Debug.WriteLine(ERROR / 70 + " %");
        }
        
        public List<double> Weights(List<double> w)
        {
            for (int i = 0; i < image_size + 1; i++)
            {
                w.Add(2.0 * randW.NextDouble() - 1.0);
            }
            w[0] = 1;
            return w;
        }

        public void Training(int unit)
        {
            List<double> w = new List<double>();
            w = Weights(w);

            int drawn;
            double O;
            int C;
            int round = 0;
            
            while (round < maxRounds)
            {
                drawn = randE.Next(train_examples.Count());
                O = 0;
                
                if(train_labels[drawn] == unit)
                {
                    C = 1;
                }
                else
                {
                    C = -1;
                }

                for (int i = 0; i < image_size + 1; i++)
                {                  
                    O += w[i] * train_examples[drawn][i];                   
                }

                for (int j = 0; j < image_size + 1; j++)
                {
                    w[j] += learn_var * (C - O) * train_examples[drawn][j];
                }

                if(round % 10000 == 0)
                {
                    ERROR += Math.Pow(O - C, 2);
                }

                round++;
            }
            units.Add(w);
        }
    }
}
