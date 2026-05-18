using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Game
{
	// Dapper note: matches column names case-insensitively but does NOT honor [Column] attribute.
	public class GameDataEntity
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Key][Column("id")]
		public int      id          { get; set; }
		[Column("data")]        public string   data        { get; set; }
		[Column("revision")]    public long     revision    { get; set; }
		[Column("is_delete")]   public bool     is_delete   { get; set; }
		[Column("access_time")] public DateTime access_time { get; set; }
	}

	public class GameDataHistory
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Key][Column("id")]
		public long     id             { get; set; }
		[Column("sid")]            public string   sid            { get; set; }
		[Column("data_table")]     public string   data_table     { get; set; }
		[Column("table_number")]   public int      table_number   { get; set; }
		[Column("data_id")]        public long     data_id        { get; set; }
		[Column("data_revision")]  public long     data_revision  { get; set; }
		[Column("query")]          public char     query          { get; set; }
		[Column("access_time")]    public DateTime access_time    { get; set; }
	}

	public class GameDataSnapshot
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Key][Column("id")]
		public long     id              { get; set; }
		[Column("snapshot_time")]  public DateTime snapshot_time  { get; set; }
		[Column("sid")]            public string   sid            { get; set; }
		[Column("data_table")]     public string   data_table     { get; set; }
		[Column("table_number")]   public int      table_number   { get; set; }
		[Column("data_id")]        public int      data_id        { get; set; }
		[Column("data_revision")]  public long     data_revision  { get; set; }
		[Column("data")]           public string   data           { get; set; }
		public string? decompress_data { get; set; }
	}

	public class IndexData
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Key][Column("id")]
		public long     id            { get; set; }
		[Column("sid")]           public string   sid           { get; set; }
		[Column("data_table")]    public string   data_table    { get; set; }
		[Column("table_number")]  public int      table_number  { get; set; }
		[Column("data_id")]       public int      data_id       { get; set; }
		[Column("data_revision")] public long     data_revision { get; set; }
		[Column("update_time")]   public DateTime update_time   { get; set; }
		[Column("register_time")] public DateTime register_time { get; set; }

		public string GetFixedDataTableName() => $"{data_table}_{table_number}";
	}

	public class HistoryData
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Key][Column("id")]
		public long     id            { get; set; }
		[Column("sid")]           public string   sid           { get; set; }
		[Column("data_table")]    public string   data_table    { get; set; }
		[Column("table_number")]  public int      table_number  { get; set; }
		[Column("data_id")]       public int      data_id       { get; set; }
		[Column("data_revision")] public long     data_revision { get; set; }
		[Column("query")]         public char     query         { get; set; }
		[Column("access_time")]   public DateTime access_time   { get; set; }
	}

	public class HistoryDataJoinData
	{
		[Key] public long     id            { get; set; }
		public string   sid           { get; set; }
		public string   data_table    { get; set; }
		public int      table_number  { get; set; }
		public int      data_id       { get; set; }
		public long     data_revision { get; set; }
		public char     query         { get; set; }
		public DateTime access_time   { get; set; }
		public int      data2         { get; set; }
	}

	public class SnapshotData
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Key][Column("id")]
		public long     id            { get; set; }
		[Column("sid")]           public string   sid           { get; set; }
		[Column("data_table")]    public string   data_table    { get; set; }
		[Column("table_number")]  public int      table_number  { get; set; }
		[Column("data_id")]       public int      data_id       { get; set; }
		[Column("data_revision")] public long     data_revision { get; set; }
		[Column("snapshot_time")] public DateTime snapshot_time { get; set; }
		[Column("data")]          public string   data          { get; set; }

		public string GetFixedDataTableName() => $"{data_table}_{table_number}";
	}
}
