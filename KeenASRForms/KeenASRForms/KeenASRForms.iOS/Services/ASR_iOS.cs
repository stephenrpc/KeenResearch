using System;
using System.Collections.Generic;
using System.IO;
using Foundation;
using KDTLibrary.KeenASR.IPad;
using Accelerate;
using Xamarin.Forms;
using System.Threading.Tasks;
using FlacBox;
using KeenASRForms.Interfaces;
using KeenASRForms.Models;

namespace KeenASRForms.iOS.Services
{

    public class ASR_iOS : KIOSRecognizerDelegate, IASR
    {

        private static KIOSRecognizer recognizer = null;
        private KIOSResult lastResult = null;

        //  RPC 14Mar2018 #700  Support Silence detection
        private bool fSpeechRecognitionRunning = false;
        private DateTime lastSpeechResult = DateTime.Now;
        private const int WATCHDOGTIMER = 1000; // 1 second watchdog
        private const int WATCHDOGTIMEOUT = 5; // 5 seconds of silence

        public ASR_iOS()
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
            //  Just to force it to be linked
            Accelerate.vImageError enumError = vImageError.BufferSizeMismatch;

            KIOSRecognizer.SetLogLevel(KIOSRecognizerLogLevel.Info);
            KIOSRecognizer.InitWithASRBundle(bundlename);
            recognizer = KIOSRecognizer.SharedInstance;

            var test = recognizer.RecognizerState;
            System.Diagnostics.Debug.WriteLine("After Init, Recognizer state is " + test);

            // KeenASR.Initialize(bundlename);
            if (recognizer == null)
                return false;

            recognizer.Delegate = this;

            return true;
        }

        public bool createDecodingGraph(string decodingGraphName, string[] sentances)
        {
            List<NSObject> objects = new List<NSObject>();
            foreach (var item in sentances)
            {
                objects.Add(NSObject.FromObject(item));
            }

            return KIOSDecodingGraph.CreateDecodingGraphForKeywordSpotting(objects.ToArray(), recognizer, decodingGraphName);

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


        private List<int> getLastResult(KIOSResult last)
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
            recognizer.SetVADParameter((KIOSVadParameter)vadParameter, value);
            return true;
        }

        public void SetCreateAudioRecordings(bool value)
        {
            recognizer.CreateAudioRecordings = value;
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

            var outputStream = new WaveOverFlacStream(
                File.Create(outputFile), WaveOverFlacStreamMode.Encode);

            CopyStreams(inputStream, outputStream);

            outputStream.Close();
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


        public override void UnwindAppAudioBeforeAudioInterrupt()
        {

        }

        public override void RecognizerPartialResult(KIOSResult result, KIOSRecognizer recognizer)
        {
            //  RPC 14Mar2018 #700  Support Silence detection
            if (fSpeechRecognitionRunning)
                lastSpeechResult = DateTime.Now;

            System.Diagnostics.Debug.WriteLine("ASR: Partial " + result.CleanText);
        }

        public void RecognizerFinalResult(KIOSResult result, KIOSRecognizer recognizer)
        {
            System.Diagnostics.Debug.WriteLine("Final");
        }


        void RecognizerReadyToListenAfterInterrupt(KIOSRecognizer recognizer)
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


    }
}