using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace DLS.Helper.Abstracts
{
    public abstract class YamlSerializable<T> where T : class, new()
    {
        [YamlIgnore]
        public string DefualtPath => Path.Combine(Environment.CurrentDirectory, $"{GetType().Name}.yaml");

        private static IDeserializer _deserializer = new DeserializerBuilder().Build();
        private ISerializer _serializer;

        public YamlSerializable()
        {
            _serializer = new SerializerBuilder().Build();
        }

        public static T Deserialize(string filePath)
        {
            if (File.Exists(filePath) == false)
                return new T();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    try
                    {
                        return _deserializer.Deserialize(reader, typeof(T)) as T;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    
                }
            }
        }

        public void Serialize(string filePath = null)
        {
            using (var stream = File.Create(filePath ?? DefualtPath))
            {
                using (var writer = new StreamWriter(stream))
                {
                    _serializer.Serialize(writer, this);
                }
            }
        }
    }

    [Serializable]
    public abstract class BinarySerializable<T> where T : class, new()
    {
        public string DefualtPath => Path.Combine(Environment.CurrentDirectory, $"{GetType().Name}.bin");

        private static BinaryFormatter _formatter = new BinaryFormatter();

        public BinarySerializable()
        {

        }

        public static T Deserialize(string filePath)
        {
            if (File.Exists(filePath) == false)
                return new T();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    return _formatter.Deserialize(stream) as T;
                }
                catch (Exception e)
                {
                    return new T();
                }
            }
        }

        public void Serialize(string filePath = null)
        {
            using (var stream = File.Create(filePath ?? DefualtPath))
            {
                _formatter.Serialize(stream, this);
            }
        }
    }
}
