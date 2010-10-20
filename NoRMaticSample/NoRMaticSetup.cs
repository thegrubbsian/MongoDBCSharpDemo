using System.Configuration;
using NoRMatic;

namespace NoRMaticSample {

    public class NoRMaticSetup : INoRMaticInitializer {

        public void Setup() {

            NoRMaticConfig.SetConnectionStringProvider(() =>
                ConfigurationManager.ConnectionStrings["NoRMaticConnectionString2"].ConnectionString);

            NoRMaticConfig.SetCurrentUserProvider(() => "JC Grubbs");

            Subscriber.EnableVersioning();
            Subscriber.AddQueryBehavior(x => x.City == "Charlotte");
        }
    }
}
