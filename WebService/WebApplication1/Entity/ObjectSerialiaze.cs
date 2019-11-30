using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace WebApplication1.Entity
{
    public class ObjectSerialiaze
    {
        public static byte[] XmlSerial(object o, Type type)
        {
            var ser = new XmlSerializer(type);
            var memStream = new MemoryStream();
            ser.Serialize(memStream, o);
            var buffer = memStream.GetBuffer();
            buffer = GZip.Compress(buffer);
            return buffer;
        }

        public static object XmlDeSerial(byte[] buffer,Type type)
        {

            buffer = GZip.DeCompress(buffer);
            var memStream = new MemoryStream(buffer) { Position = 0 };
            var ser = new XmlSerializer(type);
            return ser.Deserialize(memStream);
        }



        public static object Desrialize(byte[] buffer)
        {
            try
            {
                buffer = GZip.DeCompress(buffer);
                var memStream = new MemoryStream(buffer) { Position = 0 };
                var deserializer = new BinaryFormatter();
                var newobj = deserializer.Deserialize(memStream);
                memStream.Close();
                return newobj;
            }
            catch (System.Exception ex)
            {
                throw;
                //throw new MyException(TraceCategory.Common, ErrorCodes.Standard.DeserializationFailed, ex, Convert.ToBase64String(buffer));
            }
        }

        public static byte[] Serialiaze(object obj)
        {
            try
            {
                var serializer = new BinaryFormatter();
                var memStream = new MemoryStream();
                serializer.Serialize(memStream, obj);
                var buffer = memStream.GetBuffer();
                buffer = GZip.Compress(buffer);
                return buffer;
            }
            catch (Exception ex)
            {
                throw;
                //throw new MyException(TraceCategory.Common, ErrorCodes.Standard.SerializationFailed, ex);
            }
        }
    }

    internal static class GZip
    {
        public static byte[] Compress(byte[] data)
        {
            MemoryStream target = null;
            try
            {
                target = new MemoryStream();
                using (var gs = new GZipStream(target, CompressionMode.Compress, true))
                {
                    gs.Write(data, 0, data.Length);
                }
                return target.ToArray();
            }
            finally
            {
                if (target != null)
                {
                    target.Dispose();
                }
            }
        }

        public static byte[] DeCompress(byte[] data)
        {
            using (var source = new MemoryStream())
            {
                using (var gs = new GZipStream(new MemoryStream(data), CompressionMode.Decompress, true))
                {
                    var bytes = new byte[4096];
                    int n = gs.Read(bytes, 0, bytes.Length);
                    while (n != 0)
                    {
                        source.Write(bytes, 0, n);
                        n = gs.Read(bytes, 0, bytes.Length);
                    }
                }
                return source.ToArray();
            }
        }
    }
}