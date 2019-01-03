using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace WSIPGClient
{
    /// <summary>
    /// Class for saving Request and Response Xml code to file to be able to take a look at the Xml client is sending and Xml the server is sending.
    /// </summary>
    public class LoggerSoapExtension : SoapExtension
    {
        private static readonly string LOG_DIRECTORY = ConfigurationManager.AppSettings["LOG_DIRECTORY"];
        private LogStream _logger;

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return null;
        }

        public override object GetInitializer(Type serviceType)
        {
            return null;
        }

        public override void Initialize(object initializer)
        {
        }

        public override System.IO.Stream ChainStream(System.IO.Stream stream)
        {
            _logger = new LogStream(stream);
            return _logger;
        }

        public override void ProcessMessage(SoapMessage message)
        {
            if (LOG_DIRECTORY != null)
            {
                switch (message.Stage)
                {
                    case SoapMessageStage.BeforeSerialize:
                        _logger.Type = "request";
                        break;
                    case SoapMessageStage.AfterSerialize:
                        break;
                    case SoapMessageStage.BeforeDeserialize:
                        _logger.Type = "response";
                        break;
                    case SoapMessageStage.AfterDeserialize:
                        break;
                }
            }
        }

        internal class LogStream : Stream
        {
            private Stream _source;
            private Stream _log;
            private bool _logSetup;
            private string _type;

            public LogStream(Stream source)
            {
                _source = source;
            }
            internal string Type
            {
                set { _type = value; }
            }
            private Stream Logger
            {
                get
                {
                    if (!_logSetup)
                    {
                        if (LOG_DIRECTORY != null)
                        {
                            try
                            {
                                DateTime now = DateTime.Now;
                                string folder = LOG_DIRECTORY + now.ToString("yyyyMMdd");
                                string subfolder = folder + "\\" + now.ToString("HH");
                                string client = System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.UserHostAddress != null ? System.Web.HttpContext.Current.Request.UserHostAddress : string.Empty;
                                string ticks = now.ToString("yyyyMMdd'T'HHmmss.fffffff");
                                if (!Directory.Exists(folder))
                                    Directory.CreateDirectory(folder);
                                if (!Directory.Exists(subfolder))
                                    Directory.CreateDirectory(subfolder);
                                _log = new FileStream(new System.Text.StringBuilder(subfolder).Append('\\').Append(client).Append('_').Append(ticks).Append('_').Append(_type).Append(".xml").ToString(), FileMode.Create);
                            }
                            catch
                            {
                                _log = null;
                            }
                        }
                        _logSetup = true;
                    }
                    return _log;
                }
            }

            public override bool CanRead
            {
                get
                {
                    return _source.CanRead;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    return _source.CanSeek;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    return _source.CanWrite;
                }
            }

            public override long Length
            {
                get
                {
                    return _source.Length;
                }
            }

            public override long Position
            {
                get
                {
                    return _source.Position;
                }
                set
                {
                    _source.Position = value;
                }
            }

            public override void Flush()
            {
                _source.Flush();
                if (Logger != null)
                    Logger.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _source.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _source.SetLength(value);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                count = _source.Read(buffer, offset, count);
                if (Logger != null)
                    Logger.Write(buffer, offset, count);
                return count;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _source.Write(buffer, offset, count);
                if (Logger != null)
                    Logger.Write(buffer, offset, count);
            }

            public override int ReadByte()
            {
                int ret = _source.ReadByte();
                if (ret != -1 && Logger != null)
                    Logger.WriteByte((byte)ret);
                return ret;
            }

            public override void Close()
            {
                _source.Close();
                if (Logger != null)
                    Logger.Close();
                base.Close();
            }

            public override int ReadTimeout
            {
                get { return _source.ReadTimeout; }
                set { _source.ReadTimeout = value; }
            }

            public override int WriteTimeout
            {
                get { return _source.WriteTimeout; }
                set { _source.WriteTimeout = value; }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class LoggerSoapExtensionAttribute : SoapExtensionAttribute
    {
        private int priority = 1;
        public override int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }

        public override System.Type ExtensionType
        {
            get
            {
                return typeof(LoggerSoapExtension);
            }
        }
    }
}
