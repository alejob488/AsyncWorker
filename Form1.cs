using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TestAsyncWorker {
    public partial class Form1 : Form {
        AsyncWorker m_asyncWorker;
        AsyncWorker m_bacgroundPrintWorker;
        JustDoSomething m_justDoSomething;
        private delegate void SimpleCallDelegate();
        public Form1() {
            InitializeComponent();
            this.m_asyncWorker = new AsyncWorker(1);
            this.m_asyncWorker.DoWork += new DoWorkEventHandler(m_asyncWorker_DoWork);
            this.m_asyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_asyncWorker_RunWorkerCompleted);
            this.m_bacgroundPrintWorker = new AsyncWorker(1);
            this.m_bacgroundPrintWorker.DoWork += new DoWorkEventHandler(m_bacgroundPrintWorker_DoWork);
            this.m_justDoSomething = new JustDoSomething();
        }

        void m_asyncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
                this.Invoke(new SimpleCallDelegate(this.SetText));
        }

        private void SetText(){
            this.label1.Text = "bw ended" + DateTime.UtcNow.ToString();
            Console.WriteLine("bw ended");
        }

      

        void m_bacgroundPrintWorker_DoWork(object sender, DoWorkEventArgs e) {
            this.m_justDoSomething.Print();
        }

        void m_asyncWorker_DoWork(object sender, DoWorkEventArgs e) {
            object[] objects = { 1, 2, 3, 4, 5, 6, 7, 8 };
            this.m_justDoSomething.Add(objects);
        }

        private void button1_Click(object sender, EventArgs e) {
            for (int increment = 0; increment < 100; increment++) {
                for (int i = 0; i < 1000; i++) {
                    if (!this.m_asyncWorker.IsBusy) {
                        if (!this.m_asyncWorker.RunWorkerAsync(true)) {
                            Console.WriteLine("Worker in use....");
                        }
                    }
                    if (!this.m_bacgroundPrintWorker.IsBusy) {
                        this.m_bacgroundPrintWorker.RunWorkerAsync(true);
                    }
                }
                this.m_justDoSomething.Clear();
            }
        }
    }
}
