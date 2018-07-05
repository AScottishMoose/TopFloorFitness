using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office;
using System.ComponentModel;

namespace TopFloorFitness
{
    enum ExerTime : UInt16 { TwentyOn, ThirtyOn, FortyOn, SixtyOn };

    enum SessionState : UInt16 { SelectWO, WaitforStart, Start, NextExercise, RunExercise, BuildWorkout, Idle, Shutdown, WOLeadIn }

    enum ExerType : UInt16 { Exercise, Transition, Rest }

    /// <summary>
    /// Session Class for Workout display.  This class handles the state machine functionality and data management
    /// INotifyPropertyChanged class provides the event functionality needed to bind the class data to the UI
    /// </summary>
    class Session : INotifyPropertyChanged 
        
    {
        public event PropertyChangedEventHandler PropertyChanged;
        #region Session_Members

        SessionState m_sessionstate; // defines how the Class will behave when recieving new messages
        Workout m_workout; // Stores all information about the currently loaded workout
        bool m_continue; // 


        //data binding members
        //Define data binding variables
        private List<string> ExList = new List<string>();
        private string m_CrntWorkout;
        private string m_CrntTime;
        private string m_ExTimer;
        private string m_WOTimer;
        private string m_CrntEx;
        private string m_CrntDir;
        private string m_ExLabel;
        private string m_WOTimeLabel;
        #endregion

        #region SessionProperties

        public List<string> ExerciseList
            {
                        get { return ExList; }
    set
			{
				if(ExList != value)

                {
        ExList = value;
        this.NotifyPropertyChanged("ExList");
    }
    }
}

public string Currentworkout
        {
            get { return m_CrntWorkout; }
            set
            {
                if (value != m_CrntWorkout)
                {
                    m_CrntWorkout = value;
                    this.NotifyPropertyChanged("CrntWorkout");
                }
            }
        }

        public string CurrentTime
        {
            get { return m_CrntTime; }
            set
            {
                if (value != m_CrntTime)
                {
                    m_CrntTime = value;
                    this.NotifyPropertyChanged("CrntTime");
                }
            }
        }

        public string ExerciseTimer
        {
            get { return m_ExTimer; }
            set
            {
                if (value != m_ExTimer)
                {
                    m_ExTimer = value;
                    this.NotifyPropertyChanged("ExTimer");
                }
            }
        }

        public string WorkoutTimer
        {
            get { return m_WOTimer; }
            set
            {
                if (value != m_WOTimer)
                {
                    m_WOTimer = value;
                    this.NotifyPropertyChanged("WOTime");
                }
            }
        }

        public string CurrentExercise
        {
            get { return m_CrntEx; }
            set
            {
                if (value != m_CrntEx)
                {
                    m_CrntEx = value;
                    this.NotifyPropertyChanged("CrntExercise");
                }
            }
        }

        public string Directions
                    {
            get { return m_CrntDir; }
            set
            {
                if (value != m_CrntDir)
                {
                    m_CrntDir = value;
                    this.NotifyPropertyChanged("Directions");
                }
            }
        }

        public string ExerciseLabel
        {
            get { return m_ExLabel; }
            set
            {
                if (value != m_ExLabel)
                {
                    m_ExLabel = value;
                    this.NotifyPropertyChanged("ExerciseLabel");
                }
            }
        }

        public string WorkoutTimeLabel
        {
            get { return m_WOTimeLabel; }
            set
            {
                if (value != m_WOTimeLabel)
                {
                    m_WOTimeLabel = value;
                    this.NotifyPropertyChanged("WOTimeLabel");
                }
            }
        }

        public bool Continue()
        {
            return m_continue;
        }
        #endregion

        #region Session_Constructors

        public Session()
        {
            m_sessionstate = SessionState.SelectWO;
            m_continue = true;
            ExerciseLabel = "This is text";

        }

        #endregion

        #region Session_Methods
        /// <summary>
        /// State Machine Handler.  This is the method that I believe should be called as a thread, or at least a while loop containing this method.
        /// Takes input from the user interface and allows the class to change states based on 
        /// what is given.  The class has the right to handle the event how it wants.
        /// </summary>
        /// <param name="Input"></param>
        public void HandleInput(string Input) 
        {
            switch (m_sessionstate)
            {
                case SessionState.BuildWorkout:
                    {
                        m_workout.LoadfromFile(Input);
                        break;
                    }
                case SessionState.Idle:
                    {
                        System.Threading.Thread.Sleep(100);
                        break;
                    }
                case SessionState.NextExercise:
                    {
                        m_workout.NextExercise();
                        m_sessionstate = SessionState.RunExercise;
                        break;
                    }
                case SessionState.RunExercise:
                    {// in this case we execute an update to the UI through the current workout class.
                        // This update should be done using data binding to the WPF objects.  HOW DO MAGNETS WORK?!?!?! srsly I don't know...
                        m_workout.UpdateUI();

                        if (m_workout.ExerciseComplete())
                        {
                            m_sessionstate = SessionState.NextExercise;
                        }
                        break;
                    }

                //This SelectWO case for phase 1 will only generate a static workout instead of attempting to load from file.
                case SessionState.SelectWO:
                    {

                        // Steps to Completion 
                        // 1. Get WO File - Open WO file
                        // 2. Resolve Class type / Create Class
                        // 3. Refer file to Class generator
                        // 4. Transition into 'Wait for Start'

                        //1. Get WO File - Call Open File Dialog. 
                        //Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                        //dlg.DefaultExt = ".xlsx";
                        //dlg.Filter = Input;
                        //bool? result;
                        //    result = dlg.ShowDialog();

                        //if (result==true)
                        //{
                        //    string filename = dlg.FileName;
                        //   //Microsoft.Office.Core.
                        //}

                        //2. 3. Resolve Class type / Create Class
                        CreateSampleWorkout();

                        m_sessionstate = SessionState.WaitforStart;

                        break;
                    }
                case SessionState.Shutdown:
                    {
                        break;
                    }
                case SessionState.Start:
                    {
                        break;
                    }
                case SessionState.WaitforStart:
                    {
                        if (Input == "StartWorkout")
                        {
                            m_sessionstate = SessionState.WOLeadIn;
                            m_workout.StartLeadIn();
                        }
                        break;
                    }
                case SessionState.WOLeadIn:
                    {
                        m_workout.UpdateUI();
                        if (m_workout.LeadInComplete())
                        {
                            m_sessionstate = SessionState.NextExercise;
                        }
                        break;
                    }
            }
        }

        public void NotifyPropertyChanged(string propname)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propname));
            }
        }
        /// <summary>
        /// Returns important information to the UI on what it should be presenting right now.
        /// This can include text for all fields.
        /// </summary>

        private void CreateSampleWorkout()
        {

            m_workout = new SuperSet();

            m_workout.Loadexample();

        }




        #endregion
    }
    class Workout
    {
        #region Workout_Members
        //</ Members0
        ExerTime m_ExTime = ExerTime.TwentyOn; // This will get selected from the workout file
        String[] m_ExList; // Loaded from the list given in the workout file
        UInt32 m_ExCount; // Number of times a Circuit, Exercise, or Superset gets rerun 
        System.DateTime m_WOStartTime; // Time that the workout was started
        System.DateTime m_ExStartTime; // Time that the currently active workout was started
        System.DateTime m_LeadinStartTime; // Time that the leadin starts.  This is used to mark the lead in before the exercise actually starts

        #endregion

        #region Workout_Properties

        public string[] ExerciseList
        {
            get { return m_ExList; }
            set { m_ExList = value; }
        }

        public ExerTime ExerciseTiming
        {
            get { return m_ExTime; }
            set { m_ExTime = value; }
        }

        public UInt32 ExerciseCount
        {
            get { return m_ExCount; }
            set { m_ExCount = value; }
        }

        public DateTime WorkoutStartTime
        {
            get { return m_WOStartTime; }
        }
        #endregion

        #region Workout_Constructors
        public Workout()
        {

        }

        #endregion

        #region Workout_Methods

        public Int32 LoadfromFile(string path)
        {
            //Phase 2 Implementation
            return 0;
        }


        /// <summary>
        /// Transitions the class into the next exercise
        /// </summary>
        public void NextExercise()
        {
            m_ExStartTime = DateTime.Now;
            
        }

        /// <summary>
        /// sends all the information needed to update the User Interface
        /// </summary>
        public void UpdateUI()
        {

        }

        /// <summary>
        /// Generates an example workout for testing w/o the excel read function.
        /// </summary>
        public void Loadexample()
        {
            ExerciseList = new string[] { "Static Lunge Hold", "Dynamic Lunge", "High Knees","Side Plank Twist",
                                                    "Static Lateral Lunge","Dynamic Lateral Lunge", "Ice Skaters", "Side Plank Dip" };
            ExerciseTiming = ExerTime.TwentyOn;
            m_ExCount = 3;
            BuildWorkoutList();
        }

        public void StartLeadIn()
        {
             m_LeadinStartTime = System.DateTime.Now;
            
        }
        

        public bool ExerciseComplete()
        {
            double Runtime = ExerciseTime();
            TimeSpan duration = DateTime.Now - m_ExStartTime;

            return duration.Seconds > Runtime;
        }

        public bool LeadInComplete()
        {
            bool result;
            TimeSpan duration =
                duration = DateTime.Now - m_LeadinStartTime;
            result = duration.Seconds > 10;

            if (result)  { m_WOStartTime = DateTime.Now; }

            return result;
        }

        private UInt16 ExerciseTime()
        {
            UInt16 Result = 0;
            switch (m_ExTime)
            {
                case ExerTime.TwentyOn:
                    {
                        Result = 20;
                        break;
                    }
                case ExerTime.ThirtyOn:
                    {
                        Result = 30;
                        break;
                    }
                case ExerTime.FortyOn:
                    {
                        Result = 40;
                        break;
                    }
                case ExerTime.SixtyOn:
                    {
                        Result = 60;
                        break;
                    }

            }
            return Result;
        }
    

        private UInt16 TransitionTime()
        {
            return Convert.ToUInt16(ExerciseTime() / 2);

        }

        /// <summary>
        /// Takes the information that is available and creates a workout list with all exercises built in
        /// This list is then just executed from start to finish.
        /// Basically all that work here is done in the child because they will define the rules for order.
        /// </summary>
        private void BuildWorkoutList()
        {

        }
        #endregion
    }
    #region WorkoutChildren
    class Circuit : Workout
        {
            // A circuit workout just takes takes the list and repeats it a given number of times.
            // The # of times repeated is a custom value for this workout
            uint m_CirRepeat;

        public new void Loadexample()
        {
            m_CirRepeat = 4;
        }
        public void BuildWorkoutList()
        {
            UInt32 i = m_CirRepeat;
        }
        }

    class SuperSet : Workout
    {
        UInt32 m_setcount;

        #region Properties
        public UInt32 SuperSetSize
        {
            get { return m_setcount; }
            set { m_setcount = value; }
        }


        /// <summary>
        /// Loads the functions of this class for the example
        /// </summary>
        public new void Loadexample()
        {
            base.Loadexample();
            SuperSetSize = 4;
        }

        private void BuildWorkoutList()
        {
            UInt32 i = 0;
            UInt32 j = 0;
            string[] placehldr = new string[] { };
            //ExerciseList = new string[] { "Static Lunge Hold", "Dynamic Lunge", "High Knees","Side Plank Twist",
            //                                        "Static Lateral Lunge","Dynamic Lateral Lunge", "Ice Skaters", "Side Plank Dip" };
            //ExerciseTiming = ExerTime.TwentyOn;
            //m_ExCount = 3;
            //BuildWorkoutList();
            // Things to do here... Take exercise list and split it by the SuperSet Size then loop those
            /// totalphases = ExerciseCount * SuperSetSize + ExerciseCount;

            while (ExerciseList.Length <= i)
            {

                for (j = 0; j <= SuperSetSize; j++)
                {
                    try
                    { // we run a try/catch here in case the length of exercises isn't integer divisible by SuperSetLength. This would result in an array length error
                        placehldr[placehldr.Length + 1] = ExerciseList[i + j];
                    }
#pragma warning disable CS0168 // Variable is declared but never used
                    catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
                    { }

                };
                placehldr[placehldr.Length + 1] = "Rest";
                i = i + j;
            };
        }
    }


    class Tabata : Workout
    {

    }
    class Random : Workout
    {

    }

    #endregion

    }

#endregion