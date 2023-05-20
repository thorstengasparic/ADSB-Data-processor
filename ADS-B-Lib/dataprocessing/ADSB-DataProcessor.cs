using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ADS_B_Lib.dataprocessing
{
    public interface IADSB_DataProcessor
    {
        void RawData(byte[] rawoutput);

    }
    public class DataProcessorFactory
    {
        public static IADSB_DataProcessor Create(IMessageFIFO messageFifo)
        {
            return new ADSB_DataProcessor(messageFifo);
        }
    }
    internal class ADSB_DataProcessor : IADSB_DataProcessor
    {
        IMessageFIFO messageFifo;
        public ADSB_DataProcessor(IMessageFIFO messageFifo)
        {
            this.messageFifo = messageFifo;
        }

        void IADSB_DataProcessor.RawData(byte[] rawoutput)
        {
            String strMessages = ConvertRawDataToString(rawoutput);
            List<String> messageList = GetLines(strMessages);
            QueueData(messageList);
        }
        String ConvertRawDataToString(byte[] rawoutput)
        {
            return Encoding.UTF8.GetString(rawoutput);
        }

        void QueueData(List<String> messageList)
        {
            
            foreach (String message in messageList)
            {
                this.messageFifo.Put(message);
            }
        }

        List<String> GetLines(string text)
        {
            List<string> lines = new List<string>();
            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(ms);
                sw.Write(text);
                sw.Flush();

                ms.Position = 0;

                string line;

                using (StreamReader sr = new StreamReader(ms))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
                sw.Close();
            }
            return lines;
        }
    }
}
