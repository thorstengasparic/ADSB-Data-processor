using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS_B_Lib.dataprocessing
{
    public interface IMessageFIFO
    {
        String Get();
        void Put(string data);

    }
    
    public class FIFOFactory
    {
        public static IMessageFIFO Create()
        {
            return new MessageFIFO();
        }
    }
    internal class MessageFIFO : IMessageFIFO
    {
        ConcurrentQueue<String> stringFifo= new ConcurrentQueue<String>();
        string IMessageFIFO.Get()
        {
            String? outString;
            if (stringFifo.TryDequeue(out outString)) return outString;
            return String.Empty;

        }

        public void Put(string data)
        {
            stringFifo.Enqueue(data);   
        }
    }
}
