using System.ServiceProcess;

namespace BotService
{
    public partial class Service1: ServiceBase
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
