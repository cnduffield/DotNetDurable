namespace SimpleRecv
{
    using System;
    using Amqp;
    using Amqp.Framing;
	using Amqp.Types;
	using Amqp.Sasl;
    using System.Threading;

    class SimpeRecv
    {
        static void Main(string[] args)
        {
            // uncomment the following to write frame traces
            // Trace.TraceLevel = TraceLevel.Frame;
            // Trace.TraceListener = (f, a) => Console.WriteLine(DateTime.Now.ToString("[hh:mm:ss.fff]") + " " + string.Format(f, a));

            string url = (args.Length > 0) ? args[0] :
                "amqp://guest:guest@127.0.0.1:61616"; //change to your port
            //string source = (args.Length > 1) ? args[1] : "test-topic"; //your topic name
            string source = (args.Length > 1) ? args[1] : "test-topic"; //your topic name
            int     count = (args.Length > 2) ? Convert.ToInt32(args[2]) : 1000;

            Address      peerAddr = new Address(url);
            Open open = new Open();
            open.ContainerId = "12345abcd"; //this must be unique for each receive client to register as durable locater
            Connection connection = new Connection(peerAddr,null,open,null);
            Session       session = new Session(connection);
            Source detailedSrc = new Source();
            detailedSrc.Address = source;
            detailedSrc.ExpiryPolicy = new Symbol("never"); //this may not be needed
            detailedSrc.Durable = 1; //this may not be needed
			detailedSrc.DistributionMode = new Symbol("copy"); //this may not be needed
			ReceiverLink receiver = new ReceiverLink(session, "recv-1", detailedSrc,null);
			
            for (int i = 0; i < count; i++)
            {
                try
                {
                    Message msg = receiver.Receive(20000);
                    receiver.Accept(msg);
                    Console.WriteLine("Received: " + msg.Body.ToString());
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }

            receiver.Close();
            session.Close();
            connection.Close();
        }
    }
}
