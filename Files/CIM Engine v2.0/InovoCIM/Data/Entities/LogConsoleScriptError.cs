using InovoCIM.Data.Models;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace InovoCIM.Data.Entities
{
    public class LogConsoleScriptError
    {
        public int LogConsoleScriptErrorID { get; set; }
        public string InstanceID { get; set; }
        public string FileName { get; set; }
        public int FileLine { get; set; }
        public string Script { get; set; }
        public string Column { get; set; }
        public string Input { get; set; }
        public string Type { get; set; }

        public LogConsoleScriptError(string InstanceID, string FileName, int FileLine, string Script)
        {
            this.InstanceID = InstanceID;
            this.FileName = FileName;
            this.FileLine = FileLine;
            this.Script = Script;
        }

        public async Task<int> SaveSync(string Column, string Input, string Type)
        {
            try
            {
                this.Column = Column;
                this.Input = Input;
                this.Type = Type;

                string query = @"INSERT INTO [LogConsoleScriptError] ([InstanceID],[FileName],[FileLine],[Script],[Column],[Input],[Type],[Updated]) VALUES (@InstanceID,@FileName,@FileLine,@Script,@Column,@Input,@Type,GETDATE())";

                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@InstanceID", this.InstanceID);
                    cmd.Parameters.AddWithValue("@FileName", this.FileName);
                    cmd.Parameters.AddWithValue("@FileLine", this.FileLine);
                    cmd.Parameters.AddWithValue("@Script", this.Script);
                    cmd.Parameters.AddWithValue("@Column", this.Column);
                    cmd.Parameters.AddWithValue("@Input", this.Input);
                    cmd.Parameters.AddWithValue("@Type", this.Type);

                    await conn.OpenAsync().ConfigureAwait(false);
                    int ResultID = (int)await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    return ResultID;
                }
            }
            catch (Exception) { return -1; }
        }
    }
}

