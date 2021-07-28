using DLS.Helper.Abstracts;
using DLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLS.ViewModel.Configs
{
    [Serializable]
    public class ClassConfig : BinarySerializable<ClassConfig>
    {
        public List<Class> Classes { get; set; }

        public ClassConfig()
        {
            Classes = new List<Class>();
        }
    }
}
