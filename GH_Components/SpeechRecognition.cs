using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
//using Google.Cloud.Speech.V1s
using System.Speech.Recognition;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Speech.GH_Components
{
    public class SpeechRecognition : GH_Component
    {
        SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
        DictationGrammar dgrammer = new DictationGrammar();
        string recText = string.Empty;


        void recEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var result = new SolveResults();
            recText = e.Result.Text;
            recognizer.RecognizeAsyncStop();
            ExpireSolution(true);
        }


        /// <summary>
        /// Class that holds the solved results
        /// </summary>
        public class SolveResults
        {
            public string Text { get; set; }
        }




        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SpeechRecognition()
          : base("SpeechRecognition", "SpeechRecognition",
              "Return speach as text",
              "Audio", "Speech")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("run", "run", "Activate this to run the component", GH_ParamAccess.item, false);
            pManager.AddTextParameter("language", "lang", "The language code example: en-US, en-UK or dk-DK", GH_ParamAccess.item, "en-US");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("info", "info", "This out put provides information about the current state", GH_ParamAccess.list);
            pManager.AddTextParameter("text", "text", "The recogniesed voice input as text", GH_ParamAccess.list);
        }


        /// <summary>
        /// Setting up the component
        /// </summary>
        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();

            if (recognizer.Grammars.Count < 1)
            recognizer.LoadGrammarAsync(dgrammer);
            try 
            {
                recognizer.SetInputToDefaultAudioDevice();
            }
            catch { }

            //recEngine.UpdateRecognizerSetting(language, lang);

            /*  string lang = "en-US";
                DA.GetData("language", ref lang);
                if (DA.GetData("language", ref lang))
                {
                    new CultureInfo(lang);
                    //recEngine.Dispose();
                    //recEngine.
                }*/

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            bool run = false;

            DA.GetData("run", ref run);

            if (!run) { DA.SetData("info", "Press the button and speak into the microphone"); }
            if (run)
            {
                DA.SetData("info", "Please speak now");


                recognizer.RecognizeAsync(RecognizeMode.Single);
                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recEngine_SpeechRecognized);
            }

            DA.SetData("text", recText);

        }

        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();
        }

        public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Icons.SpeechRecognition;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("77110198-f3ad-4f6b-ab58-aa3a6a61a742"); }
        }
    }
}
