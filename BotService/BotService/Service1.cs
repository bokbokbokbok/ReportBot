using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Threading;

namespace BotService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //var task = new Thread(() => McgTgBotNet.Program.Main(new string[0]));
            //task.Start();
            McgTgBotNet.Program.Main(new string[0]);
        }

        protected override void OnStop()
        {
            //McgTgBotNet.Program.Dispose();
        }
    }
}
