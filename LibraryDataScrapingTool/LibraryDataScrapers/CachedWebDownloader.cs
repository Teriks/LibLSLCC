#region FileInfo
// 
// File: CachedWebDownloader.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 
#endregion

using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
#if __MonoCS__
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
#endif

namespace LibraryDataScrapingTools.LibraryDataScrapers
{
    public class CachedWebDownloader
    {
        private string _cacheDirectory;

        private IDbConnection _currentCacheIndexConnection;

        public bool DiskCacheEnabled { get; set; }

        public string CacheDirectory
        {
            get { return _cacheDirectory; }
            set
            {
                if (_cacheDirectory != value)
                {
                    _cacheDirectoryCheckRequired = true;
                    _cacheIndexFileCheckRequired = true;
                }
                _cacheDirectory = value;
            }
        }

        private bool _cacheIndexFileCheckRequired;
        private bool _cacheDirectoryCheckRequired;
        private string _currentCacheIndexFile;

        public CachedWebDownloader(string cacheDirectory)
        {
            CacheDirectory = cacheDirectory;
            DiskCacheEnabled = true;
        }



        public byte[] DownloadData(string url)
        {
            var cache = ReadCache(url, File.ReadAllBytes);

            if (cache != null)
            {
                return cache;
            }


            var x = new WebClient();
            var data = x.DownloadData(url);

            WriteToCache(url, data, (fileName, bytes) =>
            {
                using (var file = File.Create(Path.Combine(CacheDirectory, fileName)))
                {

                    file.Write(bytes, 0, bytes.Length);
                    file.Flush(true);
                }
            });

            return data;
        }


        public string DownloadString(string url, Encoding encoding)
        {
            return encoding.GetString(DownloadData(url));
        }


        public string DownloadString(string url)
        {
            var cache = ReadCache(url, File.ReadAllText);

            if (cache != null)
            {
                return cache;
            }


            var x = new WebClient();
            var data = x.DownloadString(url);

            WriteToCache(url, data, (fileName, stringContent) =>
            {
                using (var file = new StreamWriter(File.Create(Path.Combine(CacheDirectory, fileName))))
                {
                    file.AutoFlush = true;
                    file.Write(stringContent);
                    file.Flush();
                }
            });

            return data;
        }

        private IDbConnection CreateConnection(string file, bool createFile)
        {

            IDbConnection con;
#if __MonoCS__

			if(createFile){
				SqliteConnection.CreateFile(file);
			}

			con = new SqliteConnection(string.Format("Data Source={0};Version=3;", file));

#else
            if (createFile)
            {
                SQLiteConnection.CreateFile(file);
            }

            con = new SQLiteConnection(string.Format("Data Source={0};Version=3;", file));

#endif
            return con;
        }

        private void WriteToCache<T>(string url, T writeData, Action<string, T> writeAction)
        {
            if (!DiskCacheEnabled) return;

            IDbCommand cmd;
            if (_cacheDirectoryCheckRequired && !Directory.Exists(CacheDirectory))
            {
                Directory.CreateDirectory(CacheDirectory);
                _cacheDirectoryCheckRequired = false;

            }

            _currentCacheIndexFile = Path.Combine(CacheDirectory, "index.db");


            if (_cacheIndexFileCheckRequired && !File.Exists(_currentCacheIndexFile))
            {
                if (_currentCacheIndexConnection != null)
                {
                    _currentCacheIndexConnection.Close();
                    _currentCacheIndexConnection = null;
                }


                _currentCacheIndexConnection = CreateConnection(_currentCacheIndexFile, true);


                _currentCacheIndexConnection.Open();


                using (cmd = _currentCacheIndexConnection.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE idx (url VARCHAR, CONSTRAINT unique_url UNIQUE (url));";
                    cmd.ExecuteNonQuery();
                }

                _cacheIndexFileCheckRequired = false;
            }


            if (_currentCacheIndexConnection == null)
            {
                _currentCacheIndexConnection = CreateConnection(_currentCacheIndexFile, false);
                _currentCacheIndexConnection.Open();
            }


            using (cmd = _currentCacheIndexConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT rowid FROM idx WHERE url = ?;";

                var param = cmd.CreateParameter();
                param.DbType = DbType.AnsiString;
                param.Value = url;

                cmd.Parameters.Add(param);


                var exists = cmd.ExecuteScalar();

                if (exists != null)
                {
                    writeAction.Invoke(Path.Combine(CacheDirectory, ((long)exists) + ".cache"), writeData);
                    return;
                }


                cmd.CommandText = "INSERT INTO idx (url) VALUES(?);";

                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT last_insert_rowid()";


                var id = (long)cmd.ExecuteScalar();

                writeAction.Invoke(Path.Combine(CacheDirectory, id + ".cache"), writeData);
            }

        }

        private T ReadCache<T>(string url, Func<string, T> readAction) where T : class
        {
            if (!DiskCacheEnabled || _currentCacheIndexConnection == null) return null;


            using (var cmd = _currentCacheIndexConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT rowid FROM idx WHERE url = ?;";

                var param = cmd.CreateParameter();
                param.DbType = DbType.AnsiString;
                param.Value = url;

                cmd.Parameters.Add(param);

                var id = cmd.ExecuteScalar();
                if (id != null)
                {
                    return readAction.Invoke(Path.Combine(CacheDirectory, ((long)id) + ".cache"));
                }
            }

            return null;
        }
    }
}
