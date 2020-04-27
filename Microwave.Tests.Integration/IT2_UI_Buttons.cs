using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Tests.Integration
{
    [TestFixture]
    class IT10UIToButton
    {
        private UserInterface _userInterface;

        //Button
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;

        //CookControler
        private IPowerTube _powerTube;
        private ITimer _timer;

        //Userinterface
        private IOutput _output;
        private IDoor _door;
        private IDisplay _display;
        private ILight _light;
        private ICookController _cooker;


        [SetUp]
        public void SetUp()
        {
            _output = new Output();

            //Buttons
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();

            //Substitudes    
            _door = Substitute.For<IDoor>();

            //UI Entities
            _timer = new Timer();
            _display = new Display(_output);
            _light = new Light(_output);

            //CookController Entities
            _powerTube = new PowerTube(_output);
            _cooker = new CookController(_timer, _display, _powerTube);

            //Userinterface
            _userInterface = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _cooker);
        }

        //PowerButton
        //tryk på power, og se om display viser det rigtige
        [Test]
        public void PowerButton_PowerIs50()
        {
            _powerButton.Press();
            _display.ShowPower(Arg.Is<int>(100));
        }
    }





    //[TestFixture]
        //class IT2_UI_Buttons
        //{
        //    private IButton _timeButton;
        //    private IButton _powerButton;
        //    private IButton _startCancelButton;
        //    private IUserInterface _userInterface;

        //    private ILight _light;
        //    private IDoor _door;
        //    private IDisplay _display;
        //    private ITimer _timer;
        //    private ICookController _cookController;

        //    private IOutput _output;

        //    [SetUp]
        //    public void SetUp()
        //    {
        //        _timeButton = new Button();
        //        _powerButton = new Button();
        //        _startCancelButton = new Button();  

        //        _light = Substitute.For<ILight>();
        //        _display = Substitute.For<IDisplay>();
        //        _timer = Substitute.For<ITimer>();
        //        _cookController = Substitute.For<ICookController>();

        //        _userInterface = new UserInterface(
        //            _powerButton, _timeButton, _startCancelButton,
        //            _door, _display, _light, _cookController);
        //    }

        //    #region PowerButton

        //    [Test]
        //    public void PressPowerButton_EventHandled_ShowPowerCalled()
        //    {
        //        _powerButton.Press();
        //        _display.ShowPower(Arg.Is<int>(50));
        //    }

        //    #endregion






    }

