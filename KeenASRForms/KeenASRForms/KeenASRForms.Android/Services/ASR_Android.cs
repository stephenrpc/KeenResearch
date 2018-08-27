using KeenASRForms.Interfaces;
using KeenASRForms.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Com.Keenresearch.Keenasr;
using static Com.Keenresearch.Keenasr.KASRRecognizer;

namespace KeenASRForms.Droid.Services
{
    public class ASR_Android : IASR, IKASRRecognizerListener
    {

        private static KASRRecognizer recognizer = null;
        private KASRResult lastResult = null;

        //  RPC 14Mar2018 #700  Support Silence detection
        private bool fSpeechRecognitionRunning = false;
        private DateTime lastSpeechResult = DateTime.Now;
        private const int WATCHDOGTIMER = 1000; // 1 second watchdog
        private const int WATCHDOGTIMEOUT = 5; // 5 seconds of silence

        public IntPtr Handle => throw new NotImplementedException();

        public ASR_Android()
        {
            //  RPC 14Mar2018 #700  Support Silence detection
            watchDogTimer();
        }

        //  RPC 14Mar2018 #700  Support Silence detection
        async void watchDogTimer()
        {
            while (true)
            {
                await Task.Delay(WATCHDOGTIMER);

                if (fSpeechRecognitionRunning)
                {
                    TimeSpan ts = DateTime.Now - lastSpeechResult;
                    if (ts.TotalSeconds > WATCHDOGTIMEOUT)
                    {
                        stopListening();
                        MessagingCenter.Send<IASR, ASRReturn>(this, "SilenceDetected", null);
                    }
                }

            }
        }


        public bool initialize(string bundlename)
        {
            try
            {
                bundlename = "keenB2sQT-nnet3chain-en-us";
                List<string> assets = new List<string>();
                assets.Add(bundlename + "/decode.conf");
                assets.Add(bundlename + "/final.dubm");
                assets.Add(bundlename + "/final.ie");
                assets.Add(bundlename + "/final.mat");
                assets.Add(bundlename + "/final.mdl");
                assets.Add(bundlename + "/global_cmvn.stats");
                assets.Add(bundlename + "/ivector_extractor.conf");
                assets.Add(bundlename + "/mfcc.conf");
                assets.Add(bundlename + "/online_cmvn.conf");
                assets.Add(bundlename + "/splice.conf");
                assets.Add(bundlename + "/splice_opts");
                assets.Add(bundlename + "/wordBoundaries.int");
                assets.Add(bundlename + "/words.txt");
                assets.Add(bundlename + "/lang/lexicon.txt");
                assets.Add(bundlename + "/lang/phones.txt");
                assets.Add(bundlename + "/lang/tree");

                string asrBundleRootPath = global::Android.App.Application.Context.ApplicationInfo.DataDir;
                string asrBundlePath = asrBundleRootPath + "/" + bundlename;

                KASRBundle asrBundle = new KASRBundle(global::Android.App.Application.Context);

                asrBundle.InstallASRBundle(assets, asrBundleRootPath);

                KASRRecognizer.SetLogLevel(KASRRecognizerLogLevel.KASRRecognizerLogLevelInfo);
                KASRRecognizer.InitWithASRBundleAtPath(asrBundlePath, global::Android.App.Application.Context);
                recognizer = KASRRecognizer.SharedInstance();
                recognizer.AddListener(this);

                // KeenASR.Initialize(bundlename);
                if (recognizer == null)
                    return false;

                var test = recognizer.RecognizerState;
                System.Diagnostics.Debug.WriteLine("After Init, Recognizer state is " + test);


                //recognizer.Delegate = this;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool createDecodingGraph(string decodingGraphName, string[] sentances)
        {
            List<string> objects = new List<string>();
            return KASRDecodingGraph.CreateDecodingGraphFromSentences(sentances, recognizer, decodingGraphName);
        }

        public bool prepareForListening(string decodingGraphName)
        {
            var result = recognizer.PrepareForListeningWithCustomDecodingGraphWithName(decodingGraphName);

            var test = recognizer.RecognizerState;
            System.Diagnostics.Debug.WriteLine("After PrepareForListeningWithCustom, Recognizer state is " + test);

            return result;
        }

        public bool startListening()
        {
            //  RPC 14Mar2018 #700  Support Silence detection
            lastSpeechResult = DateTime.Now;
            fSpeechRecognitionRunning = true;

            var result = recognizer.StartListening();

            var test = recognizer.RecognizerState;
            System.Diagnostics.Debug.WriteLine("After Start Listening, Recognizer state is " + test);
            return result;
        }


        public List<int> getLastResult()
        {
            List<int> result = new List<int>();

            if (lastResult != null)
            {
                string[] text = lastResult.CleanText.Split(' ');
                foreach (var item in text)
                {
                    string lower = item.ToLower().Trim();

                    switch (lower)
                    {
                        case "zero":
                        case "0":
                            result.Add(0);
                            break;
                        case "one":
                            result.Add(1);
                            break;
                        case "two":
                            result.Add(2);
                            break;
                        case "three":
                            result.Add(3);
                            break;
                        case "four":
                            result.Add(4);
                            break;
                        case "five":
                            result.Add(5);
                            break;
                        case "six":
                            result.Add(6);
                            break;
                        case "seven":
                            result.Add(7);
                            break;
                        case "eight":
                            result.Add(8);
                            break;
                        case "nine":
                            result.Add(9);
                            break;

                    }
                }
            }

            return result;
        }


        private List<int> getLastResult(KASRResult last)
        {
            List<int> result = new List<int>();

            if (last != null)
            {
                string[] text = last.CleanText.Split(' ');
                foreach (var item in text)
                {
                    string lower = item.ToLower().Trim();

                    switch (lower)
                    {
                        case "zero":
                        case "0":
                            result.Add(0);
                            break;
                        case "one":
                            result.Add(1);
                            break;
                        case "two":
                            result.Add(2);
                            break;
                        case "three":
                            result.Add(3);
                            break;
                        case "four":
                            result.Add(4);
                            break;
                        case "five":
                            result.Add(5);
                            break;
                        case "six":
                            result.Add(6);
                            break;
                        case "seven":
                            result.Add(7);
                            break;
                        case "eight":
                            result.Add(8);
                            break;
                        case "nine":
                            result.Add(9);
                            break;

                    }
                }
            }

            return result;
        }

        public bool SetVADParameter(int vadParameter, float value)
        {
            switch(vadParameter)
            {
                case 0:
                    recognizer.SetVADParameter(KASRVadParameter.KASRVadTimeoutForNoSpeech, value);
                    break;
                case 1:
                    recognizer.SetVADParameter(KASRVadParameter.KASRVadTimeoutEndSilenceForGoodMatch, value);
                    break;
                case 2:
                    recognizer.SetVADParameter(KASRVadParameter.KASRVadTimeoutEndSilenceForAnyMatch, value);
                    break;
                case 3:
                    recognizer.SetVADParameter(KASRVadParameter.KASRVadTimeoutMaxDuration, value);
                    break;

            }
           
            return true;
        }

        public void SetCreateAudioRecordings(bool value)
        {
            recognizer.CreateAudioRecordings = (Java.Lang.Boolean)value;
        }

        public string GetLastRecordingFilename()
        {
            string lastFile = recognizer.LastRecordingFilename;


            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //var library = Path.Combine(documents, "..", "Library");
            //var targetFolder = Path.Combine(library, subFolder);
            var targetFolder = Path.Combine(documents, "testing");
            if (!System.IO.Directory.Exists(targetFolder))
                System.IO.Directory.CreateDirectory(targetFolder);

            string fileName = DateTime.Now.ToString("yyyyMMdd-hhmmss.flac");
            var outputFile = Path.Combine(targetFolder, fileName);


            var inputStream = File.OpenRead(lastFile);

            //var outputStream = new WaveOverFlacStream(File.Create(outputFile), WaveOverFlacStreamMode.Encode);

            //CopyStreams(inputStream, outputStream);

            //outputStream.Close();
            inputStream.Close();

            return outputFile;
        }



        private static void CopyStreams(Stream s, Stream t)
        {
            byte[] buffer = new byte[0x1000];
            int read = s.Read(buffer, 0, buffer.Length);
            while (read > 0)
            {
                t.Write(buffer, 0, read);
                read = s.Read(buffer, 0, buffer.Length);
                Console.Write('.');
            }
        }


        public void RecognizerFinalResult(KASRResult result, KASRRecognizer recognizer)
        {
            System.Diagnostics.Debug.WriteLine("Final");
        }


        void RecognizerReadyToListenAfterInterrupt(KASRRecognizer recognizer)
        {
            int i = 0;
        }

        public bool stopListening()
        {

            //  RPC 14Mar2018 #700  Support Silence detection
            fSpeechRecognitionRunning = false;

            var test = recognizer.RecognizerState;
            System.Diagnostics.Debug.WriteLine("ASR: Before calling stopListening, Recognizer state is " + test);

            recognizer.StopListening();
            return true;


        }


        public void stopListeningAndReturnResultWhenReady(int testCard)
        {
            //  RPC 14Mar2018 #700  Support Silence detection
            fSpeechRecognitionRunning = false;


            Task.Run(() =>
            {

                var test = recognizer.RecognizerState;
                System.Diagnostics.Debug.WriteLine("Before calling stopListeningAndReturn, Recognizer state is " + test);

                lastResult = recognizer.StopListeningAndReturnFinalResult();
                var lst = lastResult;
                if (lst != null)
                {
                    System.Diagnostics.Debug.WriteLine("---------------------");
                    if (lst.Words != null)
                    {
                        foreach (var word in lst.Words)
                        {
                            System.Diagnostics.Debug.Write(" " + word.Text + " " + word.StartTime + " " + word.Duration);
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("---------------------");
                }

                var ret = new ASRReturn();
                ret.FileName = GetLastRecordingFilename();
                ret.Result = getLastResult(lst);
                ret.TestCardNumber = testCard;
                string jsonFileName = recognizer.LastJSONMetadataFilename;
                if (jsonFileName != null)
                    ret.jsonResult = File.ReadAllText(jsonFileName);
                else
                    ret.jsonResult = "";

                MessagingCenter.Send<IASR, ASRReturn>(this, "RecordingStopped", ret);

                // i reworked this to get the filename and results from withing this thread
            });
        }





        public void OnFinalResult(KASRRecognizer recognizer, KASRResult result)
        {
            System.Diagnostics.Debug.WriteLine("Final result: " + result.CleanText);
        }

        public void OnPartialResult(KASRRecognizer recognizer, KASRResult result)
        {
            //  RPC 14Mar2018 #700  Support Silence detection
            if (fSpeechRecognitionRunning)
                lastSpeechResult = DateTime.Now;

            System.Diagnostics.Debug.WriteLine("ASR: Partial " + result.CleanText);
        }

        public void Dispose()
        {
        }
    }
}
