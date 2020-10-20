using System;
using System.Collections.Generic;
using System.IO;

using Grasshopper.Kernel;
using Rhino.Geometry;

using DeepSpeechClient;

namespace Speech.GH_Components
{
    public class SST : GH_Component
    {
        bool active = false;

        DeepSpeech deepSpeech = new DeepSpeech(@"C:\Users\psf\source\repos\faust\libs\DeepSpeach\deepspeech-0.6.0-models\output_graph.pb");

        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public SST()
          : base("DeepSpeech", "dS",
              "Mozilla DeepSpeach client implmentation",
              "Audio", "Speech")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Audio", "A", "Audio file to transcribe", GH_ParamAccess.item);
            pManager.AddTextParameter("Language Model", "lmP", "Path to the folder containg the language model", GH_ParamAccess.item);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Transcript", "T", "The transcribed audio", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string audioPath = string.Empty;
            string transcript = string.Empty;

            string lmPath = @"C:\Users\psf\source\repos\faust\libs\DeepSpeach\deepspeech-0.6.0-models\";

            string aModelPath = Path.Combine( lmPath + "output_graph.pb"); //output_graph.pb
            string lmBinary = Path.Combine( lmPath + "lm.binary"); //lm.binary 
            string triePath = Path.Combine( lmPath + "trie"); //trie

            UInt16 aBeamWidth = 500;
            float lm_alpha = 0.75f;
            float lm_beta = 1.85f;


            DA.GetData("Audio", ref audioPath);
            DA.GetData("Language Model", ref lmPath) ;

            if (!active)
            {
                deepSpeech.CreateModel(aModelPath, aBeamWidth);
                deepSpeech.EnableDecoderWithLM(lmBinary, triePath, lm_alpha, lm_beta);

                active = true;
            }


            if (audioPath != string.Empty) 
            {
                var waveBuffer = new NAudio.Wave.WaveBuffer(File.ReadAllBytes(audioPath));
                transcript = deepSpeech.SpeechToText(waveBuffer.ShortBuffer,
                        Convert.ToUInt32(waveBuffer.MaxSize / 2));
            }



            DA.SetData("Transcript", transcript);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Icons.SpeechRecognition;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("90464f35-4ddb-4122-a6ce-cb60a45bc5ec"); }
        }
    }
}