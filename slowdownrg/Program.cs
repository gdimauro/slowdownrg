using System;
using System.Diagnostics;
using System.Linq;

namespace slowdownrg
{
  class Program
  {
    static void Main(string[] args)
    {
      Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
      Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x1;

      for (; ; )
      {
        var all = Process.GetProcessesByName("rg").OrderBy(i => i.StartTime);

        int i = 10;
        int changed = 0;
        foreach(var p in all)
        {
          try
          {
            var priorityClass = i >= 0 ? ProcessPriorityClass.BelowNormal : ProcessPriorityClass.Idle;
            if (!p.HasExited && p.PriorityClass != priorityClass)
            {
              p.PriorityClass = priorityClass;
              p.ProcessorAffinity = (IntPtr)0x7;
              changed++;
              Console.Write(i >= 0 ? '+' : '-');
            }
          }
          catch { }
          i--;
        }
        System.Threading.Thread.Sleep(changed == 0 ? 500 : 100);
        //Console.Write("....");
      }
    }
  }
}
