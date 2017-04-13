using System;
using System.Collections.Generic;
using System.Text;

namespace TestAsyncWorker {
     
    class JustDoSomething {
        static Complicado s_complicado;
        static Object m_complicadoProtector = new Object();
        public JustDoSomething() {
            s_complicado = new Complicado();
        }

        public void Print() {
            lock (m_complicadoProtector) {
                s_complicado.Print();
            }
        }

        public void Add(object[] objects) {
            lock (m_complicadoProtector) {
                s_complicado.Add(objects);
            }
        }

        public void Clear() {
            lock (m_complicadoProtector) {
                s_complicado.Clear();
            }
        }
    }
}
