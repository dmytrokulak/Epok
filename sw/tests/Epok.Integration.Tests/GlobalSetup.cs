using NUnit.Framework;

namespace Epok.Integration.Tests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ServerManager.EnsureServerIsStarted();
            DbManager.DropAndCreateDb();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() 
            => ServerManager.EnsureServerIsStopped();
    }
}
