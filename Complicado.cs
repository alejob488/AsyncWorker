using System;
using System.Collections.Generic;
using System.Text;

namespace TestAsyncWorker {
    
    class Complicado {
        private List<object> m_List = new List<object>(100000);
        static Object s_protector = new Object();
        public void Add(object[] objects) {
            lock (s_protector) {
                foreach (object addItem in objects) {
                    this.m_List.Add(addItem);
                }
            }
        }

        public void Print() {
            lock (s_protector) {
                foreach (object item in this.m_List) {
                    Console.WriteLine(item.ToString());
                }
            }
        }

        public void Clear() {
            lock (s_protector) {
                this.m_List.Clear();
            }
        }
    }
}
