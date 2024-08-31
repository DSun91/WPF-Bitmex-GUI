using BitmexGUI.ViewModels;

namespace UnitTestingModelView  
{
    public class MyViewModelTests
    {
        private MainViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new MainViewModel(50,"BTCUSDT","15m","XBTUSD");
        }

        [Test]
        public void TestInitialPropertyValues()
        {
            //Assert.AreEqual("ExpectedValue", _viewModel.InstrumentInfo.Count);
        }

        [Test]
        public void TestCommandExecution()
        {
            //_viewModel.MyCommand.Execute(null);

            //Assert.AreEqual("ExpectedValueAfterCommand", _viewModel.MyProperty);
        }
    }
}