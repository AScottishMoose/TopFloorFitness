using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/** Goals to accomplish: 
1. Create a UI examplet to show Alan
	a. Create a static test to show what it would look like by loading up the controls manually
		Work on graphics with UI and get that looking nice.
2. Create the operational method that runs a workout 
	a. Create class architecture to drive test behavior
		1. Method Archtecture
		2. Data managemetn
		3. Data saving/loading from file
	b. Write Methods for individual template classes
3. Implement the load from file functions and write the templates
4. ...
5. Profit!
	**/
namespace TopFloorFitness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Session thissession;
      
        public MainWindow() 
        {
            List<string> TheExList = new List<string>();


            thissession = new Session();// This is the main State Machine class that does all the heavy lifting.
                                        // My intention is for this class to run in the background and recieve/handle the commands and inputs from the user UI.
                                        // Since this is primarily ment for presentation (timing, workout selection, etc) There isn't much input past 'select,start,pause,stop'
                                        // I believe that this will need to get launched in a new thread using StatemachineThreading and some kind of reference passed to allow it and this class to communicate.

            InitializeComponent();

            // THe following is my attempt and testing some binding methods and theories that I'd dug up online.  I haven't had any luck so far and I'm starting to wonder if
            // this isn't the correct context.  I have been able to directly write to the fields here using their names and .text fields. Actual binding has been a no go.
            // I believe that I need to use data binding so that I can send commands from the state machine class up to the UI, though the vehicle that actually does this work isn't something I really understand yet.
            // According to the MSDN docs you should have 4 parts to the binding process... from MSDN...
            /*Typically, each binding has these four components: a binding target object, a target property, a binding source, and a path to the value in the binding source to use. */
            // and I don't really understand how to implement that properly so that's probably not helping my case much...
            Exercises.ItemsSource = TheExList;
            ExerciseLabel = "This is some more text";
            thissession.ExerciseList.Add("this is a string");
            thissession.ExerciseList.Add("Hey another string!");
        }

        private void StatemachineThreading()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.Sleep(100);
                ExerciseLabel = "This is some more text";
                thissession.ExerciseList.Add("this is a string");
                thissession.ExerciseList.Add("Hey another string!");
            }).Start();

        }

        public string ExerciseLabel
        {
            get { return thissession.ExerciseLabel; }
            set { thissession.ExerciseLabel = value; }
}

    }
}
