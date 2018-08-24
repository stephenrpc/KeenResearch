using System;
using System.Collections.Generic;
using System.Text;

namespace KeenASRForms.Models
{
    public class ASRReturn
    {
        public ASRReturn() { }
        public string FileName { get; set; }
        public List<int> Result { get; set; }
        public int TestCardNumber { get; set; }
        public string jsonResult { get; set; }
    }
}
