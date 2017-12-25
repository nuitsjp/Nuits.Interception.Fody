using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PCLStorage;

namespace InterceptionApp.Models
{
    public class TransactionContext : IDisposable
    {
        public const string DatabaseFileName = "Calculator.db";

        private static AsyncLocal<TransactionContext> _asyncLocal = new AsyncLocal<TransactionContext>();


        public static TransactionContext Current
        {
            get
            {
                if (_asyncLocal.Value == null)
                {
                    throw new InvalidOperationException("TransactionContext is not initialized.");
                }
                return _asyncLocal.Value;
            }
        }
        public TransactionContext()
        {
            _asyncLocal.Value = this;
        }

        public async Task BeginTransaction()
        {
            await CreateDatabase();
        }

        public void Commit()
        {
            
        }

        private void Close()
        {
            
        }

        private async Task CreateDatabase()
        {
            // ルートフォルダを取得する
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            // ファイルシステム上のDBファイルの存在チェックを行う
            var result = await rootFolder.CheckExistsAsync(DatabaseFileName);
            if (result == ExistenceCheckResult.NotFound)
            {
                // 存在しなかった場合、新たに空のDBファイルを作成する
                var newFile = await rootFolder.CreateFileAsync(DatabaseFileName, CreationCollisionOption.ReplaceExisting);
                // Assemblyに埋め込んだDBファイルをストリームで取得し、空ファイルにコピーする
                var assembly = typeof(TransactionContext).GetTypeInfo().Assembly;
                using (var stream = assembly.GetManifestResourceStream("Calculator.db"))
                {
                    using (var outputStream = await newFile.OpenAsync(FileAccess.ReadAndWrite))
                    {
                        stream.CopyTo(outputStream);
                        outputStream.Flush();
                    }
                }
            }
        }

        public void Dispose()
        {
            _asyncLocal.Value = null;
        }
    }
}
