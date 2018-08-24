using System;
using System.Collections.Generic;
using System.Text;

namespace KeenASRForms.Interfaces
{
    public interface IASR
    {
        bool createDecodingGraph(string decodingGraphName, string[] sentances);
        string GetLastRecordingFilename();
        List<int> getLastResult();
        bool initialize(string bundlename);
        bool prepareForListening(string decodingGraphName);
        void SetCreateAudioRecordings(bool value);
        bool SetVADParameter(int vadParameter, float value);
        bool startListening();
        bool stopListening();
        void stopListeningAndReturnResultWhenReady(int testCard);
    }
}
