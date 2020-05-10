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
        int idle = 0;
        int belowNormal = 0;
        int normal = 0;
        int aboveNormal = 0;
        int realTime = 0;
        int high = 0;
        var all = Process.GetProcessesByName("rg").OrderBy(i => i.StartTime);

        int i = 10;
        int changed = 0;
        foreach (var p in all)
        {
          try
          {
            var priorityClass = i >= 0 ? ProcessPriorityClass.BelowNormal : ProcessPriorityClass.Idle;
            //var affinity = i >= 0 ? (IntPtr)7 : (IntPtr)1;
            if (!p.HasExited && (p.PriorityClass != priorityClass /*|| p.ProcessorAffinity != affinity*/))
            {
              p.PriorityClass = priorityClass;
              //p.ProcessorAffinity = affinity;
              changed++;
              Console.Write(i >= 0 ? '+' : '-');
            }
            switch (priorityClass)
            {
              case ProcessPriorityClass.Idle:
                idle++;
                break;
              case ProcessPriorityClass.BelowNormal:
                belowNormal++;
                break;
              case ProcessPriorityClass.Normal:
                normal++;
                break;
              case ProcessPriorityClass.AboveNormal:
                aboveNormal++;
                break;
              case ProcessPriorityClass.RealTime:
                realTime++;
                break;
              case ProcessPriorityClass.High:
                high++;
                break;
            }
          }
          catch
          {
            Console.Write('*');
          }
          i--;
        }
        Console.Title = $"Total: {all.Count()} Delta: {changed} Idle: {idle} Below: {belowNormal} Normal: {normal} Above: {aboveNormal} Realtime: {realTime} High: {high}";
        System.Threading.Thread.Sleep(changed == 0 ? 500 : 100);
        //Console.Write("....");
      }
    }
  }
}
