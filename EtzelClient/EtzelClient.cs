using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
namespace Etzel
{
	public struct stupid
	{
		public string qname;
		public string cmd;
		public int delay;
		public int expires;
		public string msg;
		public string uid;

	}

	public class EtzelClient

	{

		public  WebSocket ws;
		public delegate void Tqbacks(stupid data);

		//public Tqbacks []qbacks;
		IDictionary<string, Tqbacks> qbacks = new Dictionary<string, Tqbacks>();


		public EtzelClient(string host)
		{

			ws=new WebSocket (host);
			using ( ws ) {
				ws.OnMessage += (sender, e) => {
					//Console.WriteLine (e.Data);
					stupid s=JsonConvert.DeserializeObject<stupid>(e.Data);
					if(s.cmd=="nomsg"){
						this.isleep(s.qname);
					}
					if(s.cmd=="awk"){
						this.fetch(s.qname);
					}
					if(s.cmd=="msg"){
						this.qbacks[s.qname](s);
						this.fetch(s.qname);
					}

				};

			}

			ws.Connect ();


		}





		public void publish(string qname, string message, object options=null)
		{
			stupid s = new stupid();
			s.qname = qname;
			s.cmd = "PUB";
			s.msg = message;
			string data = JsonConvert.SerializeObject(s);

			//Console.WriteLine(data);

			ws.Send(data);




		}
		public void isleep(string qname)
		{
			stupid s = new stupid();
			s.qname = qname;
			s.cmd = "ISLP";
			string data = JsonConvert.SerializeObject(s);

			ws.Send(data);



		}
		public void acknowledge(string qname,string uid)
		{
			stupid s = new stupid();
			s.qname = qname;
			s.cmd = "ACK";
			string data = JsonConvert.SerializeObject(s);

			ws.Send(data);




		}
		public void fetch(string qname)
		{
			stupid s = new stupid();
			s.qname = qname;
			s.cmd = "FET";
			string data = JsonConvert.SerializeObject(s);

			this.ws.Send(data);




		}
		public void subSendCmd(string qname)
		{

			stupid s = new stupid();
			s.qname = qname;
			s.cmd = "SUB";

			string data = JsonConvert.SerializeObject(s);
			this.ws.Send(data);


		}


		public void subscribe(string qname,Tqbacks callback)
		{
			this.subSendCmd(qname);
			this.qbacks[qname] = callback;
			this.fetch(qname); 
		}

	}
}