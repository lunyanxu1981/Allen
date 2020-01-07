﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace project.service
{

    public class UploadChunkItem
    {
        public byte[] Data { get; set; }
        public int ChunkNumber { get; set; }
        public int ChunkSize { get; set; }
        public string FilePath { get; set; }
    }

    public class UploadChunkWriter
    {
        public static UploadChunkWriter Instance = new UploadChunkWriter();
        private BlockingCollection<UploadChunkItem> _queue;
        private int _writeWorkerCount = 3;
        private Thread _writeThread;
        public UploadChunkWriter()
        {
            _queue = new BlockingCollection<UploadChunkItem>(500);
            _writeThread = new Thread(this.Write);
        }

        public void Write()
        {
            while (true)
            {
                //单线程写入
                //var item = _queue.Take();
                //using (var fileStream = File.Open(item.FilePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                //{
                //    fileStream.Position = (item.ChunkNumber - 1) * item.ChunkSize;
                //    fileStream.Write(item.Data, 0, item.Data.Length);
                //    item.Data = null;
                //}

                //多线程写入
                Task[] tasks = new Task[_writeWorkerCount];
                for (int i = 0; i < _writeWorkerCount; i++)
                {
                    var item = _queue.Take();
                    tasks[i] = Task.Run(() =>
                     {
                         using (var fileStream = File.Open(item.FilePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                         {
                             fileStream.Position = (item.ChunkNumber - 1) * item.ChunkSize;
                             fileStream.Write(item.Data, 0, item.Data.Length);
                             item.Data = null;
                         }
                     });
                }
                Task.WaitAll(tasks);
            }
        }

        public void Add(UploadChunkItem item)
        {
            _queue.Add(item);
        }

        public void Start()
        {
            _writeThread.Start();
        }
    }
}
