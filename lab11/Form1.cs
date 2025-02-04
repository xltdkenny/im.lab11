﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace lab11
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        double[] prob = new double[5];
        double[] freq = new double[5];
        Random x = new Random();
        double p;
        double S;
        double SN;
        List<double> VR = new List<double>();

        public void GetVR(double mean, double var)
        {
            S = 0;
            p = x.NextDouble();
            S = Math.Sqrt(-2 * Math.Log(p));
            p = x.NextDouble();
            S = S * Math.Cos(2 * Math.PI * p); //бокса-мюллера
            SN = Math.Sqrt(var) * S + mean;

            VR.Add(SN);
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
                chart1.Series[0].Points.Clear();
                VR.Clear();
                double mean = (double)means.Value;
                double var = (double)variances.Value;

                double N = (double)countN.Value;
                for (int i = 0; i < N; i++)
                {
                    GetVR(mean, var);
                }

                double max = -100;
                double min = 100;

                foreach (double v in VR)
                {
                    if (v > max) max = v;
                    if (v < min) min = v;
                }
                double interval = max - min;
                double k = Math.Ceiling(Math.Sqrt(N));
                double step = interval / k;
                int k1 = (int)k;

                double[] intervals = new double[k1];
                for (int i = 0; i < k; i++)
                {
                    intervals[i] = min + (i + 1) * step;
                }

                //заполняем таблицу с частотами
                double[] stat = new double[k1];
                foreach (double v in VR)
                {
                    double temp = v;
                    int m = -1;
                    if (temp == min)
                    {
                        stat[0]++;
                    }
                    else
                    if (temp == max)
                    {
                        stat[stat.Length - 1]++;
                    }
                    else
                    {
                        while (temp > min)
                        {
                            m++;
                            temp -= step;
                        }
                        stat[m]++;
                    }
                }

                double[] freq = new double[k1];

                //относительная частота
                for (int i = 0; i < stat.Length; i++)
                {
                    freq[i] = stat[i] / N;
                    chart1.Series[0].Points.AddXY(i + 1, freq[i]);
                }

                double avrg = 0;
                foreach (double v in VR)
                {
                    avrg += v;
                }
                avrg = avrg / N;

                double variance = 0;
                foreach (double v in VR)
                {
                    variance += Math.Pow(v, 2);
                }
                variance = variance / N - Math.Pow(avrg, 2);
                meanLabel.Text = avrg.ToString();
                varianceLabel.Text = variance.ToString();

                double prob = step * ((max + min) / 2);
                double ChiSq = 0;
                for (int i = 0; i < stat.Length; i++)
                {
                    ChiSq += (stat[i] * stat[i]) / (N * prob);
                }
                ChiSq = Math.Round(ChiSq, 2);
                chiLabel.Text = "" + ChiSq;
                double tf;
                switch (k1)
                {
                    case 4:
                        chiLabel.Text += " > 9.488";
                        tf = 9.488;
                        break;
                    case 6:
                        chiLabel.Text += " > 12.592";
                        tf = 12.592;
                        break;
                    case 9:
                        chiLabel.Text += " > 16.919";
                        tf = 16.919;
                        break;
                    case 10:
                        chiLabel.Text += " > 18.307";
                        tf = 18.307;
                        break;
                    case 100:
                        chiLabel.Text += " > 150";
                        tf = 150;
                        break;
                    default:
                        chiLabel.Text += " > 50.307";
                        tf = 50.307;
                        break;
                }

                if (ChiSq > tf) ToFLabel.Text = "is true";
                else ToFLabel.Text = "is false";
        }
    }
}
