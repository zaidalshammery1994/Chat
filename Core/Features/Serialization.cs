using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Core.Features
{

    public class Serialization
    {

        public bool Save<T>(List<T> list, string path)
        {
            try
            {
                using (Stream stream = File.Open(path, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(stream, list);
                }
                return true;
            }
            catch (IOException e)
            {
                new Exceptions.IOException(e);
            }
            catch (Exception e)
            {
                new Exceptions.UnknowException(e);
            }
            return false;
        }

        public List<T> Load<T>(string path)
        {
            try
            {
                using (Stream stream = File.Open(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    List<T> data = (List<T>)bf.Deserialize(stream);
                    return data;
                }
            }
            catch (IOException e)
            {
                new Exceptions.IOException(e);
            }
            catch (Exception e)
            {
                new Exceptions.UnknowException(e);
            }
            return null;
        }
    }
}
