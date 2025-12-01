using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AgentForms.Models
{
    internal class ProcPidi
    {
        private int processId = -1;
        public  Process? Proc { get; set; }
        //public Pidi() { }

        public ProcPidi(int processid) {
            this.processId = processid;
            var proc = Process.GetProcessById(this.processId);
            if (proc != null)
            {
                this.Proc = proc;
            }
            else 
            { 
                this.Proc = null;
            }
        }

        public double WorkingMemoryMB
        {
            get
            {
                if (this.Proc != null)
                {
                    return Math.Round(this.Proc.WorkingSet64 / 1024.0 / 1024.0, 2);
                }
                return 0;
            }
        }
        public double WorkingMemoryGB
        {
            get
            {
                if (this.Proc != null)
                {
                    return Math.Round(this.Proc.WorkingSet64 / 1024.0 / 1024.0 / 1024.0, 2);
                }
                return 0;
            }
        }


        public string Path { get; set; }
    }
}
