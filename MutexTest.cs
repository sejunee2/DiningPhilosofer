using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Diagnostics;

namespace MutexTest
{/// <summary>
/// P1 LightPink
/// P2 Yellow
/// P3 Tomato
/// P4 LightBlue
/// P5 Silver
/// </summary>
    public partial class Form1 : Form
    {
        public static Form1 form;
        public Form1()
        {
            InitializeComponent();
            form = this;
        }

        class Table
        {
            static Mutex gM1 = new Mutex(false);
            static Mutex gM2 = new Mutex(false);
            static Mutex gM3 = new Mutex(false);
            static Mutex gM4 = new Mutex(false);
            static Mutex gM5 = new Mutex(false);
            static Mutex[] gFork = new Mutex[5];

            private object lockObject = new object();

            static bool iF1 = false;
            static bool iF2 = false;
            static bool iF3 = false;
            static bool iF4 = false;
            static bool iF5 = false;
            static bool[] bFork = new bool[5];

            public Table()
            {
                gFork[0] = gM1;
                gFork[1] = gM2;
                gFork[2] = gM3;
                gFork[3] = gM4;
                gFork[4] = gM5;

                bFork[0] = iF1;
                bFork[1] = iF2;
                bFork[2] = iF3;
                bFork[3] = iF4;
                bFork[4] = iF5;
            }
            public bool GetForks(int threadID)
            {
                Mutex[] IFork = new Mutex[2];
                IFork[0] = gFork[threadID];
                IFork[1] = gFork[(threadID + 1) % 5];
                Stopwatch sw = new Stopwatch();
                sw.Start();
                bool result = WaitHandle.WaitAll(IFork,1);
                Console.WriteLine("Time:" + sw.ElapsedMilliseconds.ToString());
                if (result != true)
                {
                    Console.WriteLine("false");
                }
                return result;
            }
            public void DropForks(int threadID)
            {
                gFork[threadID].ReleaseMutex();
                gFork[(threadID + 1) % 5].ReleaseMutex();
            }

            public bool GetBoolForks(int threadID) 
            {
                //if (bFork[1] == true && bFork[2] == true && bFork[3] == true && bFork[4] == true && bFork[0] == true)
                //{
                //    Console.WriteLine("Warnning");
                //}
                //Wait
                lock (lockObject)
                {
                    if (bFork[threadID] == false && bFork[(threadID + 1) % 5] == false)
                    {
                        bFork[threadID] = bFork[(threadID + 1) % 5] = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //Release
                
            }
            public void DropBoolForks(int threadID) 
            {
                bFork[threadID] = false;
                bFork[(threadID + 1) % 5] = false;
            }

            public class Philosopher
            {
                Random rand = new Random();
                public Circle circle;
                public Button buttonR, buttonL;
                public Label eatCount;
                public Color color;
                private int threadID;
                private int cnt = 0;
                private Table aTable;
                

                public Philosopher(int threadId, Table table)
                {
                    this.threadID = threadId;
                    this.aTable = table;
                }
                                
                public void PhilosopherWaiting()
                {
                    if (circle.InvokeRequired)
                    {
                        circle.Invoke(new MethodInvoker(delegate { circle.BackColor = System.Drawing.Color.White; }));
                    }
                    else
                        circle.BackColor = System.Drawing.Color.White;

                    Thread.Sleep(300);

                    if (circle.InvokeRequired)
                    {
                        circle.Invoke(new MethodInvoker(delegate { circle.BackColor = color; }));
                    }
                    else
                        circle.BackColor = color;

                    Thread.Sleep(300);
                }
                private void CircleEat()
                {
                    if (circle.InvokeRequired)
                    {
                        circle.Invoke(new MethodInvoker(delegate { circle.Text = " Eating."; }));
                    }
                    else
                        circle.Text = " Eating.";
                    if (buttonL.InvokeRequired)
                    {
                        buttonL.Invoke(new MethodInvoker(delegate { buttonL.Text = Convert.ToString(threadID + 1); }));
                        buttonL.BackColor = color;
                    }
                    else
                    {
                        buttonL.Text = Convert.ToString(threadID + 1);
                        buttonL.BackColor = color;
                    }
                    if (buttonR.InvokeRequired)
                    {
                        buttonR.Invoke(new MethodInvoker(delegate { buttonR.Text = Convert.ToString(threadID + 1); }));
                        buttonR.BackColor = color;
                    }
                    else
                    {
                        buttonR.Text = Convert.ToString(threadID + 1);
                        buttonR.BackColor = color;
                    }
                    if (eatCount.InvokeRequired)
                    {
                        eatCount.Invoke(new MethodInvoker(delegate { eatCount.Text = Convert.ToString(cnt += 1); }));
                    }
                    else
                        eatCount.Text = Convert.ToString(cnt += 1);
                }
                private void CircleThink()
                {
                    if (circle.InvokeRequired)
                    {
                        circle.Invoke(new MethodInvoker(delegate { circle.Text = " Thinking."; }));
                    }
                    else
                        circle.Text = " Thinking.";
                    if (buttonL.InvokeRequired)
                    {
                        buttonL.Invoke(new MethodInvoker(delegate { buttonL.Text = "free"; }));
                        buttonL.BackColor = System.Drawing.Color.White;
                    }
                    else
                    {
                        buttonL.Text = "free";
                        buttonL.BackColor = System.Drawing.Color.White;
                    }
                    if (buttonR.InvokeRequired)
                    {
                        buttonR.Invoke(new MethodInvoker(delegate { buttonR.Text = "free"; }));
                        buttonR.BackColor = System.Drawing.Color.White;
                    }
                    else
                    {
                        buttonR.Text = "free";
                        buttonR.BackColor = System.Drawing.Color.White;
                    }
                    
                }
                public void Philosophize()
                {
                    for (int i = 0; ; i++)
                    {
                        int iSleep = rand.Next(3000, 5000);

                        for (int n = 0; ; n++)
                        {
                            if (aTable.GetBoolForks(threadID) == true)
                            {
                                break;
                            }
                            else
                            {
                                PhilosopherWaiting();
                            }
                        }
                        CircleEat();
                        Thread.Sleep(iSleep);
                                                
                        CircleThink();                      
                        aTable.DropBoolForks(threadID);                        
                        Thread.Sleep(601);

                        if (cnt == 10)
                        {
                            //return;
                        }
                    }
                }
                
            }
        }
        private void Start_Click(object sender, EventArgs e)
        {
            Table table = new Table();
            Table.Philosopher[] IPhil = new Table.Philosopher[5];
            Thread[] IThread = new Thread[5];

            for (int loopctr = 0; loopctr < 5; loopctr++)
            {
                IPhil[loopctr] = new Table.Philosopher(loopctr, table);

                if (loopctr == 0)
                {
                    IPhil[loopctr].circle = form.circle1;
                    IPhil[loopctr].buttonL = form.button1;
                    IPhil[loopctr].buttonR = form.button2;
                    IPhil[loopctr].color = System.Drawing.Color.LightPink;
                    IPhil[loopctr].eatCount = Eatcount1;
                }
                if (loopctr == 1)
                {
                    IPhil[loopctr].circle = form.circle2;
                    IPhil[loopctr].buttonL = form.button2;
                    IPhil[loopctr].buttonR = form.button3;
                    IPhil[loopctr].color = System.Drawing.Color.Yellow;
                    IPhil[loopctr].eatCount = Eatcount2;
                }
                if (loopctr == 2)
                {
                    IPhil[loopctr].circle = form.circle3;
                    IPhil[loopctr].buttonL = form.button3;
                    IPhil[loopctr].buttonR = form.button4;
                    IPhil[loopctr].color = System.Drawing.Color.Tomato;
                    IPhil[loopctr].eatCount = Eatcount3;
                }
                if (loopctr == 3)
                {
                    IPhil[loopctr].circle = form.circle4;
                    IPhil[loopctr].buttonL = form.button4;
                    IPhil[loopctr].buttonR = form.button5;
                    IPhil[loopctr].color = System.Drawing.Color.LightBlue;
                    IPhil[loopctr].eatCount = Eatcount4;
                }
                if (loopctr == 4)
                {
                    IPhil[loopctr].circle = form.circle5;
                    IPhil[loopctr].buttonL = form.button5;
                    IPhil[loopctr].buttonR = form.button1;
                    IPhil[loopctr].color = System.Drawing.Color.Silver;
                    IPhil[loopctr].eatCount = Eatcount5;
                }

                IThread[loopctr] = new Thread(new ThreadStart(IPhil[loopctr].Philosophize));
                IThread[loopctr].Name = "Philosopher " + loopctr;
                IThread[loopctr].Start();
                
            }
            start.Text = "End";
        }
    }
}