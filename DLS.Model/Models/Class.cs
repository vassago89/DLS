using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLS.Model.Models
{
    [Serializable]
    public class Class
    {
        public UInt64 RealID { get; private set; }
        public double ID { get; private set; }
        public string Name { get; private set; }
        public ImageData ImageData { get; private set; }

        public Class()
        {
            
        }

        public Class(double id, string name, ImageData imageData)
        {
            ID = id;
            Name = name;
            ImageData = imageData;
        }
    }
}
